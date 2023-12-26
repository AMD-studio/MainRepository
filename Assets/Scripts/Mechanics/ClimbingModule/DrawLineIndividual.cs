using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    [ExecuteInEditMode]
    public class DrawLineIndividual : MonoBehaviour
    {
        public List<Neighbour> ConnectedPoints = new();

        public bool refresh;

        void Update()
        {
            if (!refresh)
                return;

            ConnectedPoints.Clear();
            refresh = false;
        }
    }

}
