using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace PIDTuner
{
    /// <summary>
    /// Represents a type of controller that can be auto-tuned by this system
    /// </summary>
    public interface TuneableController
    { 
        /// <summary>
        /// Create a deep copy of the whole controller
        /// </summary>
        /// <returns>A new independent copy of the controller with all non-transient state of the old controller</returns>
        TuneableController DeepCopy();

        void CopyFrom(TuneableController other);

        /// <summary>
        /// Reset the controller to it's initial state
        /// </summary>
        void Reset();

        /// <summary>
        /// Mutate this current instance of the controller using the provided mutation arugments
        /// </summary>
        /// <param name="mutation"></param>
        void Mutate(MutationArguments mutation);
    }

    /// <summary>
    /// Provides a read-only set of mutation arugments to a tuneable controller
    /// </summary>
    public class MutationArguments
    {

        //Helper methods to mutate specific varible types

        public virtual void Mutate(ref float current, float min = -1.0f, float max = 1.0f)
        {
            //TODO:Write something sane, like moving the vale by a normal distribution ammount
            float move = 0.05f * (max - min);
            move *= UnityEngine.Random.Range(-1.0f, 1.0f);
            current = math.clamp(current + move, min, max);                           
        }

        public virtual void Mutate(ref float3 current, float3 min, float3 max)
        {
            for (var i = 0; i < 3; i++)
            {
                float move = 0.05f * (max[i] - min[i]);
                move *= UnityEngine.Random.Range(-1.0f, 1.0f);

                current[i] = math.clamp(current[i] + move, min[i], max[i]);
            }
        }

        public virtual void Mutate(ref float4 current, float4 min, float4 max)
        {
            for (var i = 0; i < 4; i++)
            {
                float move = 0.05f * (max[i] - min[i]);
                move *= UnityEngine.Random.Range(-1.0f, 1.0f);

                current[i] = math.clamp(current[i] + move, min[i], max[i]);
            }
        }
    }

    /// <summary>
    /// Provides a read-only set of mutation arugments to a tuneable controller
    /// </summary>
    public class RandomMutator : MutationArguments
    {

        //Helper methods to mutate specific varible types

        public override void Mutate(ref float current, float min = -1.0f, float max = 1.0f)
        {
            //TODO:Write something sane, like moving the vale by a normal distribution ammount
            current = UnityEngine.Random.Range(min, max);
        }

        public override void Mutate(ref float3 current, float3 min, float3 max)
        {
            for (var i = 0; i < 3; i++)
            {
                current[i] = UnityEngine.Random.Range(min[i], max[i]);
            }
        }

        public override void Mutate(ref float4 current, float4 min, float4 max)
        {
            for (var i = 0; i < 4; i++)
            {
                current[i] = UnityEngine.Random.Range(min[i], max[i]);
            }
        }
    }
}
