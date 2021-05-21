using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Biocrowds.Emotion
{
    [System.Serializable]
    public struct OCEAN
    {
        [Range(0f, 1f)] public float Openness;
        [Range(0f, 1f)] public float Conscientiousness;
        [Range(0f, 5f)] public float Extraversion;
        [Range(0f, 1f)] public float Aggreableness;
        public float Neuroticism;
    }

    public class AgentOCEAN : Core.Agent
    {
        [SerializeField] private OCEAN _emotionProfile;

        [SerializeField] private float maxAuxins;


        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            maxAuxins = 75;

            _emotionProfile.Neuroticism = 1f;
            // _emotionProfile.Extraversion = Random.Range(0.6f,1f);
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();

            /*_emotionProfile.Extraversion = Mathf.Clamp01(_emotionProfile.Extraversion + Random.Range(-0.1f,0.1f));*/


            _emotionProfile.Extraversion = _auxins.Count / maxAuxins;

         
        }

        private float CalculateExtraversionFactor()
        {
            return Mathf.Sin(_emotionProfile.Extraversion * (Mathf.PI / 2));
            //return _emotionProfile.Extraversion;
        }

        private float CalculateNeuritisismFactor()
        {
            return (_emotionProfile.Neuroticism);
        }

        protected override bool DistanceTest(float agent, Core.Auxin auxin)
        {
            if (auxin.Agent as AgentOCEAN)
            {
                return agent * CalculateNeuritisismFactor() < auxin.MinDistance * (auxin.Agent as AgentOCEAN).CalculateNeuritisismFactor();
            }
            else
            {
                return base.DistanceTest(agent, auxin);
            }


        }

        protected override float CalculaW(int indiceRelacao)
        {
            var parcialW = base.CalculaW(indiceRelacao);

            //if (parcialW < 0.001f)
            //    return parcialW;

            var extraversionFactor = CalculateExtraversionFactor();

            return (parcialW * extraversionFactor) + (1 - extraversionFactor);


        }


    }

}
