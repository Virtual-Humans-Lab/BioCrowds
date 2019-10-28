using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace Biocrowds.Modules
{
    struct data
    {
        public float mean;
        public float dp;
        public float var;
    }

    [RequireComponent(typeof(Core.Agent))]
    public class TimeMachine : MonoBehaviour
    {

        // Globals
        private float deltaTime = 1f / 30f;
        // Time machine
        private List<GameObject> obstacles;
        private float enviroment_complexity;

        // Agent to which time machine is acoplada
        private Core.Agent agent;
        private Core.World world;
        // HACK: Time counter
        private int t;

        // Accuracy test
        private Dictionary<float, Vector3> predictions;
        private Dictionary<float, Vector3> actuals;
        // List with diff between predicted and actual
        private List<float> diff;

        // Constants
        private float alpha = 1;
        private float areaOfAgent = 0.6f;

        // Initialization
        void Start()
        {
            actuals = new Dictionary<float, Vector3>();
            t = 0;
            predictions = new Dictionary<float, Vector3>();
            agent = this.gameObject.GetComponent<Core.Agent>();
            world = GameObject.Find("WorldPrefab").gameObject.GetComponent<Core.World>();
            calculateEnviromentComplexity(); // Assuming no change
        }


        void Update()
        {
            Vector3[] prediction = time_machine(deltaTime * 60); // 60 = 2 seconds ahead
            predictions.Add(t, prediction[1]);
            actuals.Add(t, prediction[0]);
            t++;
            if (t > 900)
            {
                // TODO: 900 should be "is simulation still happening". This "t" is a HACK that works for this case

                // Writes the results and ends the simulation
                experimentAccuracy();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
             
        }

        float densityArroundPosition(Vector3 currentPosition)
        {
            return world.agentsAroundPosition(currentPosition) / 36;
        }

        Vector3[] time_machine(float deltaTime)
        {
            Vector3 currentPosition   = this.gameObject.transform.position;
            Vector3 unary_goal        = (agent.Goal.transform.position - currentPosition).normalized;
            Vector3 steer             = agent._velocity.magnitude * unary_goal * deltaTime;
            Vector3 predictedPosition = currentPosition + steer * (1 - enviroment_complexity);

            float density = densityArroundPosition(currentPosition);

            // Load, choose and apply weibuls based on density
            float dens = 0;
            float minDist = 10000;
            float[] densities = new float[] { 0.25f, 0.5f, 0.75f, 1f, 1.5f, 1.75f, 2f, 2.25f };
            float closestDensity = 2f;
            foreach(float ddensity in densities)
            {
                if (Mathf.Abs(density - ddensity) < minDist)
                {
                    minDist = Mathf.Abs(density - ddensity);
                    closestDensity = ddensity;
                }
            }
            // The follow commented lines should be refactored
            // Getting the path
            string path = System.IO.Path.Combine(getExperimentsPath(), "weibuls");
            // Random value from distribution
            int index = UnityEngine.Random.Range(0, 5000);
            // Reads all the file into the array and gets the index line
            string[] lines = File.ReadAllLines(string.Format("{0}Dens{1}.txt", path, closestDensity), Encoding.UTF8);
            dens =  float.Parse(lines[index]);

            predictedPosition = predictedPosition - dens * (unary_goal) * deltaTime;

            Debug.DrawLine(currentPosition, predictedPosition);

            return new Vector3[] { currentPosition, predictedPosition };
        }

        string getExperimentsPath()
        {
             return System.IO.Path.Combine(Application.dataPath, "Experiments");
        }

        data experimentAccuracy()
        {
            diff = new List<float>();
            data results = new data();
            int sampleSize = 0;
            foreach(KeyValuePair<float, Vector3> item in actuals)
            {
                if (predictions.ContainsKey(item.Key))
                { // calculate the euclidian distance for each set of points in the intersection { pred & actuals }
                    sampleSize++;
                    float diffx = Mathf.Pow(item.Value.x - actuals[item.Key].x, 2);
                    float diffy = Mathf.Pow(item.Value.y - actuals[item.Key].y, 2);
                    float diffz = Mathf.Pow(item.Value.z - actuals[item.Key].z, 2);
                    diff.Add(Mathf.Sqrt(diffx + diffy + diffz));
                }
            }

            results.mean = diff.Sum(x => x) / sampleSize;
            float var = 0;
            foreach (float f in diff)
            {
                var += Mathf.Pow((f - results.mean), 2);
            }
            results.var = var / sampleSize;
            results.dp = Mathf.Sqrt(results.var);
            return results;
        }

        void calculateEnviromentComplexity()
        {
            obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Obstacle"));
            float worldArea = GameObject.Find("WorldPrefab").GetComponent<Core.World>().Dimension.x *
                        GameObject.Find("WorldPrefab").GetComponent<Core.World>().Dimension.x;

            float numberOfAgents = GameObject.FindGameObjectsWithTag("Agent").Length;

            // Calculate the sum of obstacles area
            float sumOfObsticlesArea = 0;
            float numberOfObstacles = 0;
            foreach (GameObject g in obstacles)
            {
                numberOfObstacles++;
                sumOfObsticlesArea += g.transform.localScale.x * g.transform.localScale.y * g.transform.localScale.z;
            }

            float coefficient = (numberOfAgents * areaOfAgent + alpha * numberOfObstacles) / (worldArea - sumOfObsticlesArea);
            enviroment_complexity = Mathf.Min(1, coefficient);
        }

    }
}
