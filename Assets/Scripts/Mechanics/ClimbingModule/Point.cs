using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Climbing
{
    [System.Serializable]
    public class Neighbour
    {
        public Vector3 direction;
        public Point target;
    }

    public enum PointType
    {
        Ledge = 0,
        Pole,
        Ground
    }

    [System.Serializable]
    public class Point : MonoBehaviour
    {
        public List<Neighbour> neighbours = new();
        public PointType type = PointType.Ledge;

        public Neighbour ReturnNeighbour(Point target)
        {
            return neighbours.FirstOrDefault(n => n.target == target);
        }
    }
}

