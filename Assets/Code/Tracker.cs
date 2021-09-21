using Biocrowds.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Tracker : Agent
{

    [SerializeField] private GameObject Player;


    private bool madeContact;
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


            if (Vector3.Distance(_playerPos, _agentPos) <= agentThreshold)
            {
                _animator = agents[i].GetComponent<Animator>();

                madeContact = true;
                //TODO another if/else for different animations regarding agent's ocean

                _animator.ResetTrigger("Neutral");
                _animator.SetTrigger("Happy");
            }

            else
            {
                _animator = agents[i].GetComponent<Animator>();

                _animator.ResetTrigger("Happy");
                _animator.SetTrigger("Neutral");

                madeContact = false;
            }
        }
    }
}