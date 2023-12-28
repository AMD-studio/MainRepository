using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private GameObject _currentWeapon;


    public void SetWeapon(GameObject weapon)
    {
        if(weapon != _currentWeapon)
        {
            Destroy(_currentWeapon);
            if (weapon != null)
            {
                _currentWeapon = Instantiate(weapon, transform.position,new Quaternion(), transform);
            }
        }
    }
}
