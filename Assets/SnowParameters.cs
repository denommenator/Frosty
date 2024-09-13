using UnityEngine;

namespace Frosty
{
    public class SnowParameters : MonoBehaviour
    {
        public double MuConstitutive = 50000;
        public double LambdaConstitutive = 50000;
        public double ThetaC = 2.0E-2;
        public double ThetaS = 5.0E-3;
        public double Xi = 10;
        public double MaxExp = 10;
        public double Density = 0.4; 
    }
}