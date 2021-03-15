using Biocrowds.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Agent
{

    public CharacterController controller;

    public float speed = 12f;

    // Start is called before the first frame update
    void Start()
    {
        _world = GameObject.Find("WorldPrefab").GetComponent<World>();

        _totalX = Mathf.FloorToInt(_world.Dimension.x / 2.0f) - 1;
        _totalZ = Mathf.FloorToInt(_world.Dimension.y / 2.0f);


        Vector2 cellPostion = new Vector2((int)Mathf.Floor(transform.position.x / 2.0f) * 2,
                                     (int)Mathf.Floor(transform.position.z / 2.0f) * 2);

        //Debug.Log(cellPostion);
        _currentCell = World.posToCell[cellPostion];


    }

    // Update is called once per frame
    void Update()
    {

    

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * World.SIMULATION_STEP);


       
        //FindNearAuxins();

        //vector for each auxin
        // draws the player's auxins
        for (int j = 0; j < _auxins.Count; j++)
        {

            //just draw the lines to each auxin
            Debug.DrawLine(_auxins[j].Position, transform.position, Color.red);
        }

    }

    //find all auxins near him (Voronoi Diagram)
    //call this method from game controller, to make it sequential for each agent
    public new void FindNearAuxins()
    {
        //clear them all, for obvious reasons
        _auxins.Clear();

        //get all auxins on my cell
        List<Auxin> cellAuxins = _currentCell.Auxins;

        //Debug.Log(_currentCell.Auxins.Count);

        //iterate all cell auxins to check distance between auxins and agent
        for (int i = 0; i < cellAuxins.Count; i++)
        {
            //see if the distance between this agent and this auxin is smaller than the actual value, and inside agent radius
            float distanceSqr = (transform.position - cellAuxins[i].Position).sqrMagnitude;
            if (distanceSqr < cellAuxins[i].MinDistance && distanceSqr <= agentRadius * agentRadius)
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

    private new void FindCell()
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

    private new void CheckAuxins(ref float pDistToCellSqr, Cell pCell)
    {
        //get all auxins on neighbourcell
        List<Auxin> cellAuxins = pCell.Auxins;

        //iterate all cell auxins to check distance between auxins and agent
        for (int c = 0; c < cellAuxins.Count; c++)
        {
            //see if the distance between this agent and this auxin is smaller than the actual value, and smaller than agent radius
            float distanceSqr = (transform.position - cellAuxins[c].Position).sqrMagnitude;
            if (distanceSqr < cellAuxins[c].MinDistance && distanceSqr <= agentRadius * agentRadius)
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
            _currentCell = pCell;
        }
    }

}
