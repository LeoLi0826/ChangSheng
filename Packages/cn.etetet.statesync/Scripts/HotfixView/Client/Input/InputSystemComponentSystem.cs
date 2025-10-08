using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ET.Client
{
    [EntitySystemOf(typeof(InputSystemComponent))]
    public static partial class InputSystemComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this InputSystemComponent self)
        {
            self.InputSystem?.Dispose();
        }
        
        [EntitySystem]
        private static void Awake(this InputSystemComponent self)
        {
            self.CinemachineComponent = self.GetParent<Unit>().GetComponent<CinemachineComponent>();

            self.InputSystem = new GameInput();
            self.InputSystem.PlayerInput.Enable();
            self.WasMovingLastFrame = false;
        }

        [EntitySystem]
        private static void Update(this InputSystemComponent self)
        {
            bool isPressed = self.InputSystem.PlayerInput.Move.IsPressed();

            if (isPressed)
            {
                // 有输入：处理移动
                if (TimeInfo.Instance.FrameTime - self.PressTime > 100)
                {
                    self.PressTime = TimeInfo.Instance.FrameTime;
                    Vector2 v = self.InputSystem.PlayerInput.Move.ReadValue<Vector2>();
                    self.Move(v);
                    self.WasMovingLastFrame = true;
                }
            }
            else
            {
                // 无输入：检查是否需要停止移动
                if (self.WasMovingLastFrame)
                {
                    self.StopMove();
                    self.WasMovingLastFrame = false;
                }
            }
        }


        private static void Move(this InputSystemComponent self, Vector2 v)
        {
            Unit unit = self.GetParent<Unit>();

            if (v.magnitude < 0.001f)
            {
                self.StopMove();
                return;
            }


            float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
            if (angle >= -45 && angle < 45)
            {
                unit.Direction = Direction.Back;  // 向前移动时，角色背面朝向
            }
            else if (angle >= 45 && angle < 135)
            {
                unit.Direction = Direction.Left;  // 向右移动时，角色左侧朝向
            }
            else if (angle >= -135 && angle < -45)
            {
                unit.Direction = Direction.Right; // 向左移动时，角色右侧朝向
            }
            else
            {
                unit.Direction = Direction.Front; // 向后移动时，角色正面朝向
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            float speed = numericComponent.GetAsFloat(NumericType.Speed);
            if (speed < 0.01)
            {
                return;
            }
            v = v.normalized * speed * 1f;

            float3 targetPos = new float3(v.x,0,v.y) + unit.Position;

            unit.FindPathMoveToAsync(targetPos,speed).NoContext();
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        private static void StopMove(this InputSystemComponent self)
        {
            Unit unit = self.GetParent<Unit>();

            // 获取 Move2DComponent 并停止移动
            Move2DComponent moveComponent = unit.GetComponent<Move2DComponent>();
            if (moveComponent != null && !moveComponent.IsDisposed)
            {
                moveComponent.Stop(true);
            }
        }

        // 鼠标左键点击目标，设置主角的目标
        private static void SelectTarget(this InputSystemComponent self, InputAction.CallbackContext context)
        {
            // Vector2 mousePosition = Mouse.current.position.ReadValue();
            // Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            // const float maxDistance = 1000.0f;
            // RaycastHit hit;
            // if (!Physics.Raycast(ray, out hit, maxDistance, LayerMask.GetMask("Unit")))
            // {
            //     return;
            // }
            // GameObject clickedObject = hit.collider.gameObject;
            //
            // GameObjectEntityRef gameObjectEntityRef = clickedObject.GetComponent<GameObjectEntityRef>();
            // if (gameObjectEntityRef is null)
            // {
            //     return;
            // }
            //
            // if (gameObjectEntityRef.Entity is not Unit targetUnit)
            // {
            //     return;
            // }
            //
            // Unit myUnit = self.GetParent<Unit>();
            // myUnit.GetComponent<TargetComponent>().Unit = targetUnit;
        }
    }
}