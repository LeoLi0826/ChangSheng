using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(Move2DComponent))]
    public static partial class Move2DComponentSystem
    {
         [Invoke(TimerInvokeType.Move2DTimer)]
        public class MoveTimer: ATimer<Move2DComponent>
        {
            protected override void Run(Move2DComponent self)
            {
                try
                {
                    self.MoveForward(true);
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: {self.Id}\n{e}");
                }
            }
        }
    
        
        [EntitySystem]
        private static void Destroy(this Move2DComponent self)
        {
            self.MoveFinish(false);
        }
        
        [EntitySystem]
        private static void Awake(this Move2DComponent self)
        {
            self.StartTime = 0;
            self.StartPos = Vector3.zero;
            self.NeedTime = 0;
            self.MoveTimer = 0;
            self.tcs = null;
            self.Targets.Clear();
            self.Speed = 0;
            self.N = 0;
            self.TurnTime = 0;
        }
        
        public static bool IsArrived(this Move2DComponent self)
        {
            return self.Targets.Count == 0;
        }

        public static bool ChangeSpeed(this Move2DComponent self, float speed)
        {
            if (self.IsArrived())
            {
                return false;
            }

            if (speed < 0.0001)
            {
                return false;
            }
            
            Unit unit = self.GetParent<Unit>();

            using ListComponent<Vector3> path = ListComponent<Vector3>.Create();
            
            self.MoveForward(false);
                
            path.Add(unit.Position); // 第一个是Unit的pos
            for (int i = self.N; i < self.Targets.Count; ++i)
            {
                path.Add(self.Targets[i]);
            }
            self.MoveToAsync(path, speed).NoContext();
            return true;
        }

        // 该方法不需要用cancelToken的方式取消，因为即使不传入cancelToken，多次调用该方法也要取消之前的移动协程,上层可以stop取消
        public static async ETTask<bool> MoveToAsync(this Move2DComponent self, List<Vector3> target, float speed, int turnTime = 100)
        {
            self.Stop(false);

            foreach (Vector3 v in target)
            {
                self.Targets.Add(v);
            }

            self.IsTurnHorizontal = true;
            self.TurnTime = turnTime;
            self.Speed = speed;
            self.tcs = ETTask<bool>.Create(true);

            EventSystem.Instance.Publish(self.Scene(), new MoveStart() {Unit = self.GetParent<Unit>()});
            
            self.StartMove();
            
            bool moveRet = await self.tcs;

            if (moveRet)
            {
                EventSystem.Instance.Publish(self.Scene(), new MoveStop() {Unit = self.GetParent<Unit>()});
            }
            return moveRet;
        }

        // ret: 停止的时候，移动协程的返回值
        private static void MoveForward(this Move2DComponent self, bool ret)
        {
            Unit unit = self.GetParent<Unit>();
            
            long timeNow = TimeInfo.Instance.ClientNow();
            long moveTime = timeNow - self.StartTime;

            while (true)
            {
                if (moveTime <= 0)
                {
                    return;
                }
                
                // 计算位置插值
                if (moveTime >= self.NeedTime)
                {
                    unit.Position = self.NextTarget;
                    if (self.TurnTime > 0)
                    {
                        unit.Rotation = self.To;
                    }
                }
                else
                {
                    // 计算位置插值
                    float amount = moveTime * 1f / self.NeedTime;
                    if (amount > 0)
                    {
                        Vector3 newPos = math.lerp(self.StartPos, self.NextTarget, amount);
                        unit.Position = newPos;
                    }
                    
                    // 计算方向插值
                    if (self.TurnTime > 0)
                    {
                        amount = moveTime * 1f / self.TurnTime;
                        if (amount > 1)
                        {
                            amount = 1f;
                        }
                        quaternion q = math.slerp(self.From, self.To, amount);
                        unit.Rotation = q;
                    }
                }

                moveTime -= self.NeedTime;

                // 表示这个点还没走完，等下一帧再来
                if (moveTime < 0)
                {
                    return;
                }
                
                // 到这里说明这个点已经走完
                
                // 如果是最后一个点
                if (self.N >= self.Targets.Count - 1)
                {
                    unit.Position = self.NextTarget;
                    unit.Rotation = self.To;

                    self.MoveFinish(ret);
                    return;
                }

                self.SetNextTarget();
            }
        }

        private static void StartMove(this Move2DComponent self)
        {
            self.BeginTime = TimeInfo.Instance.ClientNow();
            self.StartTime = self.BeginTime;
            self.SetNextTarget();

            self.MoveTimer = self.Root().GetComponent<TimerComponent>().NewFrameTimer(TimerInvokeType.Move2DTimer, self);
        }

        private static void SetNextTarget(this Move2DComponent self)
        {

            Unit unit = self.GetParent<Unit>();

            ++self.N;

            // 时间计算用服务端的位置, 但是移动要用客户端的位置来插值
            Vector3 v = self.GetFaceV();
            float distance = math.length(v);
            
            // 插值的起始点要以unit的真实位置来算
            self.StartPos = unit.Position;

            self.StartTime += self.NeedTime;
            
            self.NeedTime = (long) (distance / self.Speed * 1000);
            
            if (self.TurnTime > 0)
            {
                // 要用unit的位置
                Vector3 faceV = self.GetFaceV();
                if (math.lengthsq(faceV) < 0.0001f)
                {
                    return;
                }
                self.From = unit.Rotation;
                
                if (self.IsTurnHorizontal)
                {
                    faceV.y = 0;
                }

                if (Math.Abs(faceV.x) > 0.01 || Math.Abs(faceV.z) > 0.01)
                {
                    self.To = quaternion.LookRotation(faceV, math.up());
                }

                return;
            }
            
            if (self.TurnTime == 0) // turn time == 0 立即转向
            {
                Vector3 faceV = self.GetFaceV();
                if (self.IsTurnHorizontal)
                {
                    faceV.y = 0;
                }

                if (Math.Abs(faceV.x) > 0.01 || Math.Abs(faceV.z) > 0.01)
                {
                    self.To = quaternion.LookRotation(faceV, math.up());
                    unit.Rotation = self.To;
                }
            }
        }

        private static Vector3 GetFaceV(this Move2DComponent self)
        {
            return self.NextTarget - self.PreTarget;
        }

        public static bool FlashTo(this MoveComponent self, Vector3 target)
        {
            Unit unit = self.GetParent<Unit>();
            unit.Position = target;
            return true;
        }

        // ret: 停止的时候，移动协程的返回值
        public static void Stop(this Move2DComponent self, bool ret)
        {
            if (self.Targets.Count > 0)
            {
                self.MoveForward(ret);
            }

            self.MoveFinish(ret);
        }

        private static void MoveFinish(this Move2DComponent self, bool ret)
        {
            if (self.StartTime == 0)
            {
                return;
            }
            
            self.StartTime = 0;
            self.StartPos = Vector3.zero;
            self.BeginTime = 0;
            self.NeedTime = 0;
            self.Targets.Clear();
            self.Speed = 0;
            self.N = 0;
            self.TurnTime = 0;
            self.IsTurnHorizontal = false;
            self.Root().GetComponent<TimerComponent>()?.Remove(ref self.MoveTimer);

            if (self.tcs != null)
            {
                var tcs = self.tcs;
                self.tcs = null;
                tcs.SetResult(ret);
            }
        }
    }
}

