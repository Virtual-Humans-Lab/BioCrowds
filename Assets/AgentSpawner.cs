using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Biocrowds.Core
{
    public struct SpawnProfile
    {
        public int NumberOfAgents;
    }

    public class AgentSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnProfile _profile;


        private IEnumerator CreateAgents()
        {
            Transform agentPool = new GameObject("Agents").transform;
            const float initialXPos = 1.0f;
            const float initialZPos = 1.0f;


            float xPos = initialXPos;
            float zPos = initialZPos;

            //instantiate agents
            for (int i = 0; i < _profile.NumberOfAgents; i++)
            {
                Agent newAgent = Instantiate(World.instance.experiment.agentePrefab, new Vector3(xPos, 0f, zPos), Quaternion.identity, agentPool);

                newAgent.name = i.ToString();  //name
                newAgent.CurrentCell = _cells[i];  //agent cell
                //newAgent.agentRadius = AGENT_RADIUS;  //agent radius
                newAgent.Goal = _goal.gameObject;   //really defines the agent's goal


                _agents.Add(newAgent);

                xPos += 1.0f;
                if (xPos > _dimension.x)
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