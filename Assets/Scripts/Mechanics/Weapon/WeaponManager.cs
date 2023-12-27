using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Mechanics.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        private static WeaponManager instance;

        private readonly Queue<Weapon> weaponQueue = new Queue<Weapon>();
        private readonly int maxWeapons = 2;

        public static WeaponManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WeaponManager>();
                    if (instance == null)
                    {
                        GameObject managerObject = new("WeaponManager");
                        instance = managerObject.AddComponent<WeaponManager>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            // Убеждаемся, что существует только один экземпляр WeaponManager
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void AddWeapon(Weapon weapon)
        {
            if (weaponQueue.Count >= maxWeapons)
            {
                Debug.LogWarning("Weapon queue is full. Cannot add more weapons.");
                return;
            }

            weaponQueue.Enqueue(weapon);
            weapon.PickWeapon();
        }

        public void SwitchWeapon(int index)
        {
            if (weaponQueue.Count < 2)
            {
                Debug.LogWarning("Not enough weapons to switch.");
                return;
            }

            Weapon currentWeapon = weaponQueue.Dequeue();
            currentWeapon.DisableWeapon();

            Weapon nextWeapon = weaponQueue.Peek();
            nextWeapon.PickWeapon();
        }

        public Weapon GetCurrentWeapon()
        {
            if (weaponQueue.Count > 0)
            {
                return weaponQueue.Peek();
            }

            return null;
        }
    }
}
