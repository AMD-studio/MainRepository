using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTriggerActivator : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        IDropable dropableObject = other.GetComponent<IDropable>();

        if (dropableObject != null)
        {
            dropableObject.Drop();
        }
    }
}
