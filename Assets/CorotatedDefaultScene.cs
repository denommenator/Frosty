using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Frosty
{
    public class CorotatedDefaultScene : IBuildScene
    {
        public double WallStiffness = 1000;
        public double MuConstitutive = 50000;
        public double LambdaConstitutive = 50000;
        
        public override ParticleController BuildScene(double h, double mu_damping, double gravity, int num_steps_per_frame, int num_descent_steps, int num_linear_solve_steps, int max_num_backsteps)
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


            float radius = 12;
            float offset_factor = 0.8f;
            Vector2 center_0 = new Vector2(-radius, radius);
            Vector2 velocity_0 = new Vector2(50, 0);
            Vector2 center_1 = new Vector2(radius, radius + offset_factor * 2 * radius);
            Vector2 velocity_1 = new Vector2(-50, 0);

            Color color_1 = new Color(1.0f, 0.2f, 0.2f);
            Color color_2 = new Color(0.2f, 0.2f, 1.0f);
            List<Particle> particles =
                SceneBuilder.MakeRandomParticleSphere(center_0, velocity_0, 0.0f, radius, particle_spacing, color_1);
            particles.AddRange(SceneBuilder.MakeRandomParticleSphere(center_1, velocity_1, 0.0f, radius, particle_spacing,
                color_2));
            
            
            
            
            
            
            
            
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
            double wall_width = 70.0;
            IceSYCLEngine iceSyclEngine = new IceSYCLEngine(positions, velocities, h, WallStiffness, wall_width, 1.0);
            CorotatedEngine corotatedEngine = new CorotatedEngine(iceSyclEngine);
            corotatedEngine.NumStepsPerFrame = num_steps_per_frame;
            corotatedEngine.NumDescentSteps = num_descent_steps;
            corotatedEngine.NumLinearSolveSteps = num_linear_solve_steps;
            corotatedEngine.MaxNumBacksteps = max_num_backsteps;
            corotatedEngine.MuDamping = mu_damping;
            corotatedEngine.LambdaConstitutive = LambdaConstitutive;
            corotatedEngine.MuConstitutive = MuConstitutive;
            corotatedEngine.Gravity = gravity;
            return new ParticleController(corotatedEngine, particles);
        }
    }
}