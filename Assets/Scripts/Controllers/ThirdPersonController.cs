

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Climbing
{
    public class ThirdPersonState
    {
        [HideInInspector] public bool isGrounded = false;
        [HideInInspector] public bool isAllowMovement = true;
        [HideInInspector] public bool isOnAir = false;
        [HideInInspector] public bool isJumping = false;
        [HideInInspector] public bool inSlope = false;
        [HideInInspector] public bool isVaulting = false;
        [HideInInspector] public bool isDummy = false;
        [HideInInspector] public bool isAiming = false;
        [HideInInspector] public bool testAim = false;
        [HideInInspector] public bool hitDetected = false;
    }

    [RequireComponent(typeof(InputCharacterController))]
    [RequireComponent(typeof(MovementCharacterController))]
    [RequireComponent(typeof(AnimationCharacterController))]
    [RequireComponent(typeof(DetectionCharacterController))]
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(VaultingController))]

    public class ThirdPersonController : MonoBehaviour
    {
        [HideInInspector] public InputCharacterController characterInput;
        [HideInInspector] public MovementCharacterController characterMovement;
        [HideInInspector] public AnimationCharacterController characterAnimation;
        [HideInInspector] public DetectionCharacterController characterDetection;
        [HideInInspector] public VaultingController vaultingController;
        [HideInInspector] public ThirdPersonState state;

        [Header("Colliders")]
        public CapsuleCollider normalCapsuleCollider;
        public CapsuleCollider slidingCapsuleCollider;

        [Header("Cameras")]
        public CameraController cameraController;
        public Transform mainCamera;
        public Transform freeCamera;
        Transform camCenter;

        [Header("Step Settings")]
        [Range(0, 10.0f)] public float stepHeight = 0.8f;
        public float stepVelocity = 0.2f;

        private float turnSmoothTime = 0.1f;
        private float turnSmoothVelocity;

        [Header("Camera & Character Syncing")]
        public float lookDIstance = 5;
        public float lookSpeed = 5;

        [Header("Aiming Settings")]
        RaycastHit hit;
        public LayerMask aimLayers;
        Ray ray;

        [Header("Spine Settings")]
        public Transform spine;
        public Vector3 spineOffset;

        [Header("Head Rotation Settings")]
        public float lookAtPoint = 2.8f;

        [Header("Gravity Settings")]
        public float gravityValue = 1.2f;

        public Bow bowScript;


        private void Awake()
        {
            state = new ThirdPersonState();

            characterInput = GetComponent<InputCharacterController>();
            characterMovement = GetComponent<MovementCharacterController>();
            characterAnimation = GetComponent<AnimationCharacterController>();
            characterDetection = GetComponent<DetectionCharacterController>();
            vaultingController = GetComponent<VaultingController>();
            camCenter = Camera.main.transform.parent;

            if (cameraController == null)
                Debug.LogError("Attach the Camera Controller located in the Free Look Camera");
        }

        private void Start()
        {
            characterMovement.OnLanded += characterAnimation.Land;
            characterMovement.OnFall += characterAnimation.Fall;
        }

        void Update()
        {
            Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);

            //Detect if Player is on Ground
            state.isGrounded = OnGround();

            GetShoot();

            //Get Input if controller and movement are not disabled
            if (!state.isDummy && state.isAllowMovement)
            {
                AddMovementInput(characterInput.movement);

                //Detects if Joystick is being pushed hard
                if (characterInput.run && characterInput.movement.magnitude > 0.5f)
                {
                    ToggleRun();
                }
                else if (!characterInput.run)
                {
                    ToggleWalk();
                }
            }
        }

        void GetShoot()
        {
            state.isAiming = Mouse.current.rightButton.isPressed;

            if (state.testAim)
                state.isAiming = true;

            if (bowScript.bowSettings.arrowCount < 1)
                state.isAiming = false;

            if (state.isAiming)
            {
                characterAnimation.CharacterAim(true);

                Aim();
                bowScript.EquipBow();

                if (bowScript.bowSettings.arrowCount > 0)
                    characterAnimation.CharacterPullString(Mouse.current.leftButton.isPressed);

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    characterAnimation.CharacterFireArrow();
                    if (state.hitDetected)
                    {
                        Vector3 dir = transform.forward;
                        bowScript.Fire(dir);
                    }
                    else
                    {
                        Vector3 dir = transform.forward * 300f;
                        bowScript.Fire(dir);
                    }

                }
            }
            else
            {
                characterAnimation.CharacterAim(false);
                bowScript.UnEquipBow();
                bowScript.RemoveCrosshair();
                DisableArrow();
                Release();
            }
        }

        void RotateToNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                // Если нет врагов, не поворачиваем персонажа
                return;
            }

            GameObject nearestEnemy = GetNearestEnemy(enemies);

            if (nearestEnemy != null)
            {
                Vector3 enemyDirection = nearestEnemy.transform.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(enemyDirection);
                lookRotation.x = 0;
                lookRotation.z = 0;

                Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
                transform.rotation = finalRotation;
            }
        }

        GameObject GetNearestEnemy(GameObject[] enemies)
        {
            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        void RotateCharacterSpine()
        {
            RotateToNearestEnemy();
            spine.LookAt(ray.GetPoint(50));
            spine.Rotate(spineOffset);
        }

        // Does the aiming and sends a raycast to a target
        void Aim()
        {
            Vector3 camPosition = mainCamera.position;
            Vector3 dir = mainCamera.forward;
            RotateCharacterSpine();

            ray = new Ray(camPosition, dir);
            DebugDrawAimSphere(ray.origin, 20f, Color.cyan);

            // Вместо Physics.Raycast используем SphereCast, чтобы найти противников в определенном радиусе
            RaycastHit[] hits = Physics.SphereCastAll(ray, 20f, 20f, aimLayers);

            if (hits.Length > 0)
            {
                // Находим ближайший противник из всех попавших в сферу
                state.hitDetected = true;
                GameObject nearestEnemy = GetNearestEnemy(hits);
                Debug.DrawLine(ray.origin, nearestEnemy.transform.position, Color.green);
                bowScript.ShowCrosshair(nearestEnemy.transform.position);
            }
            else
            {
                state.hitDetected = false;
                bowScript.RemoveCrosshair();
            }
        }
        void DebugDrawAimSphere(Vector3 center, float radius, Color color)
        {
            Debug.DrawRay(center + Vector3.up * radius, Vector3.forward * radius, color);
            Debug.DrawRay(center - Vector3.up * radius, Vector3.forward * radius, color);
            Debug.DrawRay(center - Vector3.right * radius, Vector3.up * radius * 2, color);
            Debug.DrawRay(center + Vector3.right * radius, Vector3.up * radius * 2, color);
            Debug.DrawRay(center - Vector3.forward * radius, Vector3.right * radius * 2, color);
            Debug.DrawRay(center + Vector3.forward * radius, Vector3.right * radius * 2, color);

            float angleStep = 10f;
            for (float angle = 0; angle < 360; angle += angleStep)
            {
                float x = center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float y = center.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                Vector3 point = new Vector3(x, y, center.z);
                Debug.DrawLine(point, point + Vector3.forward * radius, color);
            }
        }


        // Определяем ближайший объект из массива RaycastHit
        GameObject GetNearestEnemy(RaycastHit[] hits)
        {
            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                float distance = hit.distance;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = hit.collider.gameObject;
                }
            }

            return nearestEnemy;
        }

        public void Pull()
        {
            bowScript.PullString();
        }

        public void EnableArrow()
        {
            bowScript.PickArrow();
        }

        public void DisableArrow()
        {
            bowScript.DisableArrow();
        }

        public void Release()
        {
            bowScript.ReleaseString();
        }

        public void PlayPullSound()
        {
            bowScript.PullAudio();
        }

        private bool OnGround()
        {
            return characterDetection.IsGrounded(stepHeight);
        }

        public void AddMovementInput(Vector2 direction)
        {
            Vector3 translation = Vector3.zero;

            translation = GroundMovement(direction);

            characterMovement.SetVelocity(Vector3.ClampMagnitude(translation, 1.0f));
        }

        Vector3 GroundMovement(Vector2 input)
        {
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            //Gets direction of movement relative to the camera rotation
            freeCamera.eulerAngles = new Vector3(0, mainCamera.eulerAngles.y, 0);
            Vector3 translation = freeCamera.transform.forward * input.y + freeCamera.transform.right * input.x;
            translation.y = 0;

            //Detects if player is moving to any direction
            if (translation.magnitude > 0)
            {
                RotatePlayer(direction);
                characterAnimation.animator.SetBool("Released", false);
            }
            else
            {
                ToggleWalk();
                characterAnimation.animator.SetBool("Released", true);
            }

            return translation;
        }

        public void RotatePlayer(Vector3 direction)
        {
            //Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            //Rotate Mesh to Movement
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        public Quaternion RotateToCameraDirection(Vector3 direction)
        {
            //Get direction with camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

            //Rotate Mesh to Movement
            return Quaternion.Euler(0f, targetAngle, 0f);
        }

        public void ResetMovement()
        {
            characterMovement.ResetSpeed();
        }

        public void ToggleRun()
        {
            if (characterMovement.GetState() != MovementState.Running)
            {
                characterMovement.SetCurrentState(MovementState.Running);
                characterMovement.curSpeed = characterMovement.RunSpeed;
                characterAnimation.animator.SetBool("Run", true);
            }
        }
        public void ToggleWalk()
        {
            if (characterMovement.GetState() != MovementState.Walking)
            {
                characterMovement.SetCurrentState(MovementState.Walking);
                characterMovement.curSpeed = characterMovement.walkSpeed;
                characterAnimation.animator.SetBool("Run", false);
            }
        }


        public float GetCurrentVelocity()
        {
            return characterMovement.GetVelocity().magnitude;
        }

        public void DisableController()
        {
            characterMovement.SetKinematic(true);
            characterMovement.enableFeetIK = false;
            state.isDummy = true;
            state.isAllowMovement = false;
        }
        public void EnableController()
        {
            characterMovement.SetKinematic(false);
            characterMovement.EnableFeetIK();
            characterMovement.ApplyGravity();
            characterMovement.stopMotion = false;
            state.isDummy = false;
            state.isAllowMovement = true;
        }
    }
}