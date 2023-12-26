using UnityEngine;

namespace Assets.Scripts.Mechanics.EntityModule
{
    public class Damage : MonoBehaviour
    {
        public int damage = 10; 

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Arrow"))
            {
   
                Debug.Log("Player takes damage0: " + damage);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Arrow"))
            {

                Debug.Log("Player takes damage1: " + damage);
            }
        }
    }
}
