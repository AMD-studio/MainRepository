using UnityEngine;

namespace Climbing
{
    public class VaultDown : VaultAction
    {
        private float timeDrop = 0;

        private VaultDown() { }

        public VaultDown(ThirdPersonController _vaultingController, Action _actionInfo = null) : base(_vaultingController)
        {
        }

        public override bool CheckAction()
        {
            if (controller.state.isGrounded && !controller.state.isJumping && !controller.state.isDummy)
            {
                //Checks if Player is in limit of a surface to Drop
                if (controller.characterMovement.limitMovement && controller.characterMovement.velLimit == 0 && timeDrop != -1 && !controller.state.isVaulting)
                {
                    timeDrop += Time.deltaTime;

                    //Checks if below surface is too low and denies auto drop
                    if (timeDrop > 0.15f)
                    {
                        Vector3 origin = controller.transform.position + controller.transform.forward * 1.0f;
                        Vector3 origin2 = controller.transform.position + Vector3.up * 0.1f;

                        RaycastHit hit;
                        if (!controller.characterDetection.ThrowRayOnDirection(origin, Vector3.down, 1.5f, out hit) ||
                             controller.characterDetection.ThrowRayOnDirection(origin2, controller.transform.forward, 1.0f, out hit))
                        {
                            timeDrop = 0;
                        }
                    }

                    //Drop if drop input or moving drop direction during 0.2s
                    if (controller.characterMovement.limitMovement && (controller.characterInput.drop || timeDrop > 0.15f) && controller.characterInput.movement != Vector2.zero)
                    {
                        animator.animator.CrossFade("Jump Down Slow", 0.1f);
                        timeDrop = -1;
                        controller.state.isJumping = true;
                        return true;
                    }
                }
                else
                {
                    timeDrop = 0;
                }
            }
            else
            {
                timeDrop = 0;
            }

            return false;
        }

        public override bool FixedUpdate()
        {
            bool ret = false;
            if (controller.state.isVaulting)
            {
                if (!controller.state.isDummy && controller.state.isJumping)
                {
                    //Grants movement while falling
                    controller.characterMovement.rb.position += (controller.transform.forward * controller.characterMovement.walkSpeed) * Time.fixedDeltaTime;
                    ret = true;
                }
            }

            return ret;
        }

        public override void DrawGizmos()
        {
            Vector3 origin = controller.transform.position + controller.transform.forward * 1.0f;
            Vector3 origin2 = controller.transform.position + Vector3.up * 0.1f;

            Debug.DrawLine(origin, origin + Vector3.down * 1.5f);
            Debug.DrawLine(origin2, origin + controller.transform.forward * 0.5f);
        }
    }
}
