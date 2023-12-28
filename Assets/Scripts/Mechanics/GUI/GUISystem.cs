using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GUISystem : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject PauseMenu;
    public TextMeshProUGUI ActionText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (PauseMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ToggleInventory()
    {
        if (!PauseMenu.activeSelf)
        {
            Inventory.SetActive(!Inventory.activeSelf);
        }
    }

    public void SetActionText(string actionText)
    {
        ActionText.text = actionText;
    }

    public void TogglePause()
    {
        if(Inventory.activeSelf)
        {
            Inventory.SetActive(!Inventory.activeSelf);
        }

        PauseMenu.SetActive(!PauseMenu.activeSelf);
    }
}
