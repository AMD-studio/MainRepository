using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    [ExecuteInEditMode]
    public class DrawLine : MonoBehaviour
    {
        public List<Connection> ConnectedPoints = new();

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
