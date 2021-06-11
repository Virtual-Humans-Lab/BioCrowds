/// ---------------------------------------------
/// Contact: Henry Braun
/// Brief: Defines the world environment
/// Thanks to VHLab for original implementation
/// Date: November 2017 
/// ---------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEditor.AI;
using System.Collections;
using UnityEngine.AI;
using System;
using UnityEngine.Profiling;

namespace Biocrowds.Core
{
    public class World : MonoBehaviour

    {

        public static World instance;

        //[field: SerializeField]
        //public PlayerController Player { get; private set; }

        public Dictionary<Vector2, Cell> posToCell { get; private set; }

        //agent radius
        private const float AGENT_RADIUS = 1.0f;

        //radius for auxin collide
        private const float AUXIN_RADIUS = 0.1f;

        //density
        [SerializeField]
        private float AUXIN_DENSITY = 0.45f; //0.65f;

        [SerializeField]
        private Terrain _terrain;

        [SerializeField]
        private Transform _goal;

        [field: SerializeField]
        public List<GameObject> _goalList { get; private set; }

        public float SIMULATION_STEP { get; private set; } = 1f / 30f;

        [SerializeField]
        private Vector2 _dimension = new Vector2(30.0f, 20.0f);
        public Vector2 Dimension
        {
            get { return _dimension; }
        }

        //number of agents in the scene
        [SerializeField]
        private int _maxAgents = 30;

        //agent prefab
        [SerializeField]
        private Agent _agentPrefab;

        [SerializeField]
        private Cell _cellPrefab;

        [SerializeField]
        private Auxin _auxinPrefab;

        [SerializeField]
        private BoxCollider _obstacleCollider;

        List<Agent> _agents = new List<Agent>();
        List<Cell> _cells = new List<Cell>();

        public List<Cell> Cells
        {
            get { return _cells; }
        }


        public int Frame { get; private set; }

        //max auxins on the ground
        private int _maxAuxins;
        private bool _isReady;

        // Use this for initialization
        IEnumerator Start()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("The World Script " + gameObject.name + " was destroyed");
            }

            posToCell = new Dictionary<Vector2, Cell>();

            //change terrain size according informed
            _terrain.terrainData.size = new Vector3(_dimension.x, _terrain.terrainData.size.y, _dimension.y);

            //create all cells based on dimension
            yield return StartCoroutine(CreateCells());

            //populate cells with auxins
            yield return StartCoroutine(DartThrowing());

            //create our agents
            yield return StartCoroutine(CreateAgents());

            //build the navmesh at runtime
#if         UNITY_EDITOR
            UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
#endif
            //wait a little bit to start moving
            yield return new WaitForSeconds(1.0f);
            _isReady = true;
       
        }
        private IEnumerator CreateCells()
        {
            Transform cellPool = new GameObject("Cells").transform;

            for (int i = 0; i < _dimension.x / 2; i++) //i + agentRadius * 2
            {
                for (int j = 0; j < _dimension.y / 2; j++) // j + agentRadius * 2
                {
                    //instantiante a new cell
                    Cell newCell = Instantiate(_cellPrefab, new Vector3(1.0f + (i * 2.0f), 0.0f, 1.0f + (j * 2.0f)), Quaternion.Euler(90.0f, 0.0f, 0.0f), cellPool);

                    //change its name
                    newCell.name = "Cell [" + i + "][" + j + "]";

                    //metadata for optimization
                    newCell.X = i;
                    newCell.Z = j;

                    posToCell.Add(new Vector2(i, j), newCell);

                    _cells.Add(newCell);

                    //yield return null;
                }
            }

            yield return null;

        }

        private IEnumerator DartThrowing()
        {
            //lets set the qntAuxins for each cell according the density estimation
            float densityToQnt = AUXIN_DENSITY;

            Transform auxinPool = new GameObject("Auxins").transform;

            densityToQnt *= 2f / (2.0f * AUXIN_RADIUS);
            densityToQnt *= 2f / (2.0f * AUXIN_RADIUS);

            _maxAuxins = (int)Mathf.Floor(densityToQnt);

            //for each cell, we generate its auxins
            for (int c = 0; c < _cells.Count; c++)
            {
                //Dart throwing auxins
                //use this flag to break the loop if it is taking too long (maybe there is no more space)
                int flag = 0;
                for (int i = 0; i < _maxAuxins; i++)
                {
                    float x = UnityEngine.Random.Range(_cells[c].transform.position.x - 0.99f, _cells[c].transform.position.x + 0.99f);
                    float z = UnityEngine.Random.Range(_cells[c].transform.position.z - 0.99f, _cells[c].transform.position.z + 0.99f);

                    //see if there are auxins in this radius. if not, instantiante
                    List<Auxin> allAuxinsInCell = _cells[c].Auxins;
                    bool createAuxin = true;
                    for (int j = 0; j < allAuxinsInCell.Count; j++)
                    {
                        float distanceAASqr = (new Vector3(x, 0f, z) - allAuxinsInCell[j].Position).sqrMagnitude;

                        //if it is too near no need to add another
                        if (distanceAASqr < AUXIN_RADIUS * AUXIN_RADIUS)
                        {
                            createAuxin = false;
                            break;
                        }
                    }

                    //if i have found no auxin, i still need to check if is there obstacles on the way
                    if (createAuxin)
                    {
                        //sphere collider to try to find the obstacles
                        //NavMeshHit hit;
                        //createAuxin = NavMesh.Raycast(new Vector3(x, 2f, z), new Vector3(x, -2f, z), out hit, 1 << NavMesh.GetAreaFromName("Walkable")); //NavMesh.GetAreaFromName("Walkable")); // NavMesh.AllAreas);
                        //createAuxin = NavMesh.SamplePosition(new Vector3(x, 0.0f, z), out hit, 0.1f, 1 << NavMesh.GetAreaFromName("Walkable"));
                        //bool isBlocked = _obstacleCollider.bounds.Contains(new Vector3(x, 0.0f, z));
                        Collider[] hitColliders = Physics.OverlapSphere(new Vector3(x, 0f, z), AUXIN_RADIUS + 0.1f, 1 << LayerMask.NameToLayer("Obstacle"));
                        createAuxin = (hitColliders.Length == 0);
                    }

                    //check if auxin can be created there
                    if (createAuxin)
                    {
                        Auxin newAuxin = Instantiate(_auxinPrefab, new Vector3(x, 0.0f, z), Quaternion.identity, auxinPool);

                        //change its name
                        newAuxin.name = "Auxin [" + c + "][" + i + "]";
                        //this auxin is from this cell
                        newAuxin.Cell = _cells[c];
                        //set position
                        newAuxin.Position = new Vector3(x, 0f, z);

                        //add this auxin to this cell
                        _cells[c].Auxins.Add(newAuxin);

                        //reset the flag
                        flag = 0;

                        ////speed up the demonstration a little bit...
                        //if (i % 200 == 0)
                        //    yield return null;
                    }
                    else
                    {
                        //else, try again
                        flag++;
                        i--;
                    }

                    //if flag is above qntAuxins (*2 to have some more), break;
                    if (flag > _maxAuxins * 2)
                    {
                        //reset the flag
                        flag = 0;
                        break;
                    }
                }
            }

            yield return null;

        }


        private IEnumerator CreateAgents()
        {
            Transform agentPool = new GameObject("Agents").transform;
            const float initialXPos = 1.0f;
            const float initialZPos = 1.0f;


            float xPos = initialXPos;
            float zPos = initialZPos;

            //instantiate agents
            for (int i = 0; i < _maxAgents; i++)
            {
                Agent newAgent = Instantiate(_agentPrefab, new Vector3(xPos, 0f, zPos), Quaternion.identity, agentPool);

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

        //public void Spawnplayer()
        //{
        //    Debug.Log("Player Spawned");
        //    GameObject player = Spawn();
        //    Player = player.GetComponent<PlayerMovement>();

        //    //Player.Goal = _goal.gameObject;

             

        //    playerTransform = player.transform;
        //}


        //public GameObject Spawn()
        //{
        //    return Instantiate(_PlayerPrefab, _psArea.transform.position, Quaternion.identity);
        //}

        // Update is called once per frame
        void Update()
        {
            if (!_isReady)
                return;

            //reset auxins
            for (int i = 0; i < _cells.Count; i++)
                for (int j = 0; j < _cells[i].Auxins.Count; j++)
                    _cells[i].Auxins[j].ResetAuxin();

            //Profiler.BeginSample("FindNearAuxins");


            //Player.FindNearAuxins();

            //find nearest auxins for each agent
            for (int i = 0; i < _agents.Count; i++)
                _agents[i].FindNearAuxins();

            //Profiler.EndSample();

            //find the agent
            List<Auxin> agentAuxins = new List<Auxin>();
            

            ////vector for each auxin
            //for (int j = 0; j < agentAuxins.Count; j++)
            //{
            //    //add the distance vector between it and the agent
            //    Player._distAuxin.Add(agentAuxins[j].Position - Player.transform.position);

            //    //just draw the lines to each auxin
            //    //Debug.DrawLine(agentAuxins[j].Position, Player.transform.position, Color.green);
            //}

            //Player.CalculateDirection();
            //Player.CalculateVelocity();
            //Player.PlayerStep();

            /*
             * to find where the agent must move, we need to get the vectors from the agent to each auxin he has, and compare with 
             * the vector from agent to goal, generating a angle which must lie between 0 (best case) and 180 (worst case)
             * The calculation formula was taken from the Bicho´s master thesis and from Paravisi OSG implementation.
            */
            /*for each agent:
            1 - for each auxin near him, find the distance vector between it and the agent
            2 - calculate the movement vector 
            3 - calculate speed vector 
            4 - step
            */


            for (int i = 0; i < _maxAgents; i++)
            {
                agentAuxins = _agents[i].Auxins;

                for (int j = 0; j < agentAuxins.Count; j++)
                {
                    _agents[i]._distAuxin.Add(agentAuxins[j].Position - _agents[i].transform.position);
                    Debug.DrawLine(agentAuxins[j].Position, _agents[i].transform.position, Color.green);
                }

                _agents[i].CalculateDirection();
                _agents[i].CalculateVelocity();
                _agents[i].CalculateAverage();
                _agents[i].Step();


            }


            // lines 315-333 exports the csv file
            // if (Array.Exists<Agent>(_agents.ToArray(), x => x._arrivedAtGoal))
            // {
            //     WriteToFile();
            //     Debug.Log("Write");
            // }

            Frame++;

        }

        public static implicit operator World(NoPlayerWorld v)
        {
            throw new NotImplementedException();
        }

        //private void WriteToFile()
        //{
        //    string content = "";
        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        content += _agents[i].AverageSpeed + ";\n";
        //    }
        //    System.IO.File.WriteAllText(Application.dataPath + @"/averagespeed.txt ", content);
        //}
    }
}