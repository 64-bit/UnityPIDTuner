using System;
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

        void Start()
        {
            var tempInstance = Instantiate(TestPrefab);
            var tempInterface = tempInstance.GetComponentInChildren<IPIDTunerTestInterface>();
            if (tempInterface == null)
            {
                throw new InvalidOperationException("Could not find IPIDTunerTestInterface in prefab");
            }

            var requirements = tempInterface.GetTestRequirements();
            InitFromRequirements(requirements);
        }

        private void InitFromRequirements(PIDTunerRequirements requirments)
        {
            //We have the test requirements, now we must create and init the genetic tuner, and then run that for like, a bunch of generations
        }
    }

    public class GeneticInstance
    {

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
        public List<TuneableControllerMetadata> Controllers;

        public float3 FreespaceNeeded;
    }

    //How to deal with UI / Binding for different controllers ?

    public abstract class TuneableControllerMetadata
    {
        public TuneableController Controller;
        public string Name;
        public int Index;
    }
}