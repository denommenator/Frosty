using System.Collections.Generic;
using UnityEngine;

namespace Frosty
{
public class BallRenderer
{
    public GameObject[] Balls;
    public BallRenderer(List<Particle> particles)
    {
        Balls = new GameObject[particles.Count];
        //DeformationLines = new GameObject[numParticles];
        float ballShrinkage = 0.5f;
        for (int i = 0; i < Balls.Length; i++)
        {
            Balls[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Balls[i].transform.localScale = ballShrinkage * particles[i].Radius * Vector3.one;
            Balls[i].GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            Balls[i].GetComponent<MeshRenderer>().material.color = particles[i].Color;
        }
    }

    public void UpdateBallPositions(Vector3[] particlePositions)
    {
        for (int pid = 0; pid < Balls.Length; pid++)
        {
            Balls[pid].transform.position = particlePositions[pid];
        }
    }
}
}