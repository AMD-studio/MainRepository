using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDeleteController : MonoBehaviour, IDropable
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
            Destroy(item.gameObject);
        }
    }
}
