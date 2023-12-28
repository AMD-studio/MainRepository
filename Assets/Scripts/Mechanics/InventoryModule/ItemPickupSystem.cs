using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ItemPickupSystem : MonoBehaviour
{
    public GUISystem GUISystem;
    public Inventory inventory;
    private Pickable _currentItem;
    private GameObject _currentObject;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Pickable>())
        {
            _currentItem = other.GetComponent<Pickable>();
            _currentObject = other.gameObject;
            GUISystem.SetActionText($"Pickup {_currentItem.item.name} press \"F\"");
        }
    }

    private void Update()
    {
        if( _currentItem != null && _currentObject != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                try { 
                    Destroy(_currentObject);
                    GUISystem.SetActionText("");
                    inventory.PickItem(_currentItem.item);
                    _currentItem = null;
                    _currentObject = null;
                }
                catch
                {
                    
                }
            }
        }   
    }

    public void OnTriggerExit(Collider other)
    {
        _currentItem = null;
        GUISystem.SetActionText("");
    }
}
