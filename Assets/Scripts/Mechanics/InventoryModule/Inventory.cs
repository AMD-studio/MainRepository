using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

    // 0=Head, 1=Chest, 2=Legs, 3=Feet
    [SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    //[SerializeField] Button giveItemBtn;

    private int _selectedSlot = 0;

    public int SelectedSlot
    {
        get { return _selectedSlot; }
        set
        {
            
            var newValue = value;
            if (value > hotbarSlots.Length - 1)
            {
                newValue = 0;
            }
            else if (value < 0)
            {
                newValue = hotbarSlots.Length - 1;
            }
            hotbarSlots[_selectedSlot].isSelected = false;
            hotbarSlots[newValue].isSelected = true;
            _selectedSlot = newValue;
        }
    }

    public Inventory()
    {
        SelectedSlot = 0;
    }

    void Awake()
    {
        Singleton = this;
        //giveItemBtn.onClick.AddListener( delegate { SpawnInventoryItem(); } );
        SelectedSlot = 0;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            SelectedSlot--;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            SelectedSlot++;
            Debug.Log($"Current Slot {_selectedSlot}");
        }

        if(carriedItem == null) return;

        carriedItem.transform.position = Mouse.current.position.ReadValue();
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }

        if(item.activeSlot.myTag != SlotTag.None)
        { EquipEquipment(item.activeSlot.myTag, null); }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if(item == null)
                {
                    // Destroy item.equipmentPrefab on the Player Object;
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the Player Object;
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case SlotTag.Chest:
                break;
            case SlotTag.Legs:
                break;
            case SlotTag.Feet:
                break;
        }
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(_item == null)
        { _item = PickRandomItem(); }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Check if the slot is empty
            if(inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    public void PickItem(Item item)
    {
        Item _item = item;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
}
