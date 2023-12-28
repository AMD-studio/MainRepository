using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TrapPhysicController : MonoBehaviour, IDropable
{
    public GameObject[] TrapObjects = new GameObject[0];
    public int TrapObjectsCount
    {
        get
        {
            return TrapObjects.Length;
        }
        set
        {
            TrapObjects = new GameObject[value];
        }
    }

    public void Drop()
    {
        foreach (var item in TrapObjects)
        {
            foreach (var component in item.GetComponentsInChildren<Transform>())
            {
                if(component.gameObject.GetComponent<Rigidbody>() == null)
                {
                    component.gameObject.AddComponent<Rigidbody>();
                }
            }
        }
    }
}
