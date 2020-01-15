using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class PID_Test_Chaser : MonoBehaviour
{
    public Transform Target;

    public float Thrust = 10.0f;

    public PIDController3f PIDController;

    private Rigidbody _rigidbody;

    public float P = 1.0f;
    public float I = 1.0f;
    public float D = 1.0f;

    void Awake()
    {
        PIDController = new PIDController3f(new float3(P), new float3(I), new float3(D));
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        PIDController.TargetValue = Target.transform.position;
        var thrust = PIDController.Update(transform.position, Time.fixedDeltaTime);

        _rigidbody.AddForce(thrust * Thrust);
    }


}


