using UnityEngine;

namespace Climbing
{
    [CreateAssetMenu(menuName = "Climbing/Vaulting Obstacle Action")]
    public class ActionVaultObstacle : Action
    {
        public string HandAnimVariableName;

        [Range(0, 1f)]
        public float handToIKPositionSpeed = 0.0f;
    }
}
