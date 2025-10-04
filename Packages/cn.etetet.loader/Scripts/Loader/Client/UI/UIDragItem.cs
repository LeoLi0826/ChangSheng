using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ET.Client
{
    public struct UIItemDragEnd
    {
        public EntityRef<Entity> Current;
        public EntityRef<Entity> Target;
    }

    [RequireComponent(typeof(CanvasGroup))] // 确保总是有CanvasGroup组件
    public class UIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private EntityRef<Entity> entity;

        public Entity Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.entity = value;
            }
        }


        public bool CanDrag = true; // 控制是否可以被拖动
        public bool CanReceive = true; // 控制是否可以接收其他拖拽物品
        public Image DragImage;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Transform originalParent;
        private Vector3 originalPosition;
        public Canvas Canvas;

        // 用于记录DragImage的原始状态
        private Transform dragImageOriginalParent;
        private Vector3 dragImageOriginalLocalPosition;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanDrag || Entity == null || this.Canvas == null) return;

            if (DragImage != null && DragImage.gameObject.activeInHierarchy)
            {
                // 记录DragImage的原始状态
                dragImageOriginalParent = DragImage.transform.parent;
                dragImageOriginalLocalPosition = DragImage.transform.localPosition;

                // 将DragImage提升到顶层Canvas，使其脱离原布局并置于顶层
                DragImage.transform.SetParent(this.Canvas.transform, true);
                DragImage.transform.SetAsLastSibling();

                // 关闭DragImage的射线检测，防止其阻挡目标
                DragImage.raycastTarget = false;
            }
            else // 回退方案：如果DragImage未指定或不可见，则拖动整个UI项
            {
                originalParent = transform.parent;
                originalPosition = transform.position;
                transform.SetParent(this.Canvas.transform, true);
                transform.SetAsLastSibling();
            }

            // 禁用射线阻挡，以便检测下方的物体
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag || Entity == null || this.Canvas == null) return;

            if (DragImage != null && dragImageOriginalParent != null) // dragImageOriginalParent不为null表示是DragImage在移动
            {
                // 移动DragImage
                DragImage.rectTransform.anchoredPosition += eventData.delta / this.Canvas.scaleFactor;
            }
            else
            {
                // 移动自身
                rectTransform.anchoredPosition += eventData.delta / this.Canvas.scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanDrag || Entity == null) return;

            // 恢复射线检测
            canvasGroup.blocksRaycasts = true;

            GameObject pointerEnter = eventData.pointerEnter;

            // 检查是否放置在了有效的UIDragItem上
            if (pointerEnter != null)
            {
                UIDragItem targetDragItem = pointerEnter.GetComponent<UIDragItem>();
                if (targetDragItem != null && targetDragItem.Entity != null && targetDragItem != this && targetDragItem.CanReceive)
                {
                    // 触发拖拽结束事件，让逻辑层处理后续
                    DragEnd(targetDragItem.Entity);
                }
            }

            // --- 恢复UI状态 ---
            if (DragImage != null && dragImageOriginalParent != null)
            {
                // 将DragImage恢复到其原始父节点和位置
                DragImage.transform.SetParent(dragImageOriginalParent, true);
                DragImage.transform.localPosition = dragImageOriginalLocalPosition;

                // 恢复DragImage的射线检测
                DragImage.raycastTarget = true;

                dragImageOriginalParent = null; // 重置状态
            }
            else if (originalParent != null)
            {
                // 如果是整个UI项在移动，则将其恢复
                transform.SetParent(originalParent, true);
                transform.position = originalPosition;
                originalParent = null; // 重置状态
            }
        }

        /// <summary>
        /// 拖拽结束后执行，拖拽判断为成功的依据是 对方持有该组件，且不为自己，这个是要挂在具体的跟随拖拽的UI上的
        /// </summary>
        /// <param name="target"></param>
        private void DragEnd(Entity target)
        {
            if (this.Entity != null)
            {
                EventSystem.Instance.Publish(Entity.Scene(), new UIItemDragEnd()
                {
                    Current = entity,
                    Target = target
                });
            }
        }

        
    }
}

