using UnityEngine;

namespace Frosty
{
    public interface IEngine
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

        public void StepFrame()
        {
            Engine.StepFrame(NumStepsPerFrame, NumDescentSteps, MuConstitutive, LambdaConstitutive, MuDamping, Gravity);
        }

        public Vector3[] GetPositions()
        {
            return Engine.GetPositions();
        }
    }
}