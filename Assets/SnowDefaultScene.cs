using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Frosty
{
    public class SnowDefaultScene : IBuildScene
    {

        public SnowParameters SnowParameters;
        public double WallStiffness = 1000;
        public float BaseRadius = 5.0f;
        public float SnowBallSizeFactor = 0.2f;
        public float SnowBallHeightFactor = 0.8f;
        public float SnowBallDistance = 4.0f;
        public Vector2 SnowBallVelocity = 50 * Vector2.right;
        
        public override ParticleController BuildScene(double h, double mu_damping, double gravity, int num_steps_per_frame)
        {
            List<Vector2> initial_position_input = new List<Vector2>();
            List<Vector2> initial_velocity_input = new List<Vector2>();
            

            float h_float = (float)h;
            float num_particles_per_cell = 4.0f;
            float particle_spacing = math.sqrt(h_float * h_float / num_particles_per_cell);


            float radius_base = BaseRadius;
            float middle_shrinkage = 0.7f;
            float top_shrinkage = 0.49f;

            float radius_middle = radius_base * middle_shrinkage;
            float radius_top = radius_base * top_shrinkage;

            float middle_center = 2 * radius_base + radius_middle;
            float top_center = 2 * (radius_base + radius_middle) + radius_top;
            
            Vector2 center_base = new Vector2(0, radius_base);
            Vector2 center_middle = new Vector2(0, middle_center);
            Vector2 center_top = new Vector2(0, top_center);

            List<Particle> particles = new List<Particle>();
            float number_density_to_spacing_radius = 1.5f;
            particles.AddRange(SceneBuilder.MakeRandomParticleSphere(center_base, Vector2.zero, 0.0f, radius_base, number_density_to_spacing_radius * particle_spacing, Color.blue));
            particles.AddRange(SceneBuilder.MakeRandomParticleSphere(center_middle, Vector2.zero, 0.0f, radius_middle, number_density_to_spacing_radius * particle_spacing,
                Color.blue));
            particles.AddRange(SceneBuilder.MakeRandomParticleSphere(center_top, Vector2.zero, 0.0f, radius_top, number_density_to_spacing_radius * particle_spacing,
                Color.blue));


            float snowball_radius = SnowBallSizeFactor * radius_base;
            float snowman_top = 2 * (radius_base + radius_middle + radius_top);
            Vector2 center_snowball = new Vector2(-radius_base - SnowBallDistance * radius_base, SnowBallHeightFactor * snowman_top);
            Vector2 velocity_snowball = SnowBallVelocity;
            
            particles.AddRange(SceneBuilder.MakeRandomParticleSphere(center_snowball, velocity_snowball, 0.0f, snowball_radius, number_density_to_spacing_radius * particle_spacing,
                Color.red));
            

            double wall_width = 100 * radius_base;
            
            
            Debug.Log("ParticleCount: " + particles.Count);
            var positions = particles.Select(p => p.Position).ToArray();
            var velocities = particles.Select(p => p.Velocity).ToArray();
            
            IceSYCLEngine iceSyclEngine = new IceSYCLEngine(positions, velocities, h, WallStiffness, wall_width, SnowParameters.Density);
            IntPtr Psis = iceSyclEngine.CreateSnowConstitutiveModels(particles.Count, SnowParameters.MuConstitutive, SnowParameters.LambdaConstitutive, SnowParameters.Xi, SnowParameters.ThetaC, SnowParameters.ThetaS, SnowParameters.MaxExp);
            SnowEngine corotatedEngine = new SnowEngine(iceSyclEngine, Psis);
            corotatedEngine.NumStepsPerFrame = num_steps_per_frame;
            corotatedEngine.MuDamping = mu_damping;
            corotatedEngine.Gravity = gravity;
            return new ParticleController(corotatedEngine, particles);
        }
    }
}