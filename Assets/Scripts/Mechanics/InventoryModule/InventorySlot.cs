using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotTag { None, Head, Chest, Legs, Feet }

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; }
    public GameObject Player;
    
    public SlotTag myTag;
    public bool isDropSlot = false;

    public bool isSelected = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isDropSlot)
            {
                if (Inventory.carriedItem == null) return;
                Instantiate(Inventory.carriedItem.myItem.equipmentPrefab, Player.transform.position, new Quaternion());
                Destroy(Inventory.carriedItem.gameObject);
            }
            if (Inventory.carriedItem == null) return;
            if(myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag) return;
            SetItem(Inventory.carriedItem);
        }
    }

    void Update()
    {
        if (isDropSlot) return;
        if (isSelected)
        {
            gameObject.GetComponent<Image>().color = new Color(25,25,25);
            if(!myItem) {
                Player.GetComponentInChildren<WeaponController>().SetWeapon(null);
                return;
            }
            Player.GetComponentInChildren<WeaponController>().SetWeapon(myItem.myItem.equipmentPrefab);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f, 190);
        }
    }

    public void SetItem(InventoryItem item)
    {
        Inventory.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        if(myTag != SlotTag.None)
        { Inventory.Singleton.EquipEquipment(myTag, myItem); }
    }
}
