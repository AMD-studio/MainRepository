using UnityEngine;

namespace Climbing
{
    public class VaultSlide : VaultAction
    {
        private float dis;

        public VaultSlide() { }

        public VaultSlide(ThirdPersonController _vaultingController, Action _actionInfo) : base(_vaultingController, _actionInfo)
        {
        }


        /// <summary>
        /// Checks if Player can Slide the Obstacle
        /// </summary>
        public override bool CheckAction()
        {
            if (!controller.characterInput.drop || controller.state.isVaulting)
            {
                return false;
            }

            Vector3 origin = controller.transform.position + kneeRaycastOrigin;

            if (!controller.characterDetection.ThrowRayOnDirection(origin, controller.transform.forward, kneeRaycastLength, out RaycastHit hit))
            {
                return false;
            }

            Vector3 origin2 = origin + (-hit.normal * (hit.transform.localScale.z + landOffset));

            if ((hit.normal != hit.collider.transform.forward &&
                hit.normal != -hit.collider.transform.forward) ||
                Mathf.Abs(Vector3.Dot(-hit.normal, controller.transform.forward)) < 0.60 ||
                !hit.transform.CompareTag(tag))
            {
                return false;
            }


            if (controller.characterDetection.ThrowRayOnDirection(origin2, Vector3.down, 10, out RaycastHit hit2) && hit2.collider)
            {
                controller.characterAnimation.animator.CrossFade("Running Slide", 0.05f);
                dis = 4 / Vector3.Distance(startTransform.Position, targetTransform.Position);
                controller.characterAnimation.animator.SetFloat("AnimSpeed", dis);
                controller.characterAnimation.switchCameras.SlideCam();

                startTransform = new TransformData(controller.transform.position, controller.transform.rotation);
                targetTransform = new TransformData(hit2.point, Quaternion.LookRotation(targetTransform.Position - startTransform.Position));

                vaultTime = startDelay;
                animLength = clip.length + startDelay;
                controller.DisableController();

                return true;
            }

            return false;
        }


        /// <summary>
        /// Executes Vaulting Animation
        /// </summary>
        public override bool Update()
        {
            if (!controller.state.isVaulting)
                return false;

            float actualSpeed = Time.deltaTime / animLength;
            vaultTime += actualSpeed * (animator.animState.speed + dis);

            if (vaultTime > 1)
            {
                controller.characterAnimation.animator.SetFloat("AnimSpeed", 1);
                controller.characterAnimation.switchCameras.FreeLookCam();
                controller.EnableController();
            }
            else
            {
                controller.transform.SetPositionAndRotation(
                    Vector3.Lerp(startTransform.Position, targetTransform.Position, vaultTime),
                    Quaternion.Lerp(startTransform.Rotation, targetTransform.Rotation, vaultTime * 4));
                return true;
            }

            return false;
        }


        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetTransform.Position, 0.08f);
        }
    }
}