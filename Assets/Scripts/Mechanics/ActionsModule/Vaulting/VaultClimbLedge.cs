using UnityEngine;

namespace Climbing
{
    [RequireComponent(typeof(ClimbController))]
    public class VaultClimbLedge : VaultAction
    {
        readonly ClimbController climbController;

        public VaultClimbLedge() { }

        public VaultClimbLedge(ThirdPersonController _vaultingController, Action _actionInfo = null) : base(_vaultingController)
        {
            climbController = controller.GetComponent<ClimbController>();
        }

        public override bool CheckAction()
        {
            return !controller.state.isVaulting && climbController.ClimbCheck();
        }

        public override bool Update()
        {
            return climbController.ClimbUpdate();
        }

        public override void OnAnimatorIK(int layerIndex)
        {
            climbController.OnAnimatorIK(layerIndex);
        }

        public override void DrawGizmos()
        {
            climbController.OnDrawGizmos();
        }
    }
}
