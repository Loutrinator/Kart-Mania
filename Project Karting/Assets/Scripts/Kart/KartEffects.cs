using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kart
{
        public class KartEffects : MonoBehaviour
    {
        public Camera cam;
        [HideInInspector] public ShakeTransform cameraShakeTransform;
        public List<TrailRenderer> skidEmitters;
        public List<ParticleSystem> driftSmokeEmitters;


        public List<ParticleSystem> boostSparksEmitters;
        public ParticleSystem driftLoadingSparksEmitter;
        public Light boostLight;
        public AnimationCurve boostCameraEffect;
        public Material driftMaterial;
        private bool boostActivated;
        private float boostStrength;
        private float boostStartTime;
        private float boostLength;
        private bool isDrifting;


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
            baseFOV = cam.fieldOfView;
            baseZ = cam.transform.localPosition.z;
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
            //Debug.Log("currentIntensity = " + currentIntensity);
            //Debug.Log("driftLevel = " + driftLevel);
            #endif
            currentIntensity = Mathf.Lerp(currentIntensity, DriftSettings.instance.driftEffectIntensity[driftLevel], Time.fixedDeltaTime * DriftSettings.instance.boostSwitchSpeed);
            if (Mathf.Abs(currentIntensity - DriftSettings.instance.driftEffectIntensity[driftLevel]) > 0.1f)
            {
                boostLight.intensity = currentIntensity;
            }
            else
            {
                boostLight.intensity = DriftSettings.instance.driftEffectIntensity[driftLevel];
            }

            if (boostActivated)
            {
                float elapsed = Time.time - boostStartTime;
                if (elapsed < boostLength)
                {
                    float boostEffect =  boostCameraEffect.Evaluate(elapsed /boostLength)*boostStrength;
                    cam.fieldOfView = baseFOV + boostEffect * DriftSettings.instance.boostFOVOffset;
                    Vector3 previousPos = cam.transform.localPosition;
                    //cam.transform.localPosition =  new Vector3(previousPos.x,previousPos.y, baseZ - boostEffect * DriftSettings.instance.boostZOffset);
            
                }else
                {
                    stopBoost();
                }
            }
        
        }

        private IEnumerator BoostLoading()
        {
            WaitForSeconds wait = new WaitForSeconds(DriftSettings.instance.boostTimeToSwitch);
            var emissionModule = driftLoadingSparksEmitter.emission;
            var mainModule = driftLoadingSparksEmitter.main;
            yield return wait;
            if (isDrifting)
            {
                driftLoadingSparksEmitter.Play();
                emissionModule.rateOverTime = 75;
                driftLevel = 1;
                driftMaterial.SetInt("Drift_Mode",driftLevel);
                boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
                yield return wait;
            }
            if (isDrifting)
            {
                driftLevel = 2;
                emissionModule.rateOverTime = 250;
                mainModule.simulationSpeed = 1.5f;
                driftMaterial.SetInt("Drift_Mode",driftLevel);
                boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
                yield return wait;
            }
            if (isDrifting)
            {
                driftLevel = 3;
                mainModule.simulationSpeed = 2f;
                emissionModule.rateOverTime = 500;
                driftMaterial.SetInt("Drift_Mode",driftLevel);
                boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
            }
        }
        public void startDrift()
        {
            isDrifting = true;
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
            driftLoadingSparksEmitter.Stop();
            isDrifting = false;
            driftMaterial.SetInt("Drift_Mode",0);
            foreach (var skid in skidEmitters)
            {
                skid.emitting = false;
            }

            foreach(var smoke in driftSmokeEmitters)
            {
                smoke.Stop();
            }
            boostLight.color = DriftSettings.instance.driftEffectColors[0];
            currentIntensity = DriftSettings.instance.driftEffectIntensity[0];
            boostLight.gameObject.SetActive(true);
            if (driftLevel > 0)
            {
                startBoost(KartPhysicsSettings.instance.boostLength*(driftLevel/3f), KartPhysicsSettings.instance.boostStrength);
            }
            driftLevel = 0;
        }

        public void startBoost(float length, float force)
        {
            switch (driftLevel)
            {
                case 1:
                    cameraShakeTransform.AddShakeEvent(DriftSettings.instance.boost1Shake);
                    break;
                case 2:
                    cameraShakeTransform.AddShakeEvent(DriftSettings.instance.boost2Shake);
                    break;
                case 3:
                    cameraShakeTransform.AddShakeEvent(DriftSettings.instance.boost3Shake);
                    break;
            }
            
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
    }
}