using Biocrowds.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Biocrowds.Emotion
{


    public class OCEANSpawner : Core.AgentSpawner
    {
        [System.Serializable]
        public struct OCEANSpawnProfile
        {
            public int IntrovertedAgentQuantity;
            public int ExtrovertedAgentQuantity;
            public GameObject Goal;
        }

        [SerializeField] private OCEANSpawnProfile _profileOCEAN;

        private void Start()
        {
            StartCoroutine(CreateAgents());
        }

        protected override IEnumerator CreateAgents()
        {
            Transform agentPool = new GameObject("Agents").transform;
            const float initialXPos = 1.0f;
            const float initialZPos = 1.0f;


            float xPos = initialXPos;
            float zPos = initialZPos;

            var experiment = World.instance.experiment;

            for (int i = 0; i < _profileOCEAN.IntrovertedAgentQuantity; i++)
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
                newAgent.Goal = _profileOCEAN.Goal;   //really defines the agent's goal

                ((AgentOCEAN)newAgent)._emotionProfile = ((ExperimentOCEAN)experiment).IntrovertProfile;

                World.instance._agents.Add(newAgent);

                xPos += 1.0f;
                if (xPos > World.instance.Dimension.x)
                {
                    xPos = initialXPos;
                    zPos += 1.0f;
                }
            }

            for (int i = _profileOCEAN.IntrovertedAgentQuantity; i < _profileOCEAN.IntrovertedAgentQuantity + _profileOCEAN.ExtrovertedAgentQuantity; i++)
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
                newAgent.Goal = _profileOCEAN.Goal;   //really defines the agent's goal

                ((AgentOCEAN)newAgent)._emotionProfile = ((ExperimentOCEAN)experiment).ExtrovertProfile;

                World.instance._agents.Add(newAgent);

                xPos += 1.0f;
                if (xPos > World.instance.Dimension.x)
                {
                    xPos = initialXPos;
                    zPos += 1.0f;
                }
            }

            yield return null;
        }
    }
}
