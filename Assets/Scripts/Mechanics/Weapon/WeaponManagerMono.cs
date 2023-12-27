using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Mechanics.Weapon
{
    public class WeaponManagerMono : MonoBehaviour
    {
        private WeaponManager weaponManager;
        public GameObject placeToPickup;

        private void Start()
        {
            weaponManager = WeaponManager.Instance;

            Bow bow = new Bow();
            Sword sword = new Sword();

            weaponManager.AddWeapon(bow);
            weaponManager.AddWeapon(sword);
        }

        private void Update()
        {
            // Обработка событий ввода от Input System

            // Переключение оружия с клавиатуры
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Debug.Log("Code 1");
                weaponManager.SwitchWeapon(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                Debug.Log("Code 2");
                weaponManager.SwitchWeapon(1);
            }

            // Стрельба текущим оружием по нажатию кнопки мыши
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Weapon currentWeapon = weaponManager.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    // Здесь можете использовать координаты мыши или другие параметры для направления стрельбы
                    Vector3 fireDirection = Vector3.forward;
                    currentWeapon.Fire(fireDirection);
                }
            }
        }
    }
}
