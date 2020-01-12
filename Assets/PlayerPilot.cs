using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class PlayerPilot : MonoBehaviour
{
    private PhysicsShip _ship;

    void Start()
    {
        _ship = GetComponent<PhysicsShip>();
    }

    // Update is called once per frame
    void Update()
    {
        var pitch = Input.GetAxis("Pitch");
        var yaw = Input.GetAxis("Yaw");
        var roll = Input.GetAxis("Roll");

        _ship.Throttle = Input.GetAxis("Throttle");
        _ship.Torque = new float3(-pitch, yaw, -roll);     
    }
}