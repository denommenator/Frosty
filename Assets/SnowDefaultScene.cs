using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Frosty
{
    public class SnowDefaultScene : IBuildScene
    {
        public double WallStiffness = 1000;
        public double MuConstitutive = 50000;
        public double LambdaConstitutive = 50000;
        public double ThetaC = 2.0E-2;
        public double ThetaS = 5.0E-3;
        public double Xi = 10;
        public double MaxExp = 10;
        public double Density = 0.4;
        
        public override ParticleController BuildScene(double h, double mu_damping, double gravity, int num_steps_per_frame)
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

            float h_float = (float)h;
            float num_particles_per_cell = 4.0f;
            float particle_spacing = math.sqrt(h_float * h_float / num_particles_per_cell);


            float radius_base = 5.0f;
            float middle_shrinkage = 0.7f;
            float top_shrinkage = 0.49f;

            float radius_middle = radius_base * middle_shrinkage;
            float radius_top = radius_base * top_shrinkage;

            float middle_center = 2 * radius_base + radius_middle;
            float top_center = 2 * (radius_base + radius_middle) + radius_top;
            
            Vector2 center_base = new Vector2(0, radius_base);
            Vector2 center_middle = new Vector2(0, middle_center);
            Vector2 center_top = new Vector2(0, top_center);

            List<Particle> particles =
                SceneBuilder.MakeParticleSphere(center_base, Vector2.zero, 0.0f, radius_base, particle_spacing, Color.blue);
            particles.AddRange(SceneBuilder.MakeParticleSphere(center_middle, Vector2.zero, 0.0f, radius_middle, particle_spacing,
                Color.blue));
            particles.AddRange(SceneBuilder.MakeParticleSphere(center_top, Vector2.zero, 0.0f, radius_top, particle_spacing,
                Color.blue));


            float snowball_radius = 0.2f * radius_base;
            Vector2 center_snowball = new Vector2(-radius_base - 10 * radius_base, (middle_center + top_center) * 0.5f);
            Vector2 velocity_snowball = new Vector2(200.0f, 0.0f);
            
            particles.AddRange(SceneBuilder.MakeParticleSphere(center_snowball, velocity_snowball, 0.0f, snowball_radius, particle_spacing,
                Color.red));
            

            double wall_width = 20 * radius_base;
            
            
            Debug.Log("ParticleCount: " + particles.Count);
            var positions = particles.Select(p => p.Position).ToArray();
            var velocities = particles.Select(p => p.Velocity).ToArray();
            
            IceSYCLEngine iceSyclEngine = new IceSYCLEngine(positions, velocities, h, WallStiffness, wall_width, Density);
            IntPtr Psis = iceSyclEngine.CreateSnowConstitutiveModels(particles.Count, MuConstitutive, LambdaConstitutive, Xi, ThetaC, ThetaS, MaxExp);
            SnowEngine corotatedEngine = new SnowEngine(iceSyclEngine, Psis);
            corotatedEngine.NumStepsPerFrame = num_steps_per_frame;
            corotatedEngine.MuDamping = mu_damping;
            corotatedEngine.Gravity = gravity;
            return new ParticleController(corotatedEngine, particles);
        }
    }
}