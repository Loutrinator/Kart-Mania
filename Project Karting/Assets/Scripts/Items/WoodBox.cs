using System;
using Kart;
using UnityEngine;

namespace Items
{
    public class WoodBox : MonoBehaviour
    {
        [Header("Effect")] 
        [SerializeField] private Stats effect;
        [SerializeField] private float effectDuration;
        private StatPowerup _powerup;
        
        [Header("Animation")]
        [SerializeField] private float throwingDistance = .3f;
        [SerializeField] private float spawnAnimationDuration = .5f;
        [SerializeField] private float throwAnimationDuration = .5f;
        [SerializeField] private AnimationCurve spawnAnimationCurve = null;
        [SerializeField] private AnimationCurve throwAnimationCurve = null;
        
        private float _startTime;
        private float _throwStartTime;
        private bool _isThrowing;
        private Transform _transform;
        private Vector3 _startPosition;
        private Vector3 _forward;
        public void Throw(bool isHoldingKey)
        {
            if (!isHoldingKey)
            {
                _forward = _transform.parent.forward * throwingDistance;
                _transform.SetParent(null);
                _throwStartTime = Time.time;
                _startPosition = _transform.position;
                _isThrowing = true;
            }
        }

        private void Start()
        {
            _isThrowing = false;
            _startTime = Time.time;
            _transform = transform;
            _transform.localScale = Vector3.zero;
            _powerup = new StatPowerup(effect,effectDuration);

        }

        private void Update()
        {
            SpawnAnimation();
            if(_isThrowing) ThrowAnimation();
        }

        private void SpawnAnimation()
        { 
            float elapsedTime = Time.time - _startTime;
            _transform.localScale = spawnAnimationCurve.Evaluate(elapsedTime / spawnAnimationDuration) * Vector3.one;
        }

        private void ThrowAnimation()
        {
            float elapsedTime = Time.time - _throwStartTime;
            float step = elapsedTime / throwAnimationDuration;
            if (step > 1) _isThrowing = false;            
            float yCurve = throwAnimationCurve.Evaluate(step);
            float y = _startPosition.y + yCurve;
            var position = _transform.position;
            position = new Vector3(position.x,y,position.z) - _forward;
            _transform.position = position;
        }

        private void BrokeAnimation(float startTime)
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            KartBase kart = other.GetComponent<KartCollider>()?.kartBase;
            if (kart != null)
            {
                kart.addPowerup(_powerup);
                Destroy(gameObject);
            }
        }
    }
}