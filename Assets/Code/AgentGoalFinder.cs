using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biocrowds.DynamicGoals
{
    public class AgentGoalFinder : MonoBehaviour
    {

        [SerializeField]
        private Core.Agent _agent;

        private void Start()
        {
            _agent = GetComponent<Core.Agent>();

            GameObject tempGoal = null;
            float minDistance = float.MaxValue;

            foreach (var item in Core.World.instance._goalList)
            {
                var dist = Vector3.Distance(_agent.transform.position, item.transform.position);
                if (dist < minDistance)
                {
                    tempGoal = item;
                    minDistance = dist;
                }
            }


            _agent.Goal = tempGoal;

        }


    }
}

