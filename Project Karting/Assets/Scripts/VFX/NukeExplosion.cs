using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeExplosion : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public Transform shockwave;
    public float shockwaveSpeed;

    private void Start()
    {
        shockwave.localScale = Vector3.zero;
    }

    private void Update()
    {
        shockwave.localScale += Time.deltaTime * shockwaveSpeed * Vector3.one;
        foreach (var ps in particleSystems)
        {
            if (!ps.IsAlive())
            {
                Destroy(ps.gameObject);
                particleSystems.Remove(ps);
            }
        }

        if (particleSystems.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
