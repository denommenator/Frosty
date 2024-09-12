using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Frosty
{
    public class IceSYCLEngine
    {
        private IntPtr Engine;
        private IntPtr CurrentPositionPtr;
        private double[] CurrentPosition;
        //private IntPtr DeformationGradientPtr;
        //private double[] DeformationGradient;
        public int ParticleCount { get; set; }

        // [DllImport("libIceSYCL_NativeAPI")]
        // private static extern IntPtr new_ParticleState(int particle_count, IntPtr particle_state_raw);

        // [DllImport("libIceSYCL_NativeAPI")]
        // private static extern void delete_ParticleState(IntPtr particle_state);

        [DllImport("libIceSYCL_NativeAPI")]
        private static extern IntPtr create_engine(int particle_count, IntPtr positions, IntPtr velocities, double h, double wall_stiffness);

        [DllImport("libIceSYCL_NativeAPI")]
        private static extern void copy_current_positions(IntPtr engine, IntPtr current_positions_raw_ptr);

        [DllImport("libIceSYCL_NativeAPI")]
        private static extern void copy_deformation_gradients(IntPtr engine, IntPtr deformation_gradients_raw_ptr);

        // [DllImport("libIceSYCL_NativeAPI")]
        // private static extern void step_frame(IntPtr engine, int num_steps_per_frame, double mu_constitutive, double lambda_constitutive, double mu_damping, double gravity);
        
        [DllImport("libIceSYCL_NativeAPI")]
        private static extern void step_frame_implicit(IntPtr engine, int num_steps_per_frame, int num_descent_steps, double mu_constitutive, double lambda_constitutive, double mu_damping, double gravity);

        [DllImport("libIceSYCL_NativeAPI")]
        private static extern void delete_engine(IntPtr engine);

        public IceSYCLEngine(Vector2[] positions, Vector2[] velocities, double h, double wall_stiffness)
        {
            Debug.Log(System.IO.File.Exists("Assets/Plugins/libIceSYCL_NativeAPI.so"));
            int numParticles = positions.Length;
            ParticleCount = numParticles;
            List<double> positions_input = new List<double>();
            List<double> velocities_input = new List<double>();
            for (int pid = 0; pid < numParticles; pid++)
            {
                positions_input.Add(positions[pid].x);
                positions_input.Add(positions[pid].y);

                velocities_input.Add(velocities[pid].x);
                velocities_input.Add(velocities[pid].y);
            }

            var positions_raw = Marshal.AllocHGlobal(positions_input.Count * sizeof(double));
            var velocities_raw = Marshal.AllocHGlobal(velocities_input.Count * sizeof(double));

            Marshal.Copy(positions_input.ToArray(), 0, positions_raw, positions_input.Count);
            Marshal.Copy(velocities_input.ToArray(), 0, velocities_raw, velocities_input.Count);

            Engine = create_engine(ParticleCount, positions_raw, velocities_raw, h, wall_stiffness);

            Marshal.FreeHGlobal(velocities_raw);

            CurrentPosition = new double[positions_input.Count];
            CurrentPositionPtr = Marshal.AllocHGlobal(positions_input.Count * sizeof(double));

            //DeformationGradient = new double[numParticles * 4];
            //DeformationGradientPtr = Marshal.AllocHGlobal(DeformationGradient.Length * sizeof(double));

        }
        
        public void StepFrame(int num_steps_per_frame, int num_descent_steps, double mu_constitutive, double lambda_constitutive, double mu_damping, double gravity)
        {
            step_frame_implicit(Engine, num_steps_per_frame, num_descent_steps, mu_constitutive, lambda_constitutive, mu_damping, gravity);
        }
        
        public Vector3[] GetPositions()
        {
            copy_current_positions(Engine, CurrentPositionPtr);
            Marshal.Copy(CurrentPositionPtr, CurrentPosition, 0, CurrentPosition.Length);
            Vector3[] positions = new Vector3[ParticleCount];
            for (int pid = 0; pid < ParticleCount; pid++)
            {
                positions[pid] = new Vector3(
                    (float)CurrentPosition[2 * pid + 0],
                    (float)CurrentPosition[2 * pid + 1], 
                    0.0f);
            }

             return positions;
        }

        ~IceSYCLEngine()
        {
            delete_engine(Engine);
            Marshal.FreeHGlobal(CurrentPositionPtr);
            //Marshal.FreeHGlobal(DeformationGradientPtr);

        }
    }
}