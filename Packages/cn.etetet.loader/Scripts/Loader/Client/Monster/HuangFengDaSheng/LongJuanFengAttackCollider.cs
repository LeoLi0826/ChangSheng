
using System;
using UnityEngine;

namespace ET.Client
{
    public class LongJuanFengAttackCollider : MonoBehaviour
    {
        public float tickInterval = 0.5f;
        public int defaultDamage = 8;

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