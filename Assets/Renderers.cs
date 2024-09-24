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
    public List<ComputeBuffer> BatchColors;
    public Mesh ParticleMesh;
    public List<Material> ParticleMaterials;
    public BallRenderer(List<Particle> particles)
    {
        BatchMatrices = new List<Matrix4x4[]>();
        int divisor = particles.Count / 1000;
        int remainder = particles.Count - 1000 * divisor;
        List<float[]> colorData = new List<float[]>();
        BatchColors = new List<ComputeBuffer>();
        ParticleMaterials = new List<Material>();
        Material material = Resources.Load<Material>("Material/ParticleMaterial");
        for (int i = 0; i < divisor; i++)
        {
            BatchMatrices.Add(new Matrix4x4[1000]);
            colorData.Add(new float[1000 * 4]);
            BatchColors.Add(new ComputeBuffer(1000, 4 * sizeof(float)));
            ParticleMaterials.Add(new Material(material));
        }

        if (remainder > 0)
        {
            BatchMatrices.Add(new Matrix4x4[remainder]);
            colorData.Add(new float[remainder * 4]);
            BatchColors.Add(new ComputeBuffer(remainder, 4 * sizeof(float)));
            ParticleMaterials.Add(new Material(material));
        }
        
        for (int i = 0; i < particles.Count; ++i)
        {
            int divisor_i = i / 1000;
            int remainder_i = i - 1000 * divisor_i;
            
            colorData[divisor_i][4 * remainder_i + 0] = particles[i].Color.r;
            colorData[divisor_i][4 * remainder_i + 1] = particles[i].Color.g;
            colorData[divisor_i][4 * remainder_i + 2] = particles[i].Color.b;
            colorData[divisor_i][4 * remainder_i + 3] = particles[i].Color.a;
        }

        for (int i = 0; i < colorData.Count; i++)
        {
            BatchColors[i].SetData(colorData[i]);
            ParticleMaterials[i].enableInstancing = true;
            
        }

        Particles = particles;
        
        GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ParticleMesh = particle.GetComponent<MeshFilter>().mesh;
        GameObject.Destroy(particle);
        //ParticleMaterial = Resources.Load<Material>("Material/ParticleMaterial"); //new Material(Shader.Find("Standard"));//particle.GetComponent<MeshRenderer>().material;
        //ParticleMaterial.enableInstancing = true;
        
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

        for(int batch = 0; batch < BatchMatrices.Count; ++batch)
        {
            Matrix4x4[] Matrices = BatchMatrices[batch];
            ParticleMaterials[batch].SetBuffer("_Colors", BatchColors[batch]);
            Graphics.DrawMeshInstanced(
                ParticleMesh,
                0,
                ParticleMaterials[batch],
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