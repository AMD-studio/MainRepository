using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Climbing
{
    [ExecuteInEditMode]
    public class HandlePointConnection : MonoBehaviour
    {
        public float maxDistance = 1.5f;
        public float minDistance = 0.5f;
        public bool updateConnections = true;
        public bool resetConnections;

        private readonly List<Point> allPoints = new();

        private readonly Dictionary<int, Vector3> availableDirections = new Dictionary<int, Vector3>
        {
            { 0, new Vector3(1, 0, 0) },
            { 1, new Vector3(-1, 0, 0) },
            { 2, new Vector3(0, 1, 0) },
            { 3, new Vector3(0, -1, 0) },
            { 4, new Vector3(-1, -1, 0) },
            { 5, new Vector3(1, 1, 0) },
            { 6, new Vector3(1, -1, 0) },
            { 7, new Vector3(-1, 1, 0) }
        };

        public float validAngleRange = 22.5f;

        private void Update()
        {
            if (updateConnections)
            {
                GetPoints();
                CreateConnections();
                RefreshAll();
                updateConnections = false;
            }

            if (resetConnections)
            {
                ResetConnections();
                resetConnections = false;
            }
        }

        private void GetPoints()
        {
            allPoints.Clear();
            allPoints.AddRange(GetComponentsInChildren<Point>());
        }

        private void CreateConnections()
        {
            foreach (Point from in allPoints)
            {
                CandidatePointsOnDirection(from);
            }
        }

        private void ResetConnections()
        {
            foreach (Point point in allPoints)
            {
                point.neighbours.Clear();
            }
            RefreshAll();
        }

        private void CandidatePointsOnDirection(Point from)
        {
            foreach (Point target in allPoints)
            {
                if (from == target)
                    continue;

                float distance = Vector3.Distance(from.transform.position, target.transform.position);

                if (distance < maxDistance && distance > minDistance)
                {
                    Vector3 direction = target.transform.position - from.transform.position;
                    Vector3 relativeDirection = from.transform.InverseTransformDirection(direction);
                    relativeDirection.z = 0;

                    foreach (var kvp in availableDirections)
                    {
                        if (IsDirectionValid(kvp.Value, relativeDirection))
                        {
                            AddNeighbour(from, target, kvp.Value);
                        }
                    }
                }
            }
        }

        private float GetAtan2D(Vector3 vector)
        {
            return Mathf.Atan2(vector.x, vector.y);
        }

        private bool IsDirectionValid(Vector3 targetDirection, Vector3 candidate)
        {
            float targetAngle = GetAtan2D(targetDirection) * Mathf.Rad2Deg;
            float angle = GetAtan2D(candidate) * Mathf.Rad2Deg;

            targetAngle = (targetAngle + 360) % 360; // Normalize angles

            return Mathf.Abs(targetAngle - angle) <= validAngleRange;
        }

        private void AddNeighbour(Point from, Point target, Vector3 direction)
        {
            from.neighbours.Add(new Neighbour { target = target, direction = direction });

#if UNITY_EDITOR
            EditorUtility.SetDirty(from);
#endif
        }

        private void RefreshAll()
        {
            if (transform.TryGetComponent<DrawLine>(out var dl))
                dl.refresh = true;
        }

        public List<Connection> GetAllConnections()
        {
            List<Connection> connections = new();
            HashSet<(Point, Point)> uniqueConnections = new();

            foreach (Point point in allPoints)
            {
                foreach (Neighbour neighbour in point.neighbours)
                {
                    Point target = neighbour.target;
                    if (uniqueConnections.Add((point, target)) || uniqueConnections.Add((target, point)))
                    {
                        connections.Add(new Connection { target1 = point, target2 = target });
                    }
                }
            }

            return connections;
        }

        private bool ContainsConnection(List<Connection> connections, Point target1, Point target2)
        {
            return connections.Any(c => AreConnectionsEqual(c, target1, target2));
        }

        private bool AreConnectionsEqual(Connection connection, Point target1, Point target2)
        {
            return (connection.target1 == target1 && connection.target2 == target2) ||
                   (connection.target1 == target2 && connection.target2 == target1);
        }
    }

    public class Connection
    {
        public Point target1;
        public Point target2;
    }
}
