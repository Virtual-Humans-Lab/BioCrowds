/// ---------------------------------------------
/// Contact: Henry Braun
/// Brief: Defines an Agent
/// Thanks to VHLab for original implementation
/// Date: November 2017 
/// ---------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
using UnityEngine.Profiling;

namespace Biocrowds.Core
{
    public class Agent : MonoBehaviour
    {
        protected const float UPDATE_NAVMESH_INTERVAL = 0.1f;

        //agent radius
        protected float agentRadius { get; private set; } = 1f;

        //agent speed
        protected Vector3 _velocity { get; private set; }

        //max speed
        [SerializeField]
        protected float _maxSpeed = 1.5f;

        //goal
        public GameObject Goal;

        [SerializeField]
        protected Transform agentCheckTransform;

        //list with all auxins in his personal space
        protected List<Auxin> _auxins = new List<Auxin>();
        public List<Auxin> Auxins
        {
            get { return _auxins; }
            set { _auxins = value; }
        }

        //agent cell
        protected Cell _currentCell;
        public Cell CurrentCell
        {
            get { return _currentCell; }
            set { _currentCell = value; }
        }

        protected World _world;
      

        protected int _totalX;
        protected int _totalZ;

        protected NavMeshPath _navMeshPath;

        //time elapsed (to calculate path just between an interval of time)
        protected float _elapsedTime;

        //acumulates the velocity that the agent prints
        protected float _velocityAcummulator;

        //final agent velocity
        public float AverageSpeed { get; protected set; }

        //agents last position
        protected Vector3 _lastPos;

        //threshold that surronds the goal and defines an area where the agent changes his state to reached goal
        protected float _goalThreshold = 6f;

        //defines that the original state of agents is not at goal
        protected bool _arrivedAtGoal { get; set; } = false;



        //auxins distance vector from agent
        public List<Vector3> _distAuxin;

        /*-----------Paravisis' model-----------*/
        protected bool _isDenW = false; //  avoid recalculation
        protected float _denW;    //  avoid recalculation
        public Vector3 _rotation { get; protected set; } //orientation vector (movement)
        public Vector3 _goalPosition; //goal position
        protected Vector3 _dirAgentGoal; //diff between goal and agent



        protected void Start()
        {
            _navMeshPath = new NavMeshPath();

            _world = World.instance;

            _goalPosition = Goal.transform.position; // defines the goal as the goal tag position
            _dirAgentGoal = _goalPosition - transform.position;

            //cache world info
            _totalX = Mathf.FloorToInt(_world.Dimension.x / 2.0f) - 1;
            _totalZ = Mathf.FloorToInt(_world.Dimension.y / 2.0f);

            _lastPos = transform.position;
        }

        protected void Update()
        {
            //clear agent´s information
            ClearAgent();

            // Update the way to the goal every second.
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > UPDATE_NAVMESH_INTERVAL)
            {
                _elapsedTime = 0.0f;

                //calculate agent path
                bool foundPath = NavMesh.CalculatePath(transform.position, Goal.transform.position, NavMesh.AllAreas, _navMeshPath);




                //update its goal if path is found
                if (foundPath)
                {
                    _goalPosition = new Vector3(_navMeshPath.corners[1].x, 0f, _navMeshPath.corners[1].z);
                    _dirAgentGoal = _goalPosition - transform.position;
                }
               


                // defines what happens when the agent reaches the goal`s threshold
                if (Vector3.Distance(transform.position, Goal.transform.position) < _goalThreshold && !_arrivedAtGoal)
                { 
                    //calculates the average speed based on the acummulated velocity / world frames converted to meters            
                    AverageSpeed = _velocityAcummulator / (_world.Frame * _world.SIMULATION_STEP);

                    //Debug.Log("Agent" + gameObject.name + " arrived at goal with average speed of " + AverageSpeed + " m/s" );
                    //changes the agent`s status to arrived at goal
                    _arrivedAtGoal = true; 
                }

            }

            //draw line to goal
            //for (int i = 0; i < _navMeshPath.corners.Length - 1; i++)
            //    Debug.DrawLine(_navMeshPath.corners[i], _navMeshPath.corners[i + 1], Color.red);
        }


        //function that defines the average
        public void CalculateAverage()
        {
            var delta = (transform.position - _lastPos).magnitude;
            //defines the distance delta

            _velocityAcummulator = _velocityAcummulator + delta;
            //acummulates the velocity of the agent 
            _lastPos = transform.position;
            //defines the agent`s last position
        }


        //clear agent´s informations
        public void ClearAgent()
        {
            //re-set inicial values
            
            _denW = 0;
            _distAuxin.Clear();
            _isDenW = false;
            _rotation = Vector3.zero;
            _dirAgentGoal = _goalPosition - transform.position;
        }

        //walk
        public void Step()
        {
            if (_velocity.sqrMagnitude > float.Epsilon)
                transform.Translate(_velocity * _world.SIMULATION_STEP, Space.World);

           
        }

        //The calculation formula starts here
        //the ideia is to find m=SUM[k=1 to n](Wk*Dk)
        //where k iterates between 1 and n (number of auxins), Dk is the vector to the k auxin and Wk is the weight of k auxin
        //the weight (Wk) is based on the degree resulting between the goal vector and the auxin vector (Dk), and the
        //distance of the auxin from the agent
        public void CalculateDirection()
        {


            //for each agent´s auxin
            for (int k = 0; k < _distAuxin.Count; k++)
            {
                //calculate W
                float valorW = CalculaW(k);
                if (_denW < 0.0001f)
                    valorW = 0.0f;

                //sum the resulting vector * weight (Wk*Dk)
                _rotation += valorW * _distAuxin[k] * _maxSpeed;
            }


        }

        //calculate W
        protected virtual float CalculaW(int indiceRelacao)
        {
            //calculate F (F is part of weight formula)
            float fVal = GetF(indiceRelacao);

            if (!_isDenW)
            {
                _denW = 0f;

                //for each agent´s auxin
                for (int k = 0; k < _distAuxin.Count; k++)
                {
                    //calculate F for this k index, and sum up
                    _denW += GetF(k);
                }
                _isDenW = true;
            }

            //Debug.Log(gameObject.name);

       

            return (fVal / _denW);
        }

        //calculate F (F is part of weight formula)
        float GetF(int pRelationIndex)
        {
            //distance between auxin´s distance and origin 
            float Ymodule = Vector3.Distance(_distAuxin[pRelationIndex], Vector3.zero);
            //distance between goal vector and origin
            float Xmodule = _dirAgentGoal.normalized.magnitude;

            float dot = Vector3.Dot(_distAuxin[pRelationIndex], _dirAgentGoal.normalized);

            if (Ymodule < 0.00001f)
                return 0.0f;

            //return the formula, defined in thesis
            return (float)((1.0 / (1.0 + Ymodule)) * (1.0 + ((dot) / (Xmodule * Ymodule))));
        }

        //calculate speed vector    
        public void CalculateVelocity()
        {
            //distance between movement vector and origin
            float moduleM = Vector3.Distance(_rotation, Vector3.zero);

            //multiply for PI
            float s = moduleM * Mathf.PI;

            //if it is bigger than maxSpeed, use maxSpeed instead
            if (s > _maxSpeed)
                s = _maxSpeed;

            //Debug.Log("vetor M: " + m + " -- modulo M: " + s);
            if (moduleM > 0.0001f)
            {
                //calculate speed vector
                _velocity = s * (_rotation / moduleM);
                //Console.WriteLine(_velocity);
            }
            else
            {
                //else, go idle
                _velocity = Vector3.zero;
            }

            //if (gameObject.tag == "Player")
            //{
            //    Debug.Log(_velocity);
            //}
        }

        //find all auxins near him (Voronoi Diagram)
        //call this method from game controller, to make it sequential for each agent
        public void FindNearAuxins()
        {
            //clear them all, for obvious reasons
            _auxins.Clear();

            //get all auxins on my cell
            List<Auxin> cellAuxins = _currentCell.Auxins;

            //iterate all cell auxins to check distance between auxins and agent
            for (int i = 0; i < cellAuxins.Count; i++)
            {
                //see if the distance between this agent and this auxin is smaller than the actual value, and inside agent radius
                float distanceSqr = (transform.position - cellAuxins[i].Position).sqrMagnitude;
                if (DistanceMetric(distanceSqr, cellAuxins[i]) && distanceSqr <= agentRadius * agentRadius)
                {
                    //take the auxin!
                    //if this auxin already was taken, need to remove it from the agent who had it
                    if (cellAuxins[i].IsTaken)
                        cellAuxins[i].Agent.Auxins.Remove(cellAuxins[i]);

                    //auxin is taken
                    cellAuxins[i].IsTaken = true;
                    //auxin has agent
                    cellAuxins[i].Agent = this;
                    //update min distance
                    cellAuxins[i].MinDistance = distanceSqr;
                    //update my auxins
                    _auxins.Add(cellAuxins[i]);
                }
            }

            FindCell();
        }

        protected void FindCell()
        {
            //distance from agent to cell, to define agent new cell
            float distanceToCellSqr = (transform.position - _currentCell.transform.position).sqrMagnitude; //Vector3.Distance(transform.position, _currentCell.transform.position);

            //cap the limits
            //[ ][ ][ ]
            //[ ][X][ ]
            //[ ][ ][ ]
            if (_currentCell.X > 0)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X - 1) * _totalZ + (_currentCell.Z + 0)]);

            if (_currentCell.X > 0 && _currentCell.Z < _totalZ - 1)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X - 1) * _totalZ + (_currentCell.Z + 1)]);

            if (_currentCell.X > 0 && _currentCell.Z > 0)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X - 1) * _totalZ + (_currentCell.Z - 1)]);

            if (_currentCell.Z < _totalZ - 1)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X + 0) * _totalZ + (_currentCell.Z + 1)]);

            if (_currentCell.X < _totalX && _currentCell.Z < _totalZ - 1)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X + 1) * _totalZ + (_currentCell.Z + 1)]);

            if (_currentCell.X < _totalX)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X + 1) * _totalZ + (_currentCell.Z + 0)]);

            if (_currentCell.X < _totalX && _currentCell.Z > 0)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X + 1) * _totalZ + (_currentCell.Z - 1)]);

            if (_currentCell.Z > 0)
                CheckAuxins(ref distanceToCellSqr, _world.Cells[(_currentCell.X + 0) * _totalZ + (_currentCell.Z - 1)]);

        }

        protected virtual bool DistanceMetric(float agent, Auxin auxin)
        {
            return agent < auxin.MinDistance;
        }

        protected void CheckAuxins(ref float pDistToCellSqr, Cell pCell)
        {
            //get all auxins on neighbourcell
            List<Auxin> cellAuxins = pCell.Auxins;

            //iterate all cell auxins to check distance between auxins and agent
            for (int c = 0; c < cellAuxins.Count; c++)
            {
                //see if the distance between this agent and this auxin is smaller than the actual value, and smaller than agent radius
                float distanceSqr = (transform.position - cellAuxins[c].Position).sqrMagnitude;
                if (DistanceMetric(distanceSqr, cellAuxins[c]) && distanceSqr <= agentRadius * agentRadius)
                {
                    //take the auxin
                    //if this auxin already was taken, need to remove it from the agent who had it
                    if (cellAuxins[c].IsTaken)
                        cellAuxins[c].Agent.Auxins.Remove(cellAuxins[c]);

                    //auxin is taken
                    cellAuxins[c].IsTaken = true;
                    //auxin has agent
                    cellAuxins[c].Agent = this;
                    //update min distance
                    cellAuxins[c].MinDistance = distanceSqr;
                    //update my auxins
                    _auxins.Add(cellAuxins[c]);
                }
            }

            //see distance to this cell
            float distanceToNeighbourCell = (transform.position - pCell.transform.position).sqrMagnitude;
            if (distanceToNeighbourCell < pDistToCellSqr)
            {
                pDistToCellSqr = distanceToNeighbourCell;

                //TODO: change the way we find the next cell, find it by the world position
                _currentCell = pCell;
            }
        }
    }
}