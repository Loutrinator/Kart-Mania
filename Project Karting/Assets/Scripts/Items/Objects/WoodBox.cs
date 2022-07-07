using System;
using Kart;
using UnityEngine;

namespace Items
{
    public class WoodBox : ItemObject
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
        /*private void OnTriggerEnter(Collider other)
        {
            //if (other.transform.name.Contains("kart"))
            //{
                //KartBase kart = other.GetComponent<KartCollisions>().kartBase;
                //kart.AddPowerup(_powerup);
                Debug.Log("Collision Caisse : " + other.name);
                //Destroy(gameObject);
            //}
        }*/

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<KartBase>())
            {
                KartBase kart = other.GetComponentInParent<KartBase>();
                kart.Damaged();
                Destroy(gameObject);
            }
        }

        public override void OnKeyDown(PlayerRaceInfo info) {
            transform.parent = null;
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            // Calling this will change info.ItemIsUsing and invoke info.onItemUsed
            // who call wb.Throw ( cf line 22 ). 
            Transform transform = info.kart.transform;
            //WoodBox wb = Instantiate(prefab, transform.position - transform.forward*distanceFromKartBack, Quaternion.identity, transform);
            Use(info); 
        }

        public override void ResetItem()
        {
            return;
        }

        public override void OnKeyHold(PlayerRaceInfo info)
        {
            //if(info.ItemIsInUse) return;
            //info.onItemUsed += wb.Throw;
            //info.ItemIsInUse = true;
        }
    }
}