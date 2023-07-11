using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoidsLogic
{
    public struct AccelerationJob : IJobParallelFor
    {
        [ReadOnly] private NativeArray<Vector3> _positions;
        [ReadOnly] private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _accelerations;

        private readonly float _destinationThreshold;
        private readonly Vector3 _weights;
        
        private int OthersCount => _positions.Length - 1;

        public AccelerationJob(NativeArray<Vector3> positions, NativeArray<Vector3> velocities,
            NativeArray<Vector3> accelerations, float destinationThreshold, Vector3 weights)
        {
            _positions = positions;
            _velocities = velocities;
            _accelerations = accelerations;
            _destinationThreshold = destinationThreshold;
            _weights = weights;
        }

        public void Execute(int currentIndex)
        {
            Vector3 averageSpread = Vector3.zero;
            Vector3 averageVelocity = Vector3.zero;
            Vector3 averagePosition = Vector3.zero;

            for (int otherIndex = 0; otherIndex < OthersCount; otherIndex++)
            {
                if (otherIndex == currentIndex) continue;

                Vector3 targetPosition = _positions[otherIndex];
                Vector3 positionDifference = _positions[currentIndex] - targetPosition;

                if (positionDifference.magnitude > _destinationThreshold) continue;

                averageSpread += positionDifference.normalized;
                averageVelocity += _velocities[otherIndex];
                averagePosition += targetPosition;

            }

            
            _accelerations[currentIndex] += 
                _weights.x * GetAverage(averageSpread) 
                + _weights.y * GetAverage(averageVelocity) 
                + _weights.z * (GetAverage(averagePosition) - _positions[currentIndex]);
        }

        private Vector3 GetAverage(Vector3 source)
        {
            return source / OthersCount;
        }
    }
}