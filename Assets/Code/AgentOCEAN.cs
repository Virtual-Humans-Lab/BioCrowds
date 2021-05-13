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
        [Range(0f, 1f)] public float Extraversion;
        [Range(0f, 1f)] public float Aggreableness;
        [Range(0f, 1f)] public float Neuroticism;
    }

    public class AgentOCEAN : Core.Agent
    {


        [SerializeField] private OCEAN _emotionProfile;



        // Start is called before the first frame update
        void Start()
        {
            base.Start();

            _emotionProfile.Extraversion = Random.Range(0.8f, 1f);

        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
        }


        private float CalculateExtraversionFactor()
        {
            return _emotionProfile.Extraversion;
        }

        protected override float CalculaW(int indiceRelacao)
        {
            var parcialW = base.CalculaW(indiceRelacao);

            var extraversionFactor = CalculateExtraversionFactor();

            return (parcialW * extraversionFactor) + (1 - extraversionFactor);


        }
            

    }

}
