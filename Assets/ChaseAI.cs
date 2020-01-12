using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChaseAI : MonoBehaviour
{

    public PhysicsShip Target;
    private PhysicsShip _ship;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _ship = GetComponent<PhysicsShip>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var toTarget = Target.transform.position - transform.position;

        var directionToTarget = toTarget.normalized;

        float dotToTarget = math.dot(directionToTarget, transform.forward);
        if (dotToTarget <= 0.0f)
        {
            dotToTarget = 0.0f;
        }

        var inverseDotToTarget = 1.0f - dotToTarget;


        var distanceToTarget = toTarget.magnitude;

        var targetInLocal = transform.InverseTransformPoint(Target.transform.position);

        float yaw, pitch;

        pitch = 0.0f;

        if (targetInLocal.x > 0.0f)
        {
            yaw = 1.0f;
        }
        else
        {
            yaw = -1.0f;
        }

        if (targetInLocal.z > 0.0f)
        {
            pitch = -1.0f;
        }
        else
        {
            pitch = 1.0f;
        }



        yaw *= 1.5f * inverseDotToTarget;
        yaw = math.clamp(yaw , - 1.0f, 1.0f);

        pitch *= 1.5f * inverseDotToTarget;
        pitch = math.clamp(pitch, -1.0f, 1.0f);

        var globalRotVel = _rigidbody.angularVelocity;
        var localRotVel = transform.InverseTransformVector(globalRotVel);

        float roll = 0.0f;

        if(math.abs(localRotVel.z) > 0.05f)
        {
            roll = -localRotVel.z * 0.1f;
        }

        _ship.Torque = new float3(pitch, yaw, roll);

        _ship.Throttle = 1.5f * dotToTarget;
    }
}
