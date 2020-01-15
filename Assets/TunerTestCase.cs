using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    public class TunerTestCase : MonoBehaviour , IPIDTunerTestInterface
    {
        public int Nodes = 10;

        public TunerTestChaseTarget ChaseTarget;
        public PID_Test_Chaser Chaser;

        private bool _hasFinished = false;

        public bool DrawLines;

        void Awake()
        {

        }

        public PIDTunerRequirements GetTestRequirements()
        {
            var reqs = new PIDTunerRequirements()
            {
                FreespaceNeeded = new float3(25.0f)
            };

            reqs.Controllers.Add(new TuneableControllerMetadata()
            {
                Controller = Chaser.PIDController,
                Name = "Chaser PID",
                Index = 0
            });

            return reqs;
        }

        public void ProvideOnFinished(Action onFinished)
        {
            ChaseTarget.OnFinished += onFinished;
            ChaseTarget.OnFinished += () => _hasFinished = true;
        }

        public void OnStartTest()
        {
            ChaseTarget.Init(Nodes, transform.position);
            Chaser.Target = ChaseTarget.transform;
        }

        public float GetErrorForFrame()
        {
            if (_hasFinished)
            {
                return 0.0f;
            }
            return math.distance(ChaseTarget.transform.position, Chaser.transform.position);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(20.0f,20.0f,20.0f));

            float error = GetErrorForFrame();

            error /= 20.0f;
            var c = Color.Lerp(Color.green, Color.red, error);

            Gizmos.color = c;
            Gizmos.DrawLine(ChaseTarget.transform.position, Chaser.transform.position);
        }
    }
}
