using UnityEngine;

namespace Climbing
{
    public class VaultObstacle : VaultAction
    {
        private Vector3 leftHandPosition;
        private Quaternion leftHandRotation;
        private string HandAnimVariableName;

        public VaultObstacle() { }

        public VaultObstacle(ThirdPersonController _vaultingController, Action _actionInfo) : base(_vaultingController, _actionInfo)
        {
            ActionVaultObstacle action = (ActionVaultObstacle)_actionInfo;

            //Loads Action Info
            HandAnimVariableName = action.HandAnimVariableName;
        }

        /// <summary>
        /// Checks if can Vault the Fence Obstacle
        /// </summary>
        public override bool CheckAction()
        {
            if (controller.characterInput.jump && !controller.state.isVaulting)
            {
                RaycastHit hit;
                Vector3 origin = controller.transform.position + kneeRaycastOrigin;

                //Checks if Vault obstacle in front
                if (controller.characterDetection.ThrowRayOnDirection(origin, controller.transform.forward, kneeRaycastLength, out hit))
                {
                    // If direction not the same as object don't do anything
                    // or angle of movement not valid
                    if ((hit.normal == hit.collider.transform.forward || 
                        hit.normal == -hit.collider.transform.forward) == false
                        || hit.transform.tag != tag)
                        return false;


                    //Gets Fence width and adds an offset for the downward ray
                    Vector3 origin2 = origin + (-hit.normal * (hit.transform.localScale.z + landOffset));

                    RaycastHit hit2;
                    //Get landing position
                    if (controller.characterDetection.ThrowRayOnDirection(origin2, Vector3.down, 10, out hit2))
                    {
                        if (hit2.collider)
                        {
                            controller.characterAnimation.animator.CrossFade("Vaulting", 0.2f);

                            startTransform = new TransformData(controller.transform.position, controller.transform.rotation);
                            targetTransform = new TransformData(hit2.point, Quaternion.LookRotation(targetTransform.Position - startTransform.Position));
                          
                            vaultTime = startDelay; //This adds a delay to allow animation start in correct time
                            animLength = clip.length + startDelay;
                            controller.DisableController();

                            //Calculate Hand Rest Position n Rotation
                            Vector3 left = Vector3.Cross(hit.normal, Vector3.up);
                            leftHandPosition = hit.point + (-hit.normal * (hit.transform.localScale.z / 2));
                            leftHandPosition.y = hit.transform.position.y + hit.transform.localScale.y / 2;
                            leftHandPosition.x += left.x * animator.animator.GetBoneTransform(HumanBodyBones.LeftHand).localPosition.x;
                            leftHandRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

                            return true;
                        }
                    }
                }
            }            

            return false;
        }

        /// <summary>
        /// Executes Vaulting Animation
        /// </summary>
        public override bool Update()
        {
            if (controller.state.isVaulting)
            {
                float actualSpeed = Time.deltaTime / animLength;
                vaultTime += actualSpeed * animator.animState.speed;

                if (vaultTime > 1)
                {
                    controller.EnableController();
                }
                else
                {
                    if (vaultTime >= 0)
                    {
                        controller.transform.SetPositionAndRotation(Vector3.Lerp(startTransform.Position, targetTransform.Position, vaultTime), Quaternion.Lerp(startTransform.Rotation, targetTransform.Rotation, vaultTime * 4));
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnAnimatorIK(int layerIndex)
        {
            if (!controller.state.isVaulting)
                return;

            float curve = animator.animator.GetFloat(HandAnimVariableName);
            animator.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, curve);
            animator.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            animator.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, curve);
            animator.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRotation);
        }

        public override void DrawGizmos()
        {
            Gizmos.DrawSphere(targetTransform.Position, 0.08f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(leftHandPosition, 0.08f);
        }
    }
}
