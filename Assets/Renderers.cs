using UnityEngine;

namespace Frosty
{
public class BallRenderer
{
    public GameObject[] Balls;
    public BallRenderer(int particleCount)
    {
        Balls = new GameObject[particleCount];
        //DeformationLines = new GameObject[numParticles];

        for (int i = 0; i < Balls.Length; i++)
        {
            Balls[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Balls[i].transform.localScale = 5 * Vector3.one;
            Balls[i].GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            Balls[i].GetComponent<MeshRenderer>().material.color = Color.red;
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