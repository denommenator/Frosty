using System.Collections.Generic;
using UnityEngine;

namespace Frosty
{
    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;
        public Color Color;
    }
    
    public class SceneBuilder
    {
        public static List<Particle> MakeParticleSphere(Vector2 center, Vector2 velocity_com, float angular_speed, float radius,
            float spacing, Color color)
        {
            List<Particle> ret = new List<Particle>();
        
            Vector2 lower_corner = center - radius * Vector2.one;
            for (int i = 0; spacing * i < 2 * radius; i++)
            {
                for (int j = 0; spacing * j < 2 * radius; j++)
                {
                    Vector2 offset = new Vector2(i * spacing, j * spacing);
                    if ((offset - radius * Vector2.one).magnitude > radius)
                        continue;
                    ret.Add(new Particle()
                    {
                        Position = lower_corner + offset,
                        Velocity = velocity_com,
                        Radius = spacing,
                        Color = color
                    });
                }
            }

            return ret;
        }
        
    }
}