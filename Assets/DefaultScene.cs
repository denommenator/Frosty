using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frosty
{
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
                
            
            int particle_width = 40;
            int particle_height = 20;
            int numParticles = particle_width * particle_height;
            for (int i = 0; i < particle_width; i++)
            {
                for (int j = 0; j < particle_height; j++)
                {
                    initial_position_input.Add(new Vector2(
                        -100.0f + 5 * i,
                        -50.0f + 5 * j));
                }
            }

            for (int pid = 0; pid < numParticles; pid++)
            {
                initial_velocity_input.Add(new Vector2(
                    0.0f,
                    0.0f));
            }
            

            IceSYCLEngine engine = new IceSYCLEngine(initial_position_input.ToArray(), initial_velocity_input.ToArray(), H, WallStiffness);
            Controller = new ParticleController(engine, NumStepsPerFrame, NumDescentSteps, MuConstitutive, LambdaConstitutive, MuDamping, Gravity);
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