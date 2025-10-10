using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Client
{
    //玩家判定是否触碰到物品
    public class DotweenHelper : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                DotweenTest();
            }
        }


        void DotweenTest()
        {
             #region Position 位置
            //改变世界坐标
            //移动方法，第一个参数是要移动到的目标点，不是移动这个向量的距离
            //transform.DOMove(new Vector3(1, 1, 1), 2);
            //只控制x轴上的移动，其他两个方向同理
            //transform.DOMoveX(1, 2);

            //改变局部坐标
            //transform.DOLocalMove(new Vector3(1, 1, 1), 2);
            // transform.DOLocalMoveX(1, 2);
            #endregion

            #region Rotation 旋转
            // 1）世界旋转
            // 旋转到给定的值，改变的是欧拉角
            // transform.DORotate(new Vector3(0, 90, 0), 2);
            // 旋转到给定的值，改变的是四元数
            // transform.DORotateQuaternion(new Quaternion(0.1f, 0.1f, 0.1f, 0.1f), 2);
            //
            // 2）局部旋转
            // 旋转到给定的值，改变的是欧拉角
            // transform.DOLocalRotate(new Vector3(0, 90, 0), 2);
            // 旋转到给定的值，改变的是四元数
            // transform.DOLocalRotateQuaternion(new Quaternion(0.1f, 0.1f, 0.1f, 0.1f), 2);
            //
            // 在给定时间内，平滑的让自身的z轴正方向指向目标点
            // transform.DOLookAt(new Vector3(0, 0, 0), 2);

            #endregion
            
            #region Scale 缩放
            // 同上面一样，这里改变物体的缩放到目标值
            // transform.DOScale(new Vector3(2, 2, 2), 2);
            // 其他两个轴向同理
            // transform.DOScaleX(3, 2);
            #endregion

            #region Punch 冲击
            // 第一个参数 punch：表示方向及强度
            // 第二个参数 duration：表示动画持续时间
            // 第三个参数 vibrato：震动次数
            // 第四个参数 elascity: 这个值是0到1的
            // 当为0时，就是在起始点到目标点之间运动
            // 不为0时，会把你赋的值乘上一个参数，作为你运动方向反方向的点，物体在这个点和目标点之间运动
            transform.DOPunchPosition(new Vector3(0, 1, 0), 2, 2, 0.1f);
            // transform.DOPunchRotation(new Vector3(0, 90, 0), 2, 2, 0.1f);
            // transform.DOPunchScale(new Vector3(2, 2, 2), 2, 2, 0.1f);

            #endregion

            #region Shake 抖动 

            //text旧版
            // 抖动位置，持续 2 秒，强度 10，频率 20 次
            // this.transform.DOShakePosition(2f, strength: 10f, vibrato: 20, randomness: 90);

            
            // TextMeshPro 或 TextMeshProUGUI
            this.transform.DOShakePosition(2f, 10f, 20, 90);
            
            
            #endregion
        }
    }
}