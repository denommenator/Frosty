using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frosty
{
    public class DefaultScene : MonoBehaviour
    {
        private ParticleController Controller;
        public double WallStiffness = 1000;
        public double CSpeedOfSound = 1000;
        public int NumSecsToSimulate = 5;
        public bool ContinueSimulation = true;
        public int NumFramesSimulated;
        public int CurrentFrameNumber;
        void Start()
        {
            
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
                
            /*
            int numParticles = 10;
            for (int pid = 0; pid < numParticles; pid++)
            {
                initial_position_input.Add(new Vector2(
                    -100.0f / numParticles * pid,
                    5.0f));
            }

            List<Vector2> initial_velocity_input = new List<Vector2>();
            for (int pid = 0; pid < numParticles; pid++)
            {
                initial_velocity_input.Add(new Vector2(
                    100.0f,
                    100.0f / numParticles * pid));
            }
            */

            IceSYCLEngine engine = new IceSYCLEngine(initial_position_input.ToArray(), initial_velocity_input.ToArray(), WallStiffness);
            Controller = new ParticleController(engine, CSpeedOfSound);
        }

        private void FixedUpdate()
        {
            if (ContinueSimulation)
            {
                Controller.ContinueSimulationAsync(50 * NumSecsToSimulate);
            }

            NumFramesSimulated = Controller.NumFramesSimulated();
            CurrentFrameNumber = Controller.FrameNumber;
            ContinueSimulation = false;
            Controller.Update();
        }
    }
}