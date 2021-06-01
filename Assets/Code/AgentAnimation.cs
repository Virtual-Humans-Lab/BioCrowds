using Biocrowds.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Biocrowds.Animation
{
    public class AgentAnimation : MonoBehaviour
    {

        [SerializeField] private float _rotationSpeed;

        [SerializeField]
        private Queue<Vector3> _deltas;

        public List<Vector3> _deltaVisu;

        [SerializeField] private Vector3 _averageDirection;
        [SerializeField] private Vector3 _direction;

        private Vector3 _lastPos;

        private float _framesPerSecond;

        private Agent _agent;
        private Animator _animator;

        public bool _useAgentRotation { get; private set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            _framesPerSecond = Mathf.Pow(World.instance.SIMULATION_STEP, -1);

            _deltas = new Queue<Vector3>(Mathf.FloorToInt(_framesPerSecond));
            _agent = gameObject.GetComponent<Agent>();
            _animator = _agent.GetComponent<Animator>();


            for (int i = 0; i < _framesPerSecond; i++)
            {
                _deltas.Enqueue(Vector3.zero);
            }

            _deltaVisu = _deltas.ToList();

        }

        private void UpdateAnimationVelocity()
        {

            _animator.SetFloat("Speed", _direction.magnitude);
        }


        void UpdateDirection()
        {
            Vector3 targetDirection = _averageDirection.normalized;

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * World.instance.SIMULATION_STEP);

                transform.rotation = targetRotation;
            }

        }

        private void UpdateAnimationVariables()
        {
            _averageDirection = Vector3.zero;

            foreach (var item in _deltas)
            {
                _averageDirection += item;
            }

            _direction = _averageDirection;

            _averageDirection /= _deltas.Count;

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            Profiler.BeginSample("Animation");
#endif  

            if (_useAgentRotation)
            {
                if (_agent._rotation != Vector3.zero)
                    _deltas.Enqueue(_agent._rotation);
            }
            else
            {
                //if (transform.position - _lastPos != Vector3.zero)
                 _deltas.Enqueue(transform.position - _lastPos);
            }
            

            _deltaVisu = _deltas.ToList();

            

            if (_deltas.Count > _framesPerSecond)
            {
                _deltas.Dequeue();

                UpdateAnimationVariables();
                UpdateDirection();
                UpdateAnimationVelocity();

            }

            _lastPos = transform.position;

#if UNITY_EDITOR
            Profiler.EndSample();
#endif  

        }

       
    }
}

