using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Frosty
{
    public class Scene : MonoBehaviour
    {
        private ParticleController Controller;
        public IBuildScene SceneBuilder;
        
        public int NumStepsPerFrame = 1;
        public int NumDescentSteps = 20;
        public int MaxNumBacksteps = 20;
        public int NumSecsToSimulate = 5;
        
        public double MuDamping = 1.0;
        public double Gravity = 981.0f;
        public float H = 5;
        
        public int NumFramesSimulated;
        public int CurrentFrameNumber;
        public bool LiveView = true;
        
        public bool ContinueSimulation = true;

        private void Start()
        {
            //IBuildScene sceneBuilder = GetComponent<IBuildScene>();
            Controller = SceneBuilder.BuildScene(H, MuDamping, Gravity, NumStepsPerFrame, NumDescentSteps, MaxNumBacksteps);
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

    public abstract class IBuildScene : MonoBehaviour
    {
        public abstract ParticleController BuildScene(double h, double mu_damping, double gravity, int num_steps_per_frame, int num_descent_steps, int max_num_backsteps);
    }
    
}