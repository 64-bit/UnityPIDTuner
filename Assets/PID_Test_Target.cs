using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PID_Test_Target : MonoBehaviour
{

    public float Speed = 2.5f;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position -= Vector3.left * Time.deltaTime * Speed;
        }


        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * Time.deltaTime * Speed;
        }
    }
}

