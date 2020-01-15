using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIDTuner;
using Unity.Mathematics;
using UnityEngine;


public class PIDController3f : TuneableController
{
    private float3 _proportionalGain;
    private float3 _integralGain;
    private float3 _derrivativeGain;

    private float3 _integratlHistory;

    private float3 _minValue = new float3(-1.0f);
    private float3 _maxValue = new float3(1.0f);

    private float3 _maxGain = new float3(2.0f);
    private float3 _minGain = new float3(-2.0f);

    private float3 _processLast;

    public PIDController3f(float3 proportionalGain, float3 intergralGaion, float3 derrivativeGain)
    {
        _proportionalGain = proportionalGain;
        _integralGain = intergralGaion;
        _derrivativeGain = derrivativeGain;
    }

    public float3 TargetValue;

    public float3 Update(float3 currentValue, float deltaTime)
    {

        var error = TargetValue - currentValue;

        var delta = _processLast - currentValue;
        _processLast = currentValue;
        //Do some shit

        _integratlHistory += error * deltaTime * _integralGain;
        _integratlHistory = math.clamp(_integratlHistory, _minValue, _maxValue);

        //Retune some value
        //hopes and dreams realized

        var p = error * _proportionalGain;
        var i = _integratlHistory * _integralGain;
        var d = _derrivativeGain * delta / Time.fixedDeltaTime;

        //TODO:Some better debug system than this nightmare, graphs ??? draw to inspector
        //Debug.Log($"proportional:{p}    intergral:{i}    derrivative:{d}");

        var value = p + i + d;
        return math.clamp(value, _minValue, _maxValue);
    }

    public TuneableController DeepCopy()
    {
        return new PIDController3f(_proportionalGain, _integralGain, _derrivativeGain);

    }

    public void CopyFrom(TuneableController other)
    {
        if(other is PIDController3f as3f)
        {
            _proportionalGain = as3f._proportionalGain;
            _integralGain = as3f._integralGain;
            _derrivativeGain = as3f._derrivativeGain;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public void Reset()
    {
        _processLast = 0.0f;
        _integratlHistory = 0.0f;
    }

    public void Mutate(MutationArguments mutation)
    {
        mutation.Mutate(ref _proportionalGain, new float3(_minGain), new float3(_maxGain));
        mutation.Mutate(ref _integralGain, new float3(_minGain), new float3(_maxGain));
        mutation.Mutate(ref _derrivativeGain, new float3(_minGain), new float3(_maxGain));
    }

    public override string ToString()
    {
        return
            $"PIDController3f Arguments:\nProportional Gain:{_proportionalGain}\nIntergral Gain:{_integralGain}\n Derrivative Gain:{_derrivativeGain}";
    }
}