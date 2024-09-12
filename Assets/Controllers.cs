using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Frosty
{
    public class ParticleController
    {
        public ParticleController(IceSYCLEngine iceSYCLEngine, int numStepsPerFrame, int numDescentSteps, double muConstitutive, double lambdaConstitutive, double muDamping, double gravity, List<Particle> particles)
        {
            BallRenderer = new BallRenderer(particles);
            SimulationFrames = new SimulationFrames<Vector3[]>();
            IceSYCLEngine = iceSYCLEngine;
            MuConstitutive = muConstitutive;
            LambdaConstitutive = lambdaConstitutive;
            NumStepsPerFrame = numStepsPerFrame;
            NumDescentSteps= numDescentSteps;
            MuDamping = muDamping;
            Gravity = gravity;
        }

        private BallRenderer BallRenderer;
        private ISimulationFrames<Vector3[]> SimulationFrames;
        private IceSYCLEngine IceSYCLEngine;
        public PlayerState PlayerState;
        public int FrameNumber = 0;
        public double MuConstitutive;
        public double LambdaConstitutive;
        public double MuDamping;
        public double Gravity;
        public int NumStepsPerFrame;
        public int NumDescentSteps;
        public bool LiveView = true;

        public void RunSimulation(int numFrames)
        {
            for(int frame=0; frame < numFrames; frame++)
            {
                //Debug.Log("Stepping the simulation!");
                Debug.Log("Running a new frame!");
                IceSYCLEngine.StepFrame(NumStepsPerFrame, NumDescentSteps, MuConstitutive, LambdaConstitutive, MuDamping, Gravity);
                Vector3[] particlePositions = IceSYCLEngine.GetPositions();
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