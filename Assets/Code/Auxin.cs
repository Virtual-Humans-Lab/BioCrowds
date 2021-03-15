/// ---------------------------------------------
/// Contact: Henry Braun
/// Brief: Defines an Auxin
/// Thanks to VHLab for original implementation
/// Date: November 2017 
/// ---------------------------------------------

using UnityEngine;
using System.Collections;

namespace Biocrowds.Core
{
    public class Auxin : MonoBehaviour
    {
        

        //is auxin taken?
        private bool _isTaken = false;
        public bool IsTaken
        {
            get { return _isTaken; }
            set { _isTaken = value; }
        }

        //position
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                transform.position = _position;
            }
        }

        //min distance from a taken agent
        //when a new agent find it in his personal space, test the distance with this value to see which one is smaller
        private float _minDistance = 2.0f;

        public float MinDistance
        {
            get { return _minDistance; }
            set { _minDistance = value; }
        }

        //agent who took this auxin
        private Agent _agent;
        public Agent Agent
        {
            get { return _agent; }
            set { _agent = value; }
        }

        //cell who has this auxin
        private Cell _cell;
        public Cell Cell
        {
            get { return _cell; }
            set { _cell = value; }
        }

        //Reset auxin to his default state, for each update
        public void ResetAuxin()
        {
            _minDistance = 2.0f;
            _agent = null;
            _isTaken = false;
        }

        //public void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawSphere(_position, 0.075f);
        //}
    }
}