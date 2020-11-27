using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kart
{
        public class KartEffects : MonoBehaviour
    {
        public Camera camera;
        public List<TrailRenderer> skidEmitters;
        public List<ParticleSystem> driftSmokeEmitters;


        public List<ParticleSystem> boostSparksEmitters;
        public Light boostLight;
        public Color[] boostColors;
        [SerializeField] private float[] boostIntensity;
        public float boostSwitchSpeed;
        public float boostTimeToSwitch;
        public AnimationCurve boostCameraEffect;
        public float boostFOVOffset;
        public float boostZOffset;
        private bool boostActivated;
        private float boostStrength;
        private float boostLength;
        private float boostStartTime;


        private float currentIntensity;
        private float baseFOV;
        private float baseZ;

        //BoostEffect

        [HideInInspector] public int driftLevel;
        public void Start()
        {
            stopBoost();
            stopDrift();
            driftLevel = 0;
            baseFOV = camera.fieldOfView;
            baseZ = camera.transform.localPosition.z;
        }
        /*
        public void LateUpdate()
        {
            //Debug.Log("currentIntensity = " + currentIntensity);
            //Debug.Log("driftLevel = " + driftLevel);
            currentIntensity = Mathf.Lerp(currentIntensity, boostIntensity[driftLevel],
                Time.fixedDeltaTime * boostSwitchSpeed);
            if (Mathf.Abs(currentIntensity - boostIntensity[driftLevel]) > 0.1f)
            {
                stopBoost();
                stopDrift();
                driftLevel = 0;
                baseFOV = camera.fieldOfView;
                baseZ = camera.transform.localPosition.z;
            }
        }*/

        public void LateUpdate()
        {
            #if UNITY_EDITOR
            Debug.Log("currentIntensity = " + currentIntensity);
            Debug.Log("driftLevel = " + driftLevel);
            #endif
            currentIntensity = Mathf.Lerp(currentIntensity, boostIntensity[driftLevel], Time.fixedDeltaTime * boostSwitchSpeed);
            if (Mathf.Abs(currentIntensity - boostIntensity[driftLevel]) > 0.1f)
            {
                boostLight.intensity = currentIntensity;
            }
            else
            {
                boostLight.intensity = boostIntensity[driftLevel];
            }

            if (boostActivated)
            {
                float elapsed = Time.time - boostStartTime;
                if (elapsed < boostLength)
                {
                    float boostEffect =  boostCameraEffect.Evaluate(elapsed /boostLength)*boostStrength;
                    camera.fieldOfView = baseFOV + boostEffect * boostFOVOffset;
                    Vector3 previousPos = camera.transform.localPosition;
                    camera.transform.localPosition =  new Vector3(previousPos.x,previousPos.y, baseZ - boostEffect * boostZOffset);
            
                }else
                {
                    stopBoost();
                }
            }
        
        }

        private IEnumerator BoostLoading()
        {
            WaitForSeconds wait = new WaitForSeconds(boostTimeToSwitch);
            yield return wait;
            driftLevel = 1;
            boostLight.color = boostColors[driftLevel];
            yield return wait;
            driftLevel = 2;
            boostLight.color = boostColors[driftLevel];
            yield return wait;
            driftLevel = 3;
            boostLight.color = boostColors[driftLevel];
        }
        public void startDrift()
        {
            foreach(var skid in skidEmitters)
            {
                skid.emitting = true;
            }
            foreach(var smoke in driftSmokeEmitters)
            {
                smoke.Play();
            }
            driftLevel = 0;
            boostLight.gameObject.SetActive(true);
            StartCoroutine(BoostLoading());
        }

        public void stopDrift()
        {
            foreach (var skid in skidEmitters)
            {
                skid.emitting = false;
            }

            foreach(var smoke in driftSmokeEmitters)
            {
                smoke.Stop();
            }
            boostLight.color = boostColors[0];
            currentIntensity = boostIntensity[0];
            boostLight.gameObject.SetActive(true);
            driftLevel = 0;
        }

        public void startBoost(float length, float force)
        {
            foreach(var spark in boostSparksEmitters)
            {
                spark.Play();
            }

            boostLength = length;
            boostStrength = force;
            boostStartTime = Time.time;
            boostActivated = true;
        }
        public void stopBoost()
        {
            foreach(var spark in boostSparksEmitters)
            {
                spark.Stop();
            }

            boostActivated = false;
        }

        public void startDriftLoading(int level)
        {
        
        }
    }
}