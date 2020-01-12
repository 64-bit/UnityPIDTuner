using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PhysicsShip : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private float _requestedThrottle;
    private float3 _requestedTranslation;
    private float3 _requestedTorque;

    public float MaxThrust = 10000.0f;
    public float3 MaxTranslation = 1000.0f;
    public float3 MaxTorque = 1000.0f;

    public float Throttle
    {
        get => _requestedThrottle;
        set => _requestedThrottle = math.clamp(value, -1.0f, 1.0f);
    }

    public float3 Torque
    {
        get => _requestedTorque;
        set => _requestedTorque = math.clamp(value, new float3(-1.0f), new float3(1.0f));
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Throttle = 0.0f;
        Torque = float3.zero;
    }

    void FixedUpdate()
    {
        _rigidbody.AddRelativeForce(_requestedTranslation * MaxTranslation);
        _rigidbody.AddRelativeForce(new float3(0.0f,0.0f, _requestedThrottle * MaxThrust));
        _rigidbody.AddRelativeTorque(_requestedTorque * MaxTorque);
    }
}