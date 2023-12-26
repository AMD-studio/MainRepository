using UnityEngine;

namespace Climbing
{
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    public abstract class VaultAction
    {
        protected AnimationClip clip;
        protected ThirdPersonController controller;
        protected AnimationCharacterController animator;

        protected TransformData targetTransform;
        protected TransformData startTransform;

        protected float vaultTime = 0.0f;
        protected float animLength = 0.0f;
        protected Vector3 kneeRaycastOrigin;
        protected float kneeRaycastLength;
        protected float landOffset;
        protected float startDelay = 0f;
        protected string tag;

        public VaultAction() { }

        public VaultAction(ThirdPersonController _controller)
        {
            controller = _controller;
            animator = controller.characterAnimation;
        }

        public VaultAction(ThirdPersonController _controller, Action action)
        {
            controller = _controller;
            animator = controller.characterAnimation;

            //Loads Action Info from Scriptable Object
            clip = action.clip;
            kneeRaycastOrigin = action.kneeRaycastOrigin;
            kneeRaycastLength = action.kneeRaycastLength;
            landOffset = action.landOffset;
            startDelay = Mathf.Abs(action.startDelay) * -1;
            tag = action.tag;
        }

        public abstract bool CheckAction();

        public virtual bool Update() { return true; }

        public virtual bool FixedUpdate() { return true; }

        public virtual void DrawGizmos() {}

        public virtual void OnAnimatorIK(int layerIndex) {}
    }
}