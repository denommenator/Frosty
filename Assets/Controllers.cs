using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Frosty
{
    public class ParticleController
    {
        public ParticleController(IceSYCLEngine iceSYCLEngine, double cSpeedOfSound, double muDamping)
        {
            BallRenderer = new BallRenderer(iceSYCLEngine.ParticleCount);
            SimulationFrames = new SimulationFrames<Vector3[]>();
            IceSYCLEngine = iceSYCLEngine;
            CSpeedOfSound = cSpeedOfSound;
            MuDamping = muDamping;
        }

        private BallRenderer BallRenderer;
        private ISimulationFrames<Vector3[]> SimulationFrames;
        private IceSYCLEngine IceSYCLEngine;
        public PlayerState PlayerState;
        public int FrameNumber = 0;
        public double CSpeedOfSound;
        public double MuDamping;

        public void RunSimulation(int numFrames)
        {
            for(int frame=0; frame < numFrames; frame++)
            {
                //Debug.Log("Stepping the simulation!");
                IceSYCLEngine.StepFrame(CSpeedOfSound, MuDamping);
                Vector3[] particlePositions = IceSYCLEngine.GetPositions();
                SimulationFrames.QueueNewFrame(particlePositions);
            }
        }

        public void Update()
        {
            Debug.Log("Running a new frame!");
            Vector3[] particlePositions;
            SimulationFrames.UpdateQueuedSimulationFrames();
            if (SimulationFrames.NumCurrentFrames() == 0)
            {
                return;
            }
            particlePositions = SimulationFrames.GetFrame(FrameNumber);
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