using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PIDTuner
{
    public class GeneticTuner
    {
        private List<GeneticInstance> _currentPopulation = new List<GeneticInstance>();
        public IReadOnlyList<GeneticInstance> CurrentPopulation => _currentPopulation.AsReadOnly();

        private readonly GenerationArguments _generationArguments;
        private MutationArguments _mutator;

        public int CurrentGeneration { get; private set; }

        public GeneticTuner(PIDTunerRequirements requirements, MutationArguments mutator, GenerationArguments generationArguments)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            _mutator = mutator ?? throw new ArgumentNullException(nameof(mutator));
            _generationArguments = generationArguments;
            _generationArguments.Validate();

            CurrentGeneration = 0;

            InitPopulation(requirements, _generationArguments);
        }

        private void InitPopulation(PIDTunerRequirements requirements, GenerationArguments generationArguments)
        {
            //TODO:Create init mutator
            var mutator = new RandomMutator();

            while (_currentPopulation.Count < generationArguments.GenerationSize)
            {
                var newInstance = new GeneticInstance(requirements, mutator);
                _currentPopulation.Add(newInstance);
            }
        }

        public void AdvanceGeneration()//This assumes that the score has been updated externally from this process
        {
            UnityEngine.Random.InitState((int)Time.realtimeSinceStartup);
            //Sort list by score and take top x
            var newGenerationParents = 
                _currentPopulation.OrderByDescending((x) => x.CurrentScore)
                .Take(_generationArguments.KeepTopCount)
                .ToList();
            //use this to re-populate the population

            var newGeneration = new List<GeneticInstance>(_generationArguments.GenerationSize);
            if (_generationArguments.KeepExactParents)
            {
                newGeneration.AddRange(newGenerationParents);
            }

            while (newGeneration.Count < _generationArguments.GenerationSize)
            {
                var parent = newGenerationParents[UnityEngine.Random.Range(0, newGenerationParents.Count)];
                var child = new GeneticInstance(parent, _mutator);
                newGeneration.Add(child);
            }

            _currentPopulation = newGeneration;

            CurrentGeneration++;
        }

        public void PrintDebugScores()
        {
            var best10 = CurrentPopulation.OrderByDescending((x) => x.CurrentScore).Take(10);

            Debug.Log($"Best 10 of generation {CurrentGeneration}");
            var str = "";
            foreach (var entry in best10)
            {
                str += $"{entry.CurrentScore}\n";
            }

            Debug.Log(str);
        }

        /// <summary>
        /// Represents a single 'strand of dna' that can be modified as necessary
        /// </summary>
        public class GeneticInstance
        {
            public GeneticInstance()
            {
                CurrentScore = 0.0f;
                ControllerGeneticData = new List<TuneableController>();
            }

            public GeneticInstance(PIDTunerRequirements requirements, MutationArguments mutator)
            {
                CurrentScore = 0.0f;
                ControllerGeneticData = new List<TuneableController>(requirements.Controllers.Count);

                foreach (var controller in requirements.Controllers)
                {
                    var controllerCopy = controller.Controller.DeepCopy();
                    controllerCopy.Mutate(mutator);
                    ControllerGeneticData.Add(controllerCopy);
                }
            }

            public GeneticInstance(GeneticInstance parent, MutationArguments mutator)
            {
                CurrentScore = 0.0f;
                ControllerGeneticData = new List<TuneableController>(parent.ControllerGeneticData.Count);

                foreach (var controller in parent.ControllerGeneticData)
                {
                    var childController = controller.DeepCopy();
                    childController.Mutate(mutator);
                    ControllerGeneticData.Add(childController);
                }
            }

            public float CurrentScore;
            public List<TuneableController> ControllerGeneticData;//Keeps a list of controllers, that can be deep copied into test instances or mutated
        }

        public struct GenerationArguments
        {
            public int GenerationSize;
            public int KeepTopCount;
            public bool KeepExactParents;

            public void Validate()
            {
                if (GenerationSize <= 0)
                {
                    throw new InvalidOperationException("GenerationSize must be greater than zero");
                }

                if (KeepTopCount <= 0)
                {
                    throw new InvalidOperationException("KeepTopCount must be greater than zero");
                }


                if (KeepTopCount >= GenerationSize)
                {
                    throw new InvalidOperationException("KeepTopCount must be greater than zeroless than generation size");
                }
            }
        }
    }
}