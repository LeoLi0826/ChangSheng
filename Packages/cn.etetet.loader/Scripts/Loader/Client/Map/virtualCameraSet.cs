using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Cinemachine;
using ET;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class virtualCameraSet : MonoBehaviour
{
    public float rotateTime = 0.2f;

    private Transform player;
    private bool isRotating = false;
    
    public float newOrthoSize = 2.5f;
    public float nearClipPlane = -10f;  // 添加近裁剪平面的设置
    [SerializeField] private Transform target;      // 跟随目标
    [SerializeField] private float dampingX = 0.1f;    // 降低平滑系数，使相机跟随更快
    [SerializeField] private float dampingY = 0.1f;    // 降低平滑系数，使相机跟随更快
    [SerializeField] private float dampingZ = 0.01f;    // 降低平滑系数，使相机跟随更快

    
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] public GameObject boundaryCollider; // 3D边界碰撞体
    public Camera Camera { get; private set; }

    public GameObject TopboundaryCollider;
    public GameObject BottomboundaryCollider;
    public GameObject LeftboundaryCollider;
    public GameObject RightboundaryCollider;
    
    public CinemachineTransposer transposer;
    
    public Vector3 defaultOffset = new Vector3(); // 默认偏移值
    
    // 相机跟随设置
    public float maxSpeed = 20f; // 相机最大跟随速度
    public bool useMaxSpeed = true; // 是否使用最大速度限制

    void Start()
    {
        
    }

    public void Camerainit(float size)
    {
        #region 虚拟摄像机相关参数设置

        // //设置摄像机距离
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        // virtualCamera.m_Lens.OrthographicSize = newOrthoSize;
        // virtualCamera.m_Lens.NearClipPlane = nearClipPlane;

        //AfterUnitCreate_GetView在那里赋值
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Log.Debug("虚拟摄像机 没有找到玩家");
        }
        else
        {
            Log.Debug("虚拟摄像机 找到玩家");
            //找到玩家
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        // 初始化虚拟相机
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;

        if (virtualCamera.GetCinemachineComponent<CinemachineTransposer>() == null)
        {
            Log.Debug("虚拟摄像机 没有找到transposer");
        }
        else
        {
            Log.Debug("虚拟摄像机 找到transposer");
            // 设置跟随偏移和平滑
             transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            
             transposer.m_FollowOffset = defaultOffset;
            
             // 设置Body为World Space模式
            // transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            
             // 设置相机跟随平滑度
             transposer.m_XDamping = dampingX;
             transposer.m_YDamping = dampingY;
             transposer.m_ZDamping = dampingZ;
            
            // 在世界空间模式下，通过调整目标位置来实现偏移
            // if (target != null)
            // {
            //     Vector3 targetPosition = target.position;
            //     targetPosition += defaultOffset;
            //     target.position = targetPosition;
            // }
            
           
            
            // // 设置最大跟随速度
            // if (useMaxSpeed)
            // {
            //     transposer.m_MaximumSpeed = maxSpeed;
            // }
        }
       
        #endregion 
        
        
        #region 玩家边界 摄像机碰撞体相关
        
        //人物边界动态初始化
        SetupBoundaries(size);
        
        if (boundaryCollider != null)
        {
            // 确保boundaryCollider在XZ平面上
            boundaryCollider.transform.position = new Vector3(size/2, 0, size/2);
            boundaryCollider.transform.rotation = Quaternion.identity;
            
            // 确保有BoxCollider组件
            BoxCollider boxCollider = boundaryCollider.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = boundaryCollider.AddComponent<BoxCollider>();
            }
            // 设置碰撞体大小
            boxCollider.size = new Vector3(10, 7f, 10);//new Vector3(size, 7f, size); // 高度设为10，可以根据需要调整
            boxCollider.isTrigger = true;
        }
        
        
        // 设置3D边界限制
        Debug.Log("存在？ CinemachineConfiner 组件");
        var confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        if (confiner == null)
        {
            confiner = virtualCamera.gameObject.AddComponent<CinemachineConfiner>();
            Debug.Log("添加 CinemachineConfiner 组件");
        }
        else
        {
            Debug.Log("存在 CinemachineConfiner 组件");
        }
        
        //摄像机边界碰撞
        //confiner.m_BoundingVolume = boundaryCollider.GetComponent<Collider>();
        confiner.m_Damping = 0f; // 设置为0，使相机在碰到边界时立即停下
        
        Camera = virtualCamera.GetComponent<Camera>();
        
        
       
        
        #endregion

    }
    
    private void SetupBoundaries(float size)
    {
        // 边界碰撞体的厚度
        float boundaryThickness = 1f;
        float boundaryHeight = 10f; // 3D边界的高度
        float inwardOffset = 0f; // 向中心靠拢的距离

        // 创建一个父物体来固定所有边界
        GameObject boundariesParent = new GameObject("Boundaries");
        boundariesParent.transform.position = Vector3.zero;
        boundariesParent.transform.rotation = Quaternion.identity;

        // 设置四个边界的位置和大小（3D空间），向中心靠拢1个单位
        SetBoundary(TopboundaryCollider, boundariesParent.transform,
            position: new Vector3(size / 2, 0, size - inwardOffset),
            size: new Vector3(size, boundaryHeight, boundaryThickness));

        SetBoundary(BottomboundaryCollider, boundariesParent.transform,
            position: new Vector3(size / 2, 0, inwardOffset),
            size: new Vector3(size, boundaryHeight, boundaryThickness));

        SetBoundary(LeftboundaryCollider, boundariesParent.transform,
            position: new Vector3(inwardOffset, 0, size / 2),
            size: new Vector3(boundaryThickness, boundaryHeight, size));

        SetBoundary(RightboundaryCollider, boundariesParent.transform,
            position: new Vector3(size - inwardOffset, 0, size / 2),
            size: new Vector3(boundaryThickness, boundaryHeight, size));
    }

    private void SetBoundary(GameObject boundary, Transform parent, Vector3 position, Vector3 size)
    {
        if (boundary == null) return;

        // 设置父物体
        boundary.transform.SetParent(parent);
        
        // 设置位置和旋转
        boundary.transform.localPosition = position;
        boundary.transform.localRotation = Quaternion.identity;

        // 设置碰撞体
        var boxCollider = boundary.GetComponent<BoxCollider>();
        if (boxCollider == null) boxCollider = boundary.AddComponent<BoxCollider>();

        boxCollider.size = size;
        boxCollider.isTrigger = false;  // 设置为false，使其成为实体碰撞体

        // 添加刚体组件并设置为静态
        if (boundary.GetComponent<Rigidbody>() == null)
        {
            var rb = boundary.AddComponent<Rigidbody>();
            rb.isKinematic = true;  // 设置为运动学，这样就不会受物理影响
            rb.useGravity = false;  // 关闭重力
            rb.constraints = RigidbodyConstraints.FreezeAll;  // 冻结所有移动和旋转
        }
        else
        {
            var rb = boundary.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // 设置标签
        boundary.tag = "Collider";
    }

    public void Update()
    {
        // 在控制台输出当前偏移值（可选）
        // 重置偏移
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("我更新了 m_FollowOffset"+defaultOffset +" dampingX: "+this.dampingX+" dampingY: "+this.dampingY+" dampingZ: "+this.dampingZ);

            // 应用新偏移
            transposer.m_FollowOffset = defaultOffset;
            // 设置相机跟随平滑度
            transposer.m_XDamping = dampingX;
            transposer.m_YDamping = dampingY;
            transposer.m_ZDamping = dampingZ;
            Camerainit(200);
             
        }
        
        // // 调整相机跟随速度
        // if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        // {
        //     // 增加跟随速度
        //     damping = Mathf.Max(0.01f, damping - 0.05f);
        //     UpdateCameraDamping();
        //     Debug.Log($"相机跟随速度增加: {damping}");
        // }
        // else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        // {
        //     // 减少跟随速度
        //     damping = Mathf.Min(1f, damping + 0.05f);
        //     UpdateCameraDamping();
        //     Debug.Log($"相机跟随速度减少: {damping}");
        // }
    }
    
    // 更新相机跟随速度
    // private void UpdateCameraDamping()
    // {
    //     if (transposer != null)
    //     {
    //         transposer.m_XDamping = damping;
    //         transposer.m_YDamping = damping;
    //         transposer.m_ZDamping = damping;
    //     }
    // }
}