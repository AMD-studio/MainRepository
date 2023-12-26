using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Mechanics.CelestialMechanics
{
    public class SunMovement : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = 10.0f; 

        private float currentRotation = 0.0f; 

        public float CurrentPosition => currentRotation;

        private void Update()
        {
            RotateSun();
        }

        private void RotateSun()
        {
            currentRotation += rotationSpeed * Time.deltaTime; 
            transform.rotation = Quaternion.Euler(currentRotation, 0, 0);
        }
    }
}
