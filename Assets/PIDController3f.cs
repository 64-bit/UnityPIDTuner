using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class PIDController3f
{
    private float3 _proportionalGain;
    private float3 _integralGain;
    private float3 _derrivativeGain;

    private float3 _integratlHistory;

    private float3 _minValue = new float3(-1.0f);
    private float3 _maxValue = new float3(1.0f);

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
        Debug.Log($"proportional:{p}    intergral:{i}    derrivative:{d}");

        var value = p + i + d;
        return math.clamp(value, _minValue, _maxValue);
    }
}