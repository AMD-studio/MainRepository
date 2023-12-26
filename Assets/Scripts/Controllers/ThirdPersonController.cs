

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
                        bowScript.Fire(hit.point);
                    }
                    else
                    {
                        bowScript.Fire(ray.GetPoint(300f));
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


        void RotateToCamView()
        {
            Vector3 camCenterPos = camCenter.position;

            Vector3 lookPoint = camCenterPos + (camCenter.forward * lookDIstance);
            Vector3 direction = lookPoint - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.x = 0;
            lookRotation.z = 0;

            Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
            transform.rotation = finalRotation;
        }

        void RotateCharacterSpine()
        {
            RotateToCamView();
            spine.LookAt(ray.GetPoint(50));
            spine.Rotate(spineOffset);
        }

        //Does the aiming and sends a raycast to a target
        void Aim()
        {
            Vector3 camPosition = mainCamera.position;
            Vector3 dir = mainCamera.forward;
            RotateCharacterSpine();

            ray = new Ray(camPosition, dir);
            if (Physics.Raycast(ray, out hit, 500f, aimLayers))
            {
                state.hitDetected = true;
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                bowScript.ShowCrosshair(hit.point);
            }
            else
            {
                state.hitDetected = false;
                bowScript.RemoveCrosshair();
            }
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