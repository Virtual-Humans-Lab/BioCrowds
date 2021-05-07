using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(transform.position.y != 0)
        {
            Debug.LogWarning("Porra nardino bota esse goal no chão");
            transform.position = Vector3.Scale(transform.position, Vector3.one - Vector3.up);
        }

       
    } 

   
}
