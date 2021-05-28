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

        [SerializeField] private float maxAuxins;

        private float _confortFactor;

        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            maxAuxins = 75;
            _ws = new List<(int,float)>();

            _emotionProfile.Extraversion = Random.Range(0.1f, 1f);
            _emotionProfile.Neuroticism = 0f;

            //GetComponentInChildren<Renderer>().material.color = Color.red * _emotionProfile.Extraversion;

        }

        // Update is called once per frame
        void Update()
        {
            base.Update();

            /*_emotionProfile.Extraversion = Mathf.Clamp01(_emotionProfile.Extraversion + Random.Range(-0.1f,0.1f));*/


            _confortFactor = _auxins.Count / maxAuxins;

         
        }

        private float CalculateConfortFactor()
        {
            return Mathf.Sin(_confortFactor * (Mathf.PI / 2));
            //return _emotionProfile.Extraversion;
        }

        private float CalculateExtraversionFactor()
        {
            return 1 - _emotionProfile.Extraversion;
            //return _emotionProfile.Extraversion;
        }

        private float CalculateNeuritisismFactor()
        {
            return (1 - _emotionProfile.Neuroticism);
        }

        protected override bool DistanceTest(float agent, Core.Auxin auxin)
        {
            if (auxin.Agent as AgentOCEAN)
            {
                return agent * CalculateNeuritisismFactor() < auxin.MinDistance * (auxin.Agent as AgentOCEAN).CalculateNeuritisismFactor() && agent <= agentRadius * agentRadius;
            }
            else
            {
                return base.DistanceTest(agent, auxin);
            }


        }

        protected override float CalculaW(int indiceRelacao)
        {
            var parcialW = base.CalculaW(indiceRelacao);

            if (parcialW < 0.001f)
                return parcialW;

            var factor = CalculateConfortFactor();
            var extraversion = CalculateExtraversionFactor();

            float newW = (parcialW * factor * (1 + extraversion)) + ((1 - factor) * extraversion);

#if UNITY_EDITOR
            if(_debug)
                _ws.Add((indiceRelacao, newW));
#endif
            return newW;


        }

#if UNITY_EDITOR
        private List<(int, float)> _ws;
        private bool _debug = false;

        private void OnDrawGizmos()
        {
            if (!_debug)
                return;
            for (int i = 0; i < _ws.Count; i++)
            {
                GUIStyle style = new GUIStyle();
                UnityEditor.Handles.Label(_auxins[_ws[i].Item1].Position, _ws[i].Item2.ToString());
            }
            _ws.Clear();
        }

#endif

    }

}
