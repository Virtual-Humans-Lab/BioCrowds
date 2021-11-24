using Biocrowds.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Biocrowds.Emotion
{
    public class FacialAnimation : AgentOCEAN
    {

        private Animator _animator;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var agents = World.instance._agents;
            

            for(int i = 0; i <agents.Count; i++)
            {
         
            }
        }
    }
}
