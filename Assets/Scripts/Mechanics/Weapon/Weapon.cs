using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Mechanics.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponSettings
        {
            [Header("Weapon Audio Settings")]
            public AudioClip drawWeaponAudio;
            public AudioClip releaseWeaponAudio;
        }

        public WeaponSettings weaponSettings;

        protected AudioSource weaponAudio;

        protected virtual void Start()
        {
            weaponAudio = GetComponent<AudioSource>();
        }

        public abstract void PickWeapon();
        public abstract void DisableWeapon();
        public abstract void Fire(Vector3 direction);
    }
}
