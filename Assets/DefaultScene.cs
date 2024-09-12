using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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

    class SceneBuilder
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

    public class DefaultScene : MonoBehaviour
    {
        private ParticleController Controller;
        public double WallStiffness = 1000;
        public double MuConstitutive = 1000;
        public double LambdaConstitutive = 0.0;
        public double MuDamping = 1.0;
        public double Gravity = 981.0f;
        public int NumStepsPerFrame = 1;
        public int NumDescentSteps = 20;
        public int NumSecsToSimulate = 5;
        public float H = 5;
        public bool ContinueSimulation = true;
        public int NumFramesSimulated;
        public int CurrentFrameNumber;
        public bool LiveView = true;
        void Start()
        {
            List<Vector2> initial_position_input = new List<Vector2>();
            List<Vector2> initial_velocity_input = new List<Vector2>();
            /*
            
            for (int i = 0; i < 100; i++)
            {
                initial_position_input.Add(new Vector2(0, 5 * i));
            }
            
            for (int i = 0; i < 100; i++)
            {
                initial_velocity_input.Add(new Vector2(0.01f * i, 0));
            }
            */
            /*
            List<Vector2> initial_position_input = new List<Vector2>()
            {
                new Vector2(-30.0f, 5.0f),
                new Vector2(-35.0f, 5.0f),
                new Vector2(-40.0f, 5.0f),
                new Vector2(-30.0f, 0.0f),
                new Vector2(-35.0f, 0.0f),
                new Vector2(-40.0f, 0.0f),
                new Vector2(-30.0f, -5.0f),
                new Vector2(-35.0f, -5.0f),
                new Vector2(-40.0f, -5.0f),
                new Vector2(30.0f, 5.0f),
                new Vector2(35.0f, 5.0f),
                new Vector2(40.0f, 5.0f),
                new Vector2(30.0f, 0.0f),
                new Vector2(35.0f, 0.0f),
                new Vector2(40.0f, 0.0f),
                new Vector2(30.0f, -5.0f),
                new Vector2(35.0f, -5.0f),
                new Vector2(40.0f, -5.0f),
            };
            
            List<Vector2> initial_velocity_input = new List<Vector2>()
            {
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(200.0f, 40.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f),
                new Vector2(-200.0f, 100.0f)
            };
            */

            float h = H;
            float num_particles_per_cell = 4.0f;
            float particle_spacing = math.sqrt(h * h / num_particles_per_cell);


            float radius = 15;
            Vector2 center_0 = new Vector2(-30, 30);
            Vector2 velocity_0 = new Vector2(150, 0);
            Vector2 center_1 = new Vector2(30, 30.0f + 1.5f * radius);
            Vector2 velocity_1 = new Vector2(-150, 0);

            List<Particle> particles =
                SceneBuilder.MakeParticleSphere(center_0, velocity_0, 0.0f, radius, particle_spacing, Color.red);
            particles.AddRange(SceneBuilder.MakeParticleSphere(center_1, velocity_1, 0.0f, radius, particle_spacing,
                Color.blue));
            
            
            
            
            
            
            
            
            /*
            float particle_width = 30;
            float particle_height = 40;

            float wall_width = 100.0f;
            
            Vector2 bottom_left_corner = new Vector2(- particle_width / 2.0f, 0.0f + 2 * h); 
            for (int i = 0; i * particle_spacing < particle_width; i++)
            {
                for (int j = 0; j * particle_spacing < particle_height; j++)
                {
                    Vector2 offset = new Vector2(particle_spacing * i, particle_spacing * j);
                    Vector2 randomization = 0.1f * particle_spacing * UnityEngine.Random.insideUnitCircle;
                    initial_position_input.Add(bottom_left_corner + offset + randomization);
                }
            }

            int numParticles = initial_position_input.Count;
            
            for (int pid = 0; pid < numParticles; pid++)
            {
                initial_velocity_input.Add(new Vector2(
                    60.0f,
                    0.0f));
            }
            */

            Debug.Log("ParticleCount: " + particles.Count);
            var positions = particles.Select(p => p.Position).ToArray();
            var velocities = particles.Select(p => p.Velocity).ToArray();
            double wall_width = 100.0;
            IceSYCLEngine engine = new IceSYCLEngine(positions, velocities, H, WallStiffness, wall_width);
            Controller = new ParticleController(engine, NumStepsPerFrame, NumDescentSteps, MuConstitutive, LambdaConstitutive, MuDamping, Gravity, particles);
        }

        private void FixedUpdate()
        {
            if (ContinueSimulation)
            {
                Controller.ContinueSimulationAsync(50 * NumSecsToSimulate);
            }
            Controller.LiveView = LiveView;
            NumFramesSimulated = Controller.NumFramesSimulated();
            CurrentFrameNumber = Controller.FrameNumber;
            ContinueSimulation = false;
            Controller.Update();
        }
    }
}