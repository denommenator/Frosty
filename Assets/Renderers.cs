using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Frosty
{
public class BallRenderer
{
    public GameObject[] Balls;
    public List<Particle> Particles;
    public List<Matrix4x4[]> BatchMatrices;
    public Mesh ParticleMesh;
    public Material ParticleMaterial;
    public BallRenderer(List<Particle> particles)
    {
        BatchMatrices = new List<Matrix4x4[]>();
        int divisor = particles.Count / 1000;
        int remainder = particles.Count - 1000 * divisor;
        for (int i = 0; i < divisor; i++)
        {
            BatchMatrices.Add(new Matrix4x4[1000]);
        }

        if (remainder > 0)
        {
            BatchMatrices.Add(new Matrix4x4[remainder]);
        }

        Particles = particles;
        
        GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ParticleMesh = particle.GetComponent<MeshFilter>().mesh;
        GameObject.Destroy(particle);
        ParticleMaterial = new Material(Shader.Find("Standard"));//particle.GetComponent<MeshRenderer>().material;
        ParticleMaterial.enableInstancing = true;
        /*
        Balls = new GameObject[particles.Count];
        //DeformationLines = new GameObject[numParticles];
        float ballShrinkage = 0.5f;
        GameObject particle = GameObject.Find("Particle");
        for (int i = 0; i < Balls.Length; i++)
        {
            Balls[i] = GameObject.Instantiate(particle);
            //Balls[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Balls[i].transform.localScale = ballShrinkage * particles[i].Radius * Vector3.one;
            //Balls[i].GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            Balls[i].GetComponent<MeshRenderer>().material.color = particles[i].Color;
        }
        */
    }

    public void UpdateBallPositions(Vector3[] particlePositions)
    {
        float ballShrinkage = 0.5f;
        
        for (int i = 0; i < particlePositions.Length; ++i)
        {
            int divisor = i / 1000;
            int remainder = i - 1000 * divisor;
            BatchMatrices[divisor][remainder] = Matrix4x4.TRS(particlePositions[i], Quaternion.identity, ballShrinkage * Particles[i].Radius * Vector3.one);
        }

        foreach (Matrix4x4[] Matrices in BatchMatrices)
        {
            Graphics.DrawMeshInstanced(
                ParticleMesh,
                0,
                ParticleMaterial,
                Matrices
            );
        }
        // for (int pid = 0; pid < Balls.Length; pid++)
        // {
        //     Balls[pid].transform.position = particlePositions[pid];
        // }
    }
}
}