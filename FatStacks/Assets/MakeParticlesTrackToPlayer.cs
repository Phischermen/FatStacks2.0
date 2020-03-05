using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeParticlesTrackToPlayer : MonoBehaviour
{
    public ParticleSystem p;
    public ParticleSystem.Particle[] particles;
    public Transform target;
    public float force;
    //public float affectDistance;
    //float sqrDist;
    //Transform thisTransform;


    void Start()
    {
        p = GetComponent<ParticleSystem>();
        target = Player.singleton.transform;
        //sqrDist = affectDistance * affectDistance;
    }


    void Update()
    {
        if (target == null)
        {
            Destroy(this);
            return;
        }
        particles = new ParticleSystem.Particle[p.particleCount];

        p.GetParticles(particles);

        for (int i = 0; i < particles.GetUpperBound(0); i++)
        {
            Vector3 v = target.position - particles[i].position;
            //v = new Vector3(v.x, v.z, v.y);
            float ForceToAdd = particles[i].remainingLifetime * (force * v.magnitude);

            Debug.DrawRay (particles [i].position, v.normalized);

            particles[i].velocity = v.normalized * ForceToAdd;

            //particles [i].position = Vector3.Lerp (particles [i].position, Target.position, Time.deltaTime / 2.0f);

        }

        p.SetParticles(particles, particles.Length);
    }
}
