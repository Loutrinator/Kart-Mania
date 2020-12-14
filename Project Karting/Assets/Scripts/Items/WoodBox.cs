using System;
using Kart;
using UnityEngine;

namespace Items
{
    public class WoodBox : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float spawnAnimationDuration = .5f;
        [SerializeField] private AnimationCurve spawnAnimationCurve = null;
        [SerializeField] private AnimationCurve throwAnimationCurve = null;
        private float _elapsedTime;
        [HideInInspector] float _startTime;
        private float _step;
        public void Throw(bool isHoldingKey)
        {
            if (!isHoldingKey) transform.SetParent(null);
        }

        private void Start()
        {
            _startTime = Time.time;
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            SpawnAnimation();
        }

        private void SpawnAnimation()
        {
            _elapsedTime = Time.time - _startTime;
            _step = _elapsedTime / spawnAnimationDuration;
            transform.localScale = spawnAnimationCurve.Evaluate(_step) * Vector3.one;
        }

        private void ThrowAnimation()
        {
            
        }

        private void BrokeAnimation(float startTime)
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            KartBase kart = other.GetComponent<KartCollider>()?.kartBase;
            if (kart != null)
            {
                //TODO : trigger kart crash animation
                Destroy(gameObject);
            }
        }
    }
}