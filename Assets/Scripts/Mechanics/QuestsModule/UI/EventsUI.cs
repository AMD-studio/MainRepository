using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventsUI : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        AcceptQuest();
    }

    public void AcceptQuest()
    {
        QuestManager.Instance.data.questItems[QuestManager.Instance.QuestID].isQuestConfirm = true;
        QuestManager.Instance.data.questItems[QuestManager.Instance.QuestID].currentObjectivesID = 0;
    }
}