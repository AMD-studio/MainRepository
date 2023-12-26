using UnityEngine;

namespace Climbing
{
    [CreateAssetMenu(menuName = "Climbing/Vaulting Reach Action")]
    public class ActionVaultReach : Action
    {
        public float midHeight;
        public float maxHeight;

        public string HandAnimVariableName;
    }
}
