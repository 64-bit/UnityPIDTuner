using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    public class TunerTestChaseTarget : MonoBehaviour
    {
        public float3 CurrentTarget
        {
            get
            {
                if(_currentNode < NodePoints.Count)
                {
                    return NodePoints[_currentNode];
                }
                else
                {
                    return transform.position;
                }
            }
        }

        public float Speed = 2.5f;
        public List<float3> NodePoints = new List<float3>();

        private int _currentNode = 0;
        private bool _isDone = false;

        public event Action OnFinished;

        public void Init(int nodes, float3 pointOffset)
        {
            Random.InitState(1337);

            for (int i = 0; i < nodes; i++)
            {
                var point = new float3(
                    Random.Range(-10.0f, 10.0f),
                    0.0f,
                    Random.Range(-10.0f, 10.0f));

                NodePoints.Add(point + pointOffset);
            }


        }

        void FixedUpdate()
        {
            if (_isDone)
            {
                return;
            }

            var currentPos = (float3)transform.position;
            float distanceToTarget = math.distance(currentPos, CurrentTarget);

            float maxMove = Time.fixedDeltaTime * Speed;

            if (distanceToTarget <= maxMove)
            {
                transform.position = CurrentTarget;
                _currentNode++;
                if (_currentNode == NodePoints.Count)
                {
                    //DOne
                    _isDone = true;
                    OnFinished?.Invoke();
                    return;
                }
            }
            else
            {
                //MOve in direction
                var toTarget = math.normalize(CurrentTarget - currentPos);
                transform.position += (Vector3)(toTarget * maxMove);
            }
        }
    }
}