using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    private void Update()
    {

        int countAlive = 0;
        for (int i = 0; i < particleSystems.Count; i++)
        {
            
            if (particleSystems[i].IsAlive())
            {
                countAlive++;
            }
        }

        if (countAlive == 0)
        {
            Destroy(gameObject);
        }
    }
}
