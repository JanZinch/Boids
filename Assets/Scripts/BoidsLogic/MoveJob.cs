using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace BoidsLogic
{
    public struct MoveJob : IJobParallelForTransform
    {
        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _accelerations;
        private float _deltaTime;
        private float _velocityLimit;

        public MoveJob(NativeArray<Vector3> positions, NativeArray<Vector3> velocities, NativeArray<Vector3> accelerations,
            float deltaTime, float velocityLimit)
        {
            _positions = positions;
            _velocities = velocities;
            _accelerations = accelerations;
            _deltaTime = deltaTime;
            _velocityLimit = velocityLimit;
        }

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 velocity = _velocities[index] + _accelerations[index] * _deltaTime;
            Vector3 direction = velocity.normalized;
            velocity = direction * Mathf.Clamp(velocity.magnitude, 1f, _velocityLimit);
            
            transform.position += velocity * _deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);

            _positions[index] = transform.position;
            _velocities[index] = velocity;
            _accelerations[index] = Vector3.zero;
        }
    }
}