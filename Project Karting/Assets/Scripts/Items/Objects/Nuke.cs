using System.Collections;
using DG.Tweening;
using Handlers;
using Kart;
using UnityEngine;
namespace Items
{
    public class Nuke : MonoBehaviour{ 
        public GameObject explosion;
        public float timeBeforeLaunch = 1f;
        public float travelDuration = 0.5f;
        public Vector3 startPosition;
        public Transform target;
        public AnimationCurve trajectory;
        [HideInInspector] public ShakeTransform cameraShakeTransform;
        public ShakeTransformEventData nukeShake;
        public AudioSource flightEffect;
        public AudioSource alarmSound;
        

        private float startLauchTime;
        private float startAnimationTime;
        private bool launched;
        private bool exploded;
        private bool targetFound;
        private float animationPercent;
        private void Start()
        {
            startAnimationTime = Time.time;
            StartCoroutine(Alarm());
        }

        private void Update()
        {
            if (!launched)
            {
                float elapsed = Time.time - startAnimationTime;
                if (elapsed >= timeBeforeLaunch)
                {
                    Launch();
                }
            }
            else if(!exploded)
            {
                float elapsed = Time.time - startLauchTime;
                float percent = elapsed / travelDuration;
                Vector3 targetPos = targetFound ? target.position : Vector3.zero;
                float x = Mathf.Lerp(startPosition.x, targetPos.x, percent);
                float y = startPosition.y*(1-animationPercent) + targetPos.y * animationPercent;
                float z = Mathf.Lerp(startPosition.z, targetPos.z, percent);
                Vector3 oldPos = transform.position;
                Vector3 newPos = new Vector3(x, y, z);
                transform.position = newPos;
                transform.forward = (newPos - oldPos).normalized;
                if (percent >= 1f)
                {
                    Explode();
                }
            }
        }

        private IEnumerator Alarm()
        {
            alarmSound.Play();
            yield return new WaitForSeconds(2f);
            alarmSound.Stop();
            alarmSound.Play();
        }

        private void Launch()
        {
            launched = true;
            startLauchTime = Time.time;
            transform.position = startPosition;
            //flightEffect.Play();
            targetFound = target != null;
            animationPercent = 0;
            DOTween.To(() => animationPercent, x => animationPercent = x, 1, travelDuration).SetEase(Ease.InBack);
        }

        private void Explode()
        {
            exploded = true;
            KartBase kart = target.GetComponent<KartBase>();
            Quaternion rotation = kart != null ? kart.closestBezierPos.Rotation : Quaternion.identity;
            Vector3 targetPos = targetFound ? target.position : Vector3.zero;
            Instantiate(explosion, targetPos, rotation);
            //faire un spherecast
            GameManager.Instance.ShakeCameras(nukeShake);
            Destroy(gameObject);
        }
    }
}