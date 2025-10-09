// #if UNITY_EDITOR
// using System;
// using Sirenix.OdinInspector;
// using Sirenix.Serialization;
// using UnityEngine;
//
// namespace ET.Client
// {
//     [CreateAssetMenu(menuName = "ET/AIScriptableObject")]
//     [EnableClass]
//     public class AIScriptableObject : ScriptableObject,IBehaviourTreeEditable
//     {
//         [Title("AI配置", TitleAlignment = TitleAlignments.Centered)]
//         [NonSerialized, OdinSerialize]
//         [HideLabel]
//         [HideReferenceObjectPicker]
//         public AIConfig AIConfig;
//
//         [Title("行为树数据", TitleAlignment = TitleAlignments.Centered)]
//         [NonSerialized, OdinSerialize]
//         [HideLabel]
//         [HideReferenceObjectPicker]
//         public BehaviourTreeData BehaviourTreeData = new();
//
//         public AIConfig Export()
//         {
//             // 在导出前确保行为树的序列化预处理已完成
//             if (BehaviourTreeData != null && BehaviourTreeData.RootNode != null)
//             {
//                 // 先进行反序列化后处理，重建节点引用关系
//                 BehaviourTreeData.DeserializePostProcess();
//                 // 再进行序列化预处理，确保ID和关系正确
//                 BehaviourTreeData.SerializePreProcess();
//                 // this.AIConfig.Root = BehaviourTreeData.RootNode;
//             }
//             return this.AIConfig;
//         }
//
//         /// <summary>
//         /// 获取行为树数据（用于编辑器）
//         /// </summary>
//         public BehaviourTreeData GetBehaviourTreeData()
//         {
//             if (BehaviourTreeData == null)
//             {
//                 BehaviourTreeData = BehaviourTreeData.CreateDefault();
//             }
//             else
//             {
//                 // 确保从序列化数据加载时正确重建 Children 关系
//                 BehaviourTreeData.DeserializePostProcess();
//             }
//             return BehaviourTreeData;
//         }
//
//         public string GetEditorWindowTitle()
//         { 
//             return "行为树窗口 - AI";
//         }
//     }
// }
// #endif