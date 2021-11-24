using Biocrowds.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class LookAt : Agent
{

    [SerializeField] private GameObject Player;


    //private bool madeContact;
    private Vector3 _playerPos;
    private Vector3 _agentPos;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var agents = World.instance._agents;

        for (int i = 0; i < agents.Count; i++)
        {

            _agentPos = agents[i].transform.position;
            _playerPos = Player.transform.position;

            Quaternion rotTarget = Quaternion.LookRotation(_playerPos - _agentPos);
            agents[i].transform.rotation = Quaternion.RotateTowards(agents[i].transform.rotation, rotTarget, 100f * Time.deltaTime);
        }
    }
}