using System;
using UnityEngine;

namespace ET
{
    // 与 LongJuanFengAttackCollider 一致：仅做触发事件转发，不做任何业务判断
    public class MonsterAttackCollector : MonoBehaviour
    {
        public Action<Collider> onEnter;
        public Action<Collider> onStay;
        public Action<Collider> onExit;

        public void SetOnTriggerEnter(Action<Collider> action) => onEnter = action;
        public void SetOnTriggerStay(Action<Collider> action) => onStay = action;
        public void SetOnTriggerExit(Action<Collider> action) => onExit = action;

        private void OnTriggerEnter(Collider other) => onEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => onStay?.Invoke(other);
        private void OnTriggerExit(Collider other) => onExit?.Invoke(other);
    }
}