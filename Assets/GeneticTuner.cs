using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace PIDTuner
{
    public class GeneticTuner
    {
        private List<GeneticInstance> _currentPopulation;
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

            InitPopulation(requirements);
        }

        private void InitPopulation(PIDTunerRequirements requirements)
        {

        }

        public void AdvanceGeneration()//This assumes that the score has been updated externally from this process
        {
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
        }

        /// <summary>
        /// Represents a single 'strand of dna' that can be modified as necessary
        /// </summary>
        public class GeneticInstance
        {
            public GeneticInstance()
            {
                CurrentScore = -1.0f;
                ControllerGeneticData = new List<TuneableController>();
            }

            public GeneticInstance(GeneticInstance parent, MutationArguments mutator)
            {
                CurrentScore = -1.0f;
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