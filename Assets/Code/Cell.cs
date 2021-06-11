/// ---------------------------------------------
/// Contact: Henry Braun
/// Brief: Defines an Cell
/// Thanks to VHLab for original implementation
/// Date: November 2017 
/// ---------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;

namespace Biocrowds.Core
{
    public class Cell : MonoBehaviour
    {
        public int X;
        public int Z;

        private List<Auxin> _auxins = new List<Auxin>();
        public NativeList<float3> _auxinsPositions = new NativeList<float3>(0, Allocator.Persistent);

        public List<Auxin> Auxins
        {
            get { return _auxins; }
            set { _auxins = value; }
        }

        private void OnDisable()
        {
            _auxinsPositions.Dispose();
        }
    }
}