using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    internal class DestroyTrigger : MonoBehaviour
    {
        public List<GameObject> objectsToDestroy;

        private void OnTriggerEnter(Collider other)
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
