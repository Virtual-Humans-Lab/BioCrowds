using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Biocrowds.Core
{
    [System.Serializable]
    public struct SpawnProfile
    {
        public int NumberOfAgents;
        public GameObject Goal;
    }

    public class AgentSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnProfile _profile;

        private void Start()
        {
            StartCoroutine(CreateAgents());
        }

        protected virtual IEnumerator CreateAgents()
        {
            Transform agentPool = new GameObject("Agents").transform;
            const float initialXPos = 1.0f;
            const float initialZPos = 1.0f;


            float xPos = initialXPos;
            float zPos = initialZPos;

            var experiment = World.instance.experiment;

            //Debug.Log(experiment.agentPrefab);

            //instantiate agents
            for (int i = 0; i < _profile.NumberOfAgents; i++)
            {

                Agent newAgent = Instantiate(experiment.agentPrefab, new Vector3(xPos, 0f, zPos), Quaternion.identity, agentPool);

                newAgent.name = i.ToString();  //name

                Vector2 cellPostion = new Vector2
                (
                    Math.Abs((Mathf.FloorToInt(xPos / 2.0f)) - 1),
                    Math.Abs((Mathf.FloorToInt(zPos / 2.0f)) - 1)
                 );

                newAgent.CurrentCell = World.instance.posToCell[cellPostion];
                //newAgent.agentRadius = AGENT_RADIUS;  //agent radius
                newAgent.Goal = _profile.Goal;   //really defines the agent's goal


                World.instance._agents.Add(newAgent);

                xPos += 1.0f;
                if (xPos > World.instance.Dimension.x)
                {
                    xPos = initialXPos;
                    zPos += 1.0f;
                }



                // yield return null;
            }

            yield return null;
        }
    }
}