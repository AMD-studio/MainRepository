using UnityEngine;

namespace Climbing
{
    [RequireComponent(typeof(JumpPredictionController))]
    public class VaultJumpPrediction : VaultAction
    {
        private JumpPredictionController jumpController;

        private VaultJumpPrediction() { }

        public VaultJumpPrediction(ThirdPersonController _vaultingController, Action _actionInfo = null) : base(_vaultingController)
        {
            jumpController = controller.GetComponent<JumpPredictionController>();
        }

        public override bool CheckAction()
        {
            if (controller.state.isVaulting)
                return false;

            //Ensures that curPoint is Pole type
            if (jumpController.curPoint != null)
            {
                if (jumpController.curPoint.transform.parent.parent.tag != "Pole")
                    jumpController.curPoint = null;
            }

            jumpController.CheckJump();

            return !jumpController.hasArrived();
        }

        public override bool FixedUpdate()
        {
            bool ret;
            if (!jumpController.hasArrived())
            {
                jumpController.hasEndedJump();

                jumpController.FollowParabola(0.7f);
                ret = true;
            }
            else
            {
                ret = jumpController.IsMidPoint();
            }

            return ret;
        }
    }
}
