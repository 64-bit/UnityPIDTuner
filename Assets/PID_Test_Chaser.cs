using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


class PID_Test_Chaser : MonoBehaviour
{
    public PID_Test_Target target;

    public float Thrust = 10.0f;

    private PIDController3f _pid;

    private Rigidbody _rigidbody;

    public float P = 1.0f;
    public float I = 1.0f;
    public float D = 1.0f;

    void Awake()
    {
        _pid = new PIDController3f(new float3(P), new float3(I), new float3(D));
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        _pid.TargetValue = target.transform.position;
        var thrust = _pid.Update(transform.position, Time.fixedDeltaTime);

        _rigidbody.AddForce(thrust * Thrust);
    }


}


