using Biocrowds.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Biocrowds.Animation
{
    public class AgentAnimation : MonoBehaviour
    {

        [SerializeField]
        private Queue<Vector3> _deltas;

        public List<Vector3> _deltaVisu;

        private Vector3 _averageDirection;
        private Vector3 _lastPos;

        // Start is called before the first frame update
        void Start()
        {
            _deltas = new Queue<Vector3>(30);


            for (int i = 0; i < Mathf.Pow(World.instance.SIMULATION_STEP, -1); i++)
            {
                _deltas.Enqueue(Vector3.zero);
            }

            _deltaVisu = _deltas.ToList();

        }


        void UpdateDirection()
        {
            _averageDirection = Vector3.zero;

            foreach (var item in _deltas)
            {
                _averageDirection += item;
            }

            _averageDirection /= _deltas.Count;

            Vector3 targetDirection = _averageDirection.normalized;


            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 36f);
            }

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            Profiler.BeginSample("Animation");
#endif  
            if(transform.position - _lastPos != Vector3.zero)
                _deltas.Enqueue(transform.position - _lastPos);

            _deltaVisu = _deltas.ToList();


            if (_deltas.Count > Mathf.Pow(World.instance.SIMULATION_STEP, -1))
            {
                _deltas.Dequeue();

                UpdateDirection();

            }

            _lastPos = transform.position;

#if UNITY_EDITOR
            Profiler.EndSample();
#endif  

        }
    }
}

