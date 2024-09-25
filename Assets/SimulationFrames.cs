using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Frosty
{
    public interface ISimulationFrames<FrameData>
    {
        int NumCurrentFrames();
        FrameData GetFrame(int frameNum);
        
        /// <summary>
        /// Don't call UpdateQueuedSimulationFrames and GetFrame concurrently
        /// </summary>
        void UpdateQueuedSimulationFrames();
        void QueueNewFrame(FrameData frameData);
    }
    
    public class SimulationFrames<FrameData> : ISimulationFrames<FrameData>
    {
        public int NumCurrentFrames()
        {
            return RenderingFrames.Count;
        }

        public FrameData GetFrame(int frameNum)
        {
            //Debug.Log("retrieving frame " + frameNum + " of " + RenderingFrames.Count);
            return RenderingFrames[frameNum];
        }
        
        public void UpdateQueuedSimulationFrames()
        {
            if (QueuedFrames.TryDequeue(out FrameData frameData))
            {
                RenderingFrames.Add(frameData);
            }
        }

        public void QueueNewFrame(FrameData framePos)
        {
            QueuedFrames.Enqueue(framePos);
        }

        List<FrameData> RenderingFrames = new List<FrameData>();
        ConcurrentQueue<FrameData> QueuedFrames = new ConcurrentQueue<FrameData>();
        private int ParticleCount;
    }
}