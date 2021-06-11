using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biocrowds.Core
{

    [CreateAssetMenu]
    public class Experiment : ScriptableObject
    {
        public enum Interaction
        {
            None, PlayerBiocrowds, PlayerNormalLife
        }

        public Interaction InteractionType;

        public Agent agentePrefab;


    }
}