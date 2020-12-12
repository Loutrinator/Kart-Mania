using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeExplosion : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public Transform shockwave;
    public float shockwaveSpeed;
    public AudioSource explosionEffect;

    private void Start()
    {
        shockwave.localScale = Vector3.zero;
    }

    private void Update()
    {
        shockwave.localScale += Time.deltaTime * shockwaveSpeed * Vector3.one;
        List<ParticleSystem> psToDelete = new List<ParticleSystem>();
       
        for (int i = 0; i < particleSystems.Count; i++)
        {
            
            if (!particleSystems[i].IsAlive())
            {
                psToDelete.Add(particleSystems[i]);//Impossible to delete an element of a List while iterating. need to do it outside of for
            }
        }

        foreach (var ps in psToDelete)
        {
            Destroy(ps.gameObject);
            particleSystems.Remove(ps);
        }

        if (particleSystems.Count == 0 && !explosionEffect.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
