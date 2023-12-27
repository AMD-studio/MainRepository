using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Mechanics.Weapon
{
    public class Sword : Weapon
    {
        public override void PickWeapon()
        {
            // Логика для поднятия меча
            Debug.Log("Picking up the sword");
        }

        public override void DisableWeapon()
        {
            // Логика для отключения меча
            Debug.Log("Disabling the sword");
        }

        public override void Fire(Vector3 direction)
        {
            // Логика удара мечом
            Debug.Log("Swinging the sword");
        }
    }
}
