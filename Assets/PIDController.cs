using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class PIDController
{
    private float _proportionalGain;
    private float _integralGain;
    private float _derrivativeGain;

    private float _integratlHistory;

    private float _minValue = -1.0f;
    private float _maxValue = 1.0f;

    private float _processLast;

    public PIDController(float proportionalGain, float I, float derrivativeGain)
    {
        _proportionalGain = proportionalGain;
        _integralGain = I;
        _derrivativeGain = derrivativeGain;
    }

    public float TargetValue;

    public float Update(float currentValue, float deltaTime)
    {

        float error = TargetValue - currentValue;

        float delta = _processLast - currentValue;
        _processLast = currentValue;
        //Do some shit

        _integratlHistory += error * deltaTime * _integralGain;
        _integratlHistory = math.clamp(_integratlHistory, _minValue, _maxValue);

        //Retune some value
        //hopes and dreams realized
        
        float p = error * _proportionalGain;
        float i = _integratlHistory * _integralGain;
        float d = _derrivativeGain * delta;

        //TODO:Some better debug system than this nightmare, graphs ??? draw to inspector
        Debug.Log($"proportional:{p:F3}    intergral:{i:F3}    derrivative:{d:F3}");


        float value = p + i + d;
        return math.clamp(value, _minValue, _maxValue);
    }
}