using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    [SerializeField] private Transform _target;



    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_target.position + Vector3.up, Vector3.up);
    }
}
