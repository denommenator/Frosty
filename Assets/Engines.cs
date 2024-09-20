using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Frosty
{
    public interface IEngine : IDisposable
    {
        void StepFrame();
        Vector3[] GetPositions();
        
    }
    public class CorotatedEngine : IEngine
    {
        public CorotatedEngine(IceSYCLEngine engine)
        {
            Engine = engine;
        }

        public IceSYCLEngine Engine;
        public double MuConstitutive = 50000;
        public double LambdaConstitutive = 50000;
        public double MuDamping = 1.0;
        public double Gravity = 981;
        public int NumStepsPerFrame = 50;
        public int NumDescentSteps = 10;
        public int MaxNumBacksteps = 10;

        public void StepFrame()
        {
            Engine.StepFrame(NumStepsPerFrame, NumDescentSteps, MaxNumBacksteps, MuConstitutive, LambdaConstitutive, MuDamping, Gravity);
        }

        public Vector3[] GetPositions()
        {
            return Engine.GetPositions();
        }

        public void Dispose()
        {
            Engine.Dispose();
        }
    }
    
    public class SnowEngine : IEngine, IDisposable
    {
        public SnowEngine(IceSYCLEngine engine, IntPtr psis)
        {
            Engine = engine;
            Psis = psis;
        }

        public IceSYCLEngine Engine;
        private IntPtr Psis;
        public double MuDamping = 1.0;
        public double Gravity = 981;
        public int NumStepsPerFrame = 50;
        public int NumDescentSteps = 10;

        public void StepFrame()
        {
            Engine.StepFramePlastic(Psis, NumStepsPerFrame, NumDescentSteps, MuDamping, Gravity);
        }

        public Vector3[] GetPositions()
        {
            return Engine.GetPositions();
        }

        public void Dispose()
        {
            Engine.Dispose();
            Marshal.FreeHGlobal(Psis);
        }
    }
}