using System.Collections.Generic;
using UnityEngine;

namespace Biocrowds.Modules
{

    [RequireComponent(typeof(Core.Agent))]
    public class TimeMachine : MonoBehaviour
    {

        // Globals
        private List<GameObject> obstacles;
        private float enviroment_complexity;
        private Core.Agent agent;

        // Constants
        private float alpha = 1;
        private float areaOfAgent = 1;

        // Initialization
        void Start()
        {
            agent = this.gameObject.GetComponent<Core.Agent>();

            // ENVIROMENT COMPLEXITY ------------{{{{
            obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Obstacle"));
            float worldArea = GameObject.Find("World").GetComponent<Core.World>().Dimension.x *
                        GameObject.Find("World").GetComponent<Core.World>().Dimension.x;

            float numberOfAgents = GameObject.FindGameObjectsWithTag("Agent").Length;

            // Calculate the sum of obstacles area (thanks diogo)
            float sumOfObsticlesArea = 0;
            float numberOfObstacles = 0;
            foreach (GameObject g in obstacles)
            {
                numberOfObstacles++;
                sumOfObsticlesArea += g.transform.localScale.x * g.transform.localScale.y * g.transform.localScale.z;
            }


            float coefficient = (numberOfAgents * areaOfAgent + alpha * numberOfObstacles) / (worldArea - sumOfObsticlesArea);
            enviroment_complexity = Mathf.Min(1, coefficient);
            //----}}}}
        }


        void Update()
        {
            // Draws a debug line at each frame for each agent indicating the predicted
            // position and increments the accuracy counter
        }

        Vector3 time_machine(float deltaTime)
        {
            Vector3 currentPosition   = this.gameObject.transform.position;
            Vector3 unary_goal        = agent.Goal.transform.position.normalized;
            Vector3 steer             = agent._velocity.magnitude * unary_goal * deltaTime;
            Vector3 predictedPosition = currentPosition + steer * (1 - enviroment_complexity);
            return predictedPosition;
        }

    }
}
