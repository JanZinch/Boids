using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoidsLogic
{
    [BurstCompile]
    public struct BoundsJob : IJobParallelFor
    {
        [ReadOnly] private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _accelerations;
        private readonly Vector3 _size;

        public BoundsJob(NativeArray<Vector3> positions, NativeArray<Vector3> accelerations, Vector3 size)
        {
            _positions = positions;
            _accelerations = accelerations;
            _size = size;
        }

        public void Execute(int index)
        {
            Vector3 position = _positions[index];
            Vector3 size = _size * 0.5f;

            _accelerations[index] += Compensate(-size.x - position.x, Vector3.right)
                                     + Compensate(size.x - position.x, Vector3.left)
                                     + Compensate(-size.y - position.y, Vector3.up)
                                     + Compensate(size.y - position.y, Vector3.down)
                                     + Compensate(-size.z - position.z, Vector3.forward)
                                     + Compensate(size.z - position.z, Vector3.back);
        }

        private static Vector3 Compensate(float delta, Vector3 direction)
        {
            const float threshold = 3f;
            const float multiplier = 100f;

            delta = Mathf.Abs(delta);

            if (delta > threshold)
            {
                return Vector3.zero;
            }
            else
            {
                return direction * (1 - delta / threshold) * multiplier;
            }
        }
    }
}