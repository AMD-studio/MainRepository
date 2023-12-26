using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static Assets.Scripts.Mechanics.EntityModule.EnemyAI;

namespace Assets.Scripts.Mechanics.EntityModule
{
    [RequireComponent(typeof(NavMeshAgent), typeof(SphereCollider))]
    public class EnemySight : MonoBehaviour
    {
        public enum Sensitivity { Strict, Loose }

        public Sensitivity sensitivity = Sensitivity.Strict;
        public Transform[] wayPoints;
        public Transform player;
        public Transform eyePoint;
        public float sightRange = 45f;
        public float radius = 10.0f;

        public float health = 100f;

        private NavMeshAgent navAgent;
        private Transform thisTransform;
        private SphereCollider sphereCollider;
        public Vector3 LastKnowSighting;
        private int waypointIndex = 0;
        public bool canSeePlayer;
        [HideInInspector] Animator animator;
        public bool IsDead = false;
        public GameObject prefab;
        public GameObject DiePrefab;

        private void Awake()
        {
            thisTransform = transform;
            navAgent = GetComponent<NavMeshAgent>();
            sphereCollider = GetComponent<SphereCollider>();
            animator = GetComponent<Animator>();
            LastKnowSighting = thisTransform.position;
        }

        private void Update()
        {
            if (IsPlayerNear())
            {
                Debug.Log("Player is near. Last sighting: " + UpdateLastSighting(player.position));
            }

            if (navAgent.remainingDistance < 0.5f)
            {
                NextWayPoint();
            }
            Die();
            UpdateAnimatorSpeed();

            if (navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                Vector3 directionToPlayer = player.position - thisTransform.position;
                Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = rotation;

                Debug.Log("Player is within stopping distance. Rotating towards player.");

                animator.SetBool("Atack", true);
            }
            else
            {
                animator.SetBool("Atack", false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Arrow"))
            {   
                if (collision.gameObject.TryGetComponent<Arrow>(out var arrow))
                {
                    float damage = arrow.GetDamage();

                    Debug.Log($"Applying damage {damage} to target.");

                    health -= damage;
                }
                else
                {
                    Debug.LogWarning("Arrow component not found on collided object.");
                }
            }
            else
            {
                Debug.LogWarning("Collision with object not tagged as 'Arrow'.");
            }
        }

        private void Die()
        {
            if (health <= 0)
            {
                Destroy(prefab);
                Instantiate(DiePrefab, thisTransform.position, thisTransform.rotation);
            }
        }

        private void UpdateAnimatorSpeed()
        {
            float currentSpeed = navAgent.velocity.magnitude;
            float maxSpeed = navAgent.speed;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / maxSpeed);
            animator.SetFloat("Speed", normalizedSpeed);
        }

        private bool IsPlayerNear()
        {
            float distanceToPlayer = Vector3.Distance(player.position, thisTransform.position);
            canSeePlayer = distanceToPlayer < radius;
            return canSeePlayer;
        }

        private void NextWayPoint()
        {
            if (wayPoints.Length == 0)
                return;

            SetNextWaypointDestination();
        }

        private void SetNextWaypointDestination()
        {
            navAgent.destination = wayPoints[waypointIndex].position;
            waypointIndex = (waypointIndex + 1) % wayPoints.Length;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CheckObjectSensitivity();
            }
        }

        private void OnTriggerExit()
        {
            canSeePlayer = false;
        }

        private bool EnemyFieldOfView()
        {
            Vector3 directionToPlayer = player.position - eyePoint.position;
            float angle = Vector3.Angle(eyePoint.forward, directionToPlayer);

            return angle <= sightRange;
        }

        private bool ClearView()
        {
            RaycastHit hit;

            if (Physics.Raycast(eyePoint.position, (player.position - eyePoint.position).normalized, out hit, sphereCollider.radius))
            {
                Debug.DrawLine(eyePoint.position, hit.point, Color.green);
                Debug.DrawLine(hit.point, hit.point + hit.normal * 0.2f, Color.red);

                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckObjectSensitivity()
        {
            canSeePlayer = sensitivity switch
            {
                Sensitivity.Strict => EnemyFieldOfView() && ClearView(),
                Sensitivity.Loose => EnemyFieldOfView() || ClearView(),
                _ => false,
            };
        }

        private Vector3 UpdateLastSighting(Vector3 newSighting)
        {
            LastKnowSighting = newSighting;
            return LastKnowSighting;
        }
    }
}