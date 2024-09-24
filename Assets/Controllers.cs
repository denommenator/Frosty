using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frosty
{
    public class ParticleController
    {
        public ParticleController(IEngine engine, List<Particle> particles)
        {
            BallRenderer = new BallRenderer(particles);
            SimulationFrames = new SimulationFrames<Vector3[]>();
            Engine = engine;
        }

        private BallRenderer BallRenderer;
        private ISimulationFrames<Vector3[]> SimulationFrames;
        public IEngine Engine;
        public PlayerState PlayerState;
        public int FrameNumber = 0;
        public bool LiveView = true;

        public void RunSimulation(int numFrames)
        {
            for(int frame=0; frame < numFrames; frame++)
            {
                //Debug.Log("Stepping the simulation!");
                
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Engine.StepFrame();
                timer.Stop();
                var time_ms = timer.ElapsedMilliseconds;
                string time = time_ms < 1000 ? time_ms.ToString() + " ms" : (time_ms / 1000).ToString() + " seconds";
                float time_for_second = 50.0f * time_ms / (1000.0f * 60);
                string time_for_second_string = time_for_second > 1.0 ? time_for_second.ToString() + " minutes" : (60 * time_for_second).ToString() + " seconds";
                Debug.Log("Frame took: " + time + ". Simulation time for one film second: " + time_for_second_string );
                Vector3[] particlePositions = Engine.GetPositions();
                SimulationFrames.QueueNewFrame(particlePositions);
            }
        }

        public void Update()
        {
            Vector3[] particlePositions;
            SimulationFrames.UpdateQueuedSimulationFrames();
            if (SimulationFrames.NumCurrentFrames() == 0)
            {
                return;
            }
            
            if (LiveView)
            {
                particlePositions = SimulationFrames.GetFrame(SimulationFrames.NumCurrentFrames() - 1);
            }
            else
            {
                particlePositions = SimulationFrames.GetFrame(FrameNumber);
            }
            BallRenderer.UpdateBallPositions(particlePositions);
            
            if (FrameNumber == SimulationFrames.NumCurrentFrames() - 1)
            {
                FrameNumber = 0;
            }
            else
            {
                FrameNumber++;
            }


        }
        
        public int NumFramesSimulated()
        {
            return SimulationFrames.NumCurrentFrames();
        }

        public void ContinueSimulationAsync(int numFrames)
        {
            Task runSim = new Task(() => RunSimulation(numFrames));
            runSim.Start();
        }
    }
    
    

    public enum PlayerState
    {
        Playing,
        Paused
    }
}