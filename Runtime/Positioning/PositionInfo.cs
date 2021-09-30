using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JelleKUL.MeshAlignment
{
    [System.Serializable]
    public class PositionInfo
    {
        public Vector3 position = Vector3.zero;
        public float errorRadius = 0f;
        public int coordinateSystem = 0;

    }
}
