﻿using Biocrowds.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Agent
{
    [SerializeField] private float Speed;
    [SerializeField] private float LookSpeed;

    public Camera camera;

    public float _playerPriority = 1f;
    


    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        _goalThreshold = 0.1f;

        _totalX = Mathf.FloorToInt(_world.Dimension.x / 2.0f) - 1;
        _totalZ = Mathf.FloorToInt(_world.Dimension.y / 2.0f);


        Vector2 cellPostion = new Vector2((int)Mathf.Floor(transform.position.x / 2.0f) * 2,
                                     (int)Mathf.Floor(transform.position.z / 2.0f) * 2);


        _currentCell = _world.posToCell[cellPostion];
    }



    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (World.instance.experiment.InteractionType == Experiment.Interaction.None)
        {
            transform.position += move * _world.SIMULATION_STEP;
        }
        else
        {

            for (int j = 0; j < _auxins.Count; j++)
            {
                Debug.DrawLine(_auxins[j].Position, transform.position, Color.red);
            }


            base.ClearAgent();

           

            _arrivedAtGoal = false;

            if (Vector3.Distance(transform.position, Goal.transform.position) < _goalThreshold && !_arrivedAtGoal)
            {
                _arrivedAtGoal = true;
            }


            if (move != Vector3.zero)
            {
                Goal.transform.position = transform.position + move;

                _goalPosition = Goal.transform.position;
            }

            if(World.instance.experiment.InteractionType == Experiment.Interaction.PlayerNormalLife)
            {

            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _playerPriority = 0.01f;
                    agentRadius = 0.1f;
                }
                else
                {
                    _playerPriority = 1f;
                    agentRadius = 1;
                }

            }
    

            FindNearAuxins();

            List<Auxin> agentAuxins = _auxins;

            //vector for each auxin
            for (int j = 0; j < agentAuxins.Count; j++)
            {
                //add the distance vector between it and the agent
                _distAuxin.Add(agentAuxins[j].Position - transform.position);

                //just draw the lines to each auxin
                //Debug.DrawLine(agentAuxins[j].Position, Player.transform.position, Color.green);
            }

            CalculateDirection();
            CalculateVelocity();
            PlayerStep();


        }


    }


    public void PlayerStep()
    {
        if (_arrivedAtGoal)
        {
            return;
        }


        if (_velocity.sqrMagnitude > 0.0f)
        {
            transform.position += (_velocity * _world.SIMULATION_STEP);

            //Debug.Log(_velocity);
        }
        //Debug.Log(_velocity);
    }


    protected override float DistanceMetric(Agent agent, Auxin auxin)
    {
        //Debug.Log("PASSANDO AQ PLAYER");
        return base.DistanceMetric(agent, auxin) * _playerPriority;
    }

  

}