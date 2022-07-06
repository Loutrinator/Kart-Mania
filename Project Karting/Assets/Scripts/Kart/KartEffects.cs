using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
    public class KartEffects : MonoBehaviour
    {
        public KartBase kart;
        public Camera cam;
        public CameraFollowPlayer cameraFollowPlayer;
        public List<TrailRenderer> skidEmitters;
        public List<ParticleSystem> driftSmokeEmitters;
        

        public List<ParticleSystem> boostSparksEmitters;
        public ParticleSystem driftLoadingSparksEmitter;
        public Light boostLight;
        public Material driftMaterial;
        [SerializeField] private Keyhole _keyhole;

        public GameObject explosionMotorEffect;
        private bool boostActivated;
        private float boostStrength;
        private float boostStartTime;
        private float boostLength;
        private float boostCoeff;
        private bool isDrifting;
        private bool FOVFadedIn;

        private float currentIntensity;
        private float baseFOV;
        private float baseZ;

        private float timeWhenKeyInserted;
        private bool keyIsRewinding;
        private bool keyIsInserted;

        private AudioSource _driftSoundSource;
        private float startTime = 0;

        //BoostEffect
        [HideInInspector] public int driftLevel;

        public void Start()
        {
            stopBoost();
            StopDrift();
            driftLevel = 0;
            baseFOV = cam.fieldOfView;
            baseZ = cam.transform.localPosition.z;
            _driftSoundSource = gameObject.AddComponent<AudioSource>();
            _driftSoundSource.volume = 0f;
            _driftSoundSource.loop = true;
            _driftSoundSource.playOnAwake = false;
            _driftSoundSource.clip = DriftSettings.instance.driftAudioClip;
            
        }

        public void Update()
        {
            if (isDrifting)
            {
                float elapsed = Time.time - startTime;
                
                var emissionModule = driftLoadingSparksEmitter.emission;
                var mainModule = driftLoadingSparksEmitter.main;
                
                if (elapsed > DriftSettings.instance.boostTimeToSwitch && driftLevel < 1)
                {
                    driftLoadingSparksEmitter.Play();
                    emissionModule.rateOverTime = 75;
                    driftLevel = 1;
                    driftMaterial.SetInt("Drift_Mode", driftLevel);
                    boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
                }else if (elapsed > DriftSettings.instance.boostTimeToSwitch*2 && driftLevel < 2)
                {
                    driftLevel = 2;
                    emissionModule.rateOverTime = 250;
                    mainModule.simulationSpeed = 1.5f;
                    driftMaterial.SetInt("Drift_Mode", driftLevel);
                    boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
                }else if (elapsed > DriftSettings.instance.boostTimeToSwitch*3 && driftLevel < 3)
                {
                    driftLevel = 3;
                    mainModule.simulationSpeed = 2f;
                    emissionModule.rateOverTime = 500;
                    driftMaterial.SetInt("Drift_Mode", driftLevel);
                    boostLight.color = DriftSettings.instance.driftEffectColors[driftLevel];
                }
                
            }
        }
        
        public void LateUpdate()
        {
            //drift light intensity
            currentIntensity = Mathf.Lerp(currentIntensity, DriftSettings.instance.driftEffectIntensity[driftLevel],
                Time.fixedDeltaTime * DriftSettings.instance.boostSwitchSpeed);
            if (Mathf.Abs(currentIntensity - DriftSettings.instance.driftEffectIntensity[driftLevel]) > 0.1f)
            {
                boostLight.intensity = currentIntensity;
            }
            else
            {
                boostLight.intensity = DriftSettings.instance.driftEffectIntensity[driftLevel];
            }

            //drift released, the boosts activates 
            if (boostActivated)
            {
                float elapsed = Time.time - boostStartTime;
                if (elapsed < boostLength)
                {
                    float boostEffect = 0;
                    if (!FOVFadedIn)
                    {
                        float fadePercent = elapsed / DriftSettings.instance.transitionSpeed;
                        boostEffect = DriftSettings.instance.boostCameraIn.Evaluate(fadePercent);
                        if (fadePercent >= 1)
                        {
                            FOVFadedIn = true;
                        }
                    }
                    else
                    {
                        float fadePercent = (elapsed - (boostLength - DriftSettings.instance.transitionSpeed)) /
                                            DriftSettings.instance.transitionSpeed;
                        fadePercent = fadePercent > 0 ? fadePercent : 0;
                        boostEffect = DriftSettings.instance.boostCameraOut.Evaluate(fadePercent);
                    }

                    // baseFOV : the FOV at the start
                    // boostEffect : the coefficient given by the animation curves for the transition
                    // boostStrength: a general coefficient to increase the strength of the effect
                    // boostCoeff : a value scaled based on the drift level
                    cam.fieldOfView = baseFOV + boostEffect * boostStrength * DriftSettings.instance.boostFOVOffset * boostCoeff;
                    Vector3 previousPos = cam.transform.localPosition;
                    if (cameraFollowPlayer.currentCameraMode == CameraMode.front)
                        previousPos.z = baseZ - boostEffect * boostCoeff * DriftSettings.instance.boostZOffset;
                    cam.transform.localPosition = previousPos;

                }
                else
                {
                    stopBoost();
                }
            }

        }


        public void StartDrift()
        {
            isDrifting = true;
            startTime = Time.time;
            
            foreach (var skid in skidEmitters)
            {
                skid.emitting = true;
            }

            foreach (var smoke in driftSmokeEmitters)
            {
                smoke.Play();
            }
            driftLevel = 0;
            boostLight.gameObject.SetActive(true);
            PlayBoostSound();
            float low = DriftSettings.instance.driftLowVibration;
            float high = DriftSettings.instance.driftHighVibration;
                            
            kart.rumbler.RumbleHold(low,high);
        }

        public void StopDrift()
        {
            if(kart.rumbler != null && kart.rumbler.activeRumblePattern == RumblePattern.Hold)
                kart.rumbler.StopRumble();
            driftLoadingSparksEmitter.Stop();
            isDrifting = false;
            driftMaterial.SetInt("Drift_Mode", 0);
            foreach (var skid in skidEmitters)
            {
                skid.emitting = false;
            }

            foreach (var smoke in driftSmokeEmitters)
            {
                smoke.Stop();
            }

            boostLight.color = DriftSettings.instance.driftEffectColors[0];
            currentIntensity = DriftSettings.instance.driftEffectIntensity[0];
            boostLight.gameObject.SetActive(true);
            if (driftLevel > 0)
            {
                float boostDuration = 0;
                switch (driftLevel)
                {
                    case 1:
                        boostDuration = DriftSettings.instance.boostDuration[0];
                        break;
                    case 2:
                        boostDuration = DriftSettings.instance.boostDuration[1];
                        break;
                    case 3:
                        boostDuration = DriftSettings.instance.boostDuration[2];
                        break;
                }

                startBoost(boostDuration, KartPhysicsSettings.instance.boostStrength);
            }

            driftLevel = 0;

            StopBoostSound();
        }


        private void PlayBoostSound()
        {
            if (_driftSoundSource != null)
            {
                _driftSoundSource.Play();
                DOTween.To(() => _driftSoundSource.volume, value => _driftSoundSource.volume = value,
                        AudioSettings.instance.driftVolume, DriftSettings.instance.driftSoundAnimationSpeed)
                    .SetEase(DriftSettings.instance.driftVolumeEaseIn);
                _driftSoundSource.pitch = 1;
                DOTween.To(() => _driftSoundSource.pitch, value => _driftSoundSource.pitch = value,
                        DriftSettings.instance.driftPitchMax, DriftSettings.instance.driftSoundAnimationSpeed)
                    .SetEase(DriftSettings.instance.driftPitchEaseIn);
            }
        }

        private void StopBoostSound()
        {
            if (_driftSoundSource != null)
            {
                _driftSoundSource.clip = DriftSettings.instance.driftAudioClip;
                DOTween.To(() => _driftSoundSource.volume, value => _driftSoundSource.volume = value, 0, DriftSettings.instance.driftSoundAnimationSpeed).SetEase(DriftSettings.instance.driftVolumeEaseOut);
                DOTween.To(() => _driftSoundSource.pitch, value => _driftSoundSource.pitch = value,
                        DriftSettings.instance.driftPitchMin, DriftSettings.instance.driftSoundAnimationSpeed)
                    .SetEase(DriftSettings.instance.driftPitchEaseOut);
            }
        }



        public void ApplyPowerup(StatPowerup powerup, ShakeTransformEventData shake,float length, float force)
        {
            kart.AddPowerup(powerup);
            cameraFollowPlayer.cameraShakeTransform.AddShakeEvent(shake);
            foreach (var spark in boostSparksEmitters)
            {
                spark.Play();
            }
            
            boostLength = length;
            boostStrength = force;
            boostStartTime = Time.time;
            boostActivated = true;
            FOVFadedIn = false;

            if (kart.rumbler != null)
            {
                float low = DriftSettings.instance.boostLowVibration * boostCoeff;
                float high = DriftSettings.instance.boostHighVibration * boostCoeff;
                            
                kart.rumbler.RumbleConstant(low,high, length);
            }
        }
        
        public void startBoost(float length, float force)
        {
            ShakeTransformEventData shake = DriftSettings.instance.boostShake[driftLevel - 1];
            boostCoeff = DriftSettings.instance.driftLevelCoeff[driftLevel-1];
            Stats statModifier = new Stats();
            statModifier.acceleration = 200000f;
            statModifier.topSpeed = DriftSettings.instance.boostBaseStrength * boostCoeff;
            float duration = DriftSettings.instance.boostDuration[driftLevel - 1];
            StatPowerup boost = new StatPowerup(statModifier, duration);
            ApplyPowerup(boost, shake,length,force);
        }

        public void stopBoost()
        {
            foreach (var spark in boostSparksEmitters)
            {
                spark.Stop();
            }

            boostActivated = false;
        }

        public void InsertKey()
        {
            keyIsInserted = true;
            _keyhole.InsertKey(Keyhole.RewindMode.startRewindMode, null);
            keyIsRewinding = false;
        }
        
        public void Rewind()
        {
            if (!keyIsRewinding)
            {
                timeWhenKeyInserted = Time.time;
                _keyhole.Rewind(null);
                keyIsRewinding = true;
            }
        }
        
        public void StopRewind()
        {
            if (!keyIsRewinding)
            {
                _keyhole.RemoveKey();
                return;
            }
            float now = Time.time;
            float duration = now - timeWhenKeyInserted;
            
            if (duration > 2)
            {
                ExplodeMotor();
                _keyhole.StopRewind(0);
            }
            else {
                
                _keyhole.StopRewind(duration);
                if(duration > 1.6)
                {
                    cameraFollowPlayer.cameraShakeTransform.AddShakeEvent(DriftSettings.instance.boostShake[1]);
                    Stats statModifier = new Stats();
                    statModifier.acceleration = 500f;
                    statModifier.topSpeed = 35;
                    StatPowerup boost = new StatPowerup(statModifier, duration);
                    kart.AddPowerup(boost);

                    foreach (var spark in boostSparksEmitters)
                    {
                        spark.Play();
                    }

                    boostLength = duration;
                    boostStrength = 35;
                    boostStartTime = Time.time;
                    boostActivated = true;
                    FOVFadedIn = false;
                }
            }
            keyIsRewinding = false;
            keyIsInserted = false;
        }

        private void ExplodeMotor()
        {
            Instantiate(explosionMotorEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity, transform);
            StartCoroutine(FreezeControl());
        }

        private IEnumerator FreezeControl()
        {
            kart.canMove = false;
            yield return new WaitForSeconds(2f);
            kart.canMove = true;
        }
    }
}