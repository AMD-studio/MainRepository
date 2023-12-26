using UnityEngine;

namespace Assets.Scripts.Mechanics.EnviromentModule
{
    public class HighlightObject : MonoBehaviour
    {
        [SerializeField] private float highlightingRadius = 4f;
        private Outline _lastOutItem;
        [SerializeField] private LayerMask itemLayer; // Определяет, на каких слоях выполнять проверку

        private void Update()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, highlightingRadius, itemLayer);

            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Item"))
                {
                    if (_lastOutItem != null)
                        _lastOutItem.enabled = false;

                    _lastOutItem = collider.GetComponent<Outline>();
                    _lastOutItem.enabled = true;
                    return; // Выходим из цикла, чтобы обработать только первый объект в зоне действия
                }
            }

            if (_lastOutItem != null)
            {
                _lastOutItem.enabled = false;
                _lastOutItem = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, highlightingRadius);
        }
    }
}
