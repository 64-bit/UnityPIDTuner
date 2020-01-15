using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIDTuner;
using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    public class PIDTunner : MonoBehaviour
    {
        public GameObject TestPrefab;

        public int Generations = 10;
        public int Population = 100;
        public int KeepTop = 5;

        private GeneticTuner _geneticTuner;

        private float3 _testSize;

        void Start()
        {
            var tempInstance = Instantiate(TestPrefab);
            var tempInterface = tempInstance.GetComponentInChildren<IPIDTunerTestInterface>();
            if (tempInterface == null)
            {
                throw new InvalidOperationException("Could not find IPIDTunerTestInterface in prefab");
            }

            Destroy(tempInstance);

            var requirements = tempInterface.GetTestRequirements();
            InitFromRequirements(requirements);
            StartCoroutine(WholeProcess());
        }

        private void InitFromRequirements(PIDTunerRequirements requirments)
        {
            //We have the test requirements, now we must create and init the genetic tuner, and then run that for like, a bunch of generations
            //TODO:HERE

            _testSize = requirments.FreespaceNeeded * 1.5f;

            _geneticTuner = new GeneticTuner(requirments, new MutationArguments(),new GeneticTuner.GenerationArguments()
            {
                GenerationSize = Population,
                KeepTopCount = KeepTop,
                KeepExactParents = true
            });
        }

        private IEnumerator WholeProcess()
        {
            yield return null;
            while (_geneticTuner.CurrentGeneration < Generations)
            {
                yield return ProcessGeneration();
                _geneticTuner.AdvanceGeneration();
                yield return null;
            }

            //Print result

            var bestController = _geneticTuner.CurrentPopulation.OrderByDescending((x) => x.CurrentScore).First();
            Debug.Log($"Best Controller Args:{bestController.ControllerGeneticData[0]}");
        }

        private IEnumerator ProcessGeneration()
        {
            int xSize = (int)Math.Sqrt(Population);

            var finishedCount = new IntContainer();

            var prefabInstances = new List<GameObject>();
            var testInterfaces = new List<IPIDTunerTestInterface>();
        
            for (int i = 0; i < Population; i++)
            {
                int xPos = i % xSize;
                int yPos = i / xSize;

                float3 prefabPos = _testSize * new float3(xPos, 0.0f, yPos);

                var prefabInstance = GameObject.Instantiate(TestPrefab, prefabPos, Quaternion.identity);
                prefabInstances.Add(prefabInstance);
                var testInterface = prefabInstance.GetComponentInChildren<IPIDTunerTestInterface>();
                var requs = testInterface.GetTestRequirements();

                var geneticIndividual = _geneticTuner.CurrentPopulation[i];

                for (int j = 0; j < requs.Controllers.Count; j++)
                {
                    requs.Controllers[j].Controller.CopyFrom(geneticIndividual.ControllerGeneticData[j]);                 
                }

                var incrementer = new CounterIncrementer(finishedCount);
                testInterface.ProvideOnFinished(incrementer.Trigger());
                testInterface.OnStartTest();
                testInterfaces.Add(testInterface);
            }

            float[] accumulatedError = new float[Population];

            while (finishedCount.Value < Population)
            {
                //Accumualte error
                for (int i = 0; i < Population; i++)
                {
                    accumulatedError[i] += testInterfaces[i].GetErrorForFrame();
                }

                yield return new WaitForFixedUpdate();
            }

            //Now everything is finished, apply scores to generation
            for (int i = 0; i < Population; i++)
            {
                _geneticTuner.CurrentPopulation[i].CurrentScore = -accumulatedError[i];//Stupid hack to get score working right
            }

            _geneticTuner.PrintDebugScores();
            DestroyList(prefabInstances);
        }

        private void DestroyList(List<GameObject> objects)
        {
            foreach (var obj in objects)
            {
                GameObject.Destroy(obj);
            }
        }

        private class IntContainer //Caputure correctly in clouser for stupidness
        {
            public int Value;
        }

        private class CounterIncrementer
        {
            private bool _hasFired = false;
            private IntContainer _intTarget;

            public CounterIncrementer(IntContainer container)
            {
                _intTarget = container;
            }

            public Action Trigger()
            {
                return () =>
                {
                    if (_hasFired)
                    {
                        return;
                    }

                    _hasFired = true;
                    _intTarget.Value += 1;
                };
            }
        }
    }



    public interface IPIDTunerTestInterface
    {

        //Fetch requirements, Initial state for tuning is dependent on how your prefab returns the controllers when GetTestRequirements is called
        PIDTunerRequirements GetTestRequirements();//Should always return the same values

        //Provide arguments (done though requirements)

        //Set OnFinished callback
        void ProvideOnFinished(Action onFinished);

        //Start Test
        void OnStartTest();


        //fetch list of all errors over frames ?
        //Or fetch error for frame
        float GetErrorForFrame();
    }

    public class PIDTunerRequirements
    {
        //Controllers
        public readonly List<TuneableControllerMetadata> Controllers = new List<TuneableControllerMetadata>();

        public float3 FreespaceNeeded;
    }

    //How to deal with UI / Binding for different controllers ?

    public class TuneableControllerMetadata
    {
        public TuneableController Controller;
        public string Name;
        public int Index;
    }
}