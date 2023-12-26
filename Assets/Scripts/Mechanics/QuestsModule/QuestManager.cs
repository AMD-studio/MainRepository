using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private const float QUEST_COMPLETED_MESSAGE_SECONDS = 1.5f;

    public QuestData data;
    public GameObject mainQuestTrigger;

    private int m_playerLvl = 1;
    private float m_playerXP = 0;

    [Header("Experience")]
    public float maxLvlXP = 750;

    [Range(1.1f, 5.0f)]
    public float XPModificator = 1.1f;

    public AchivementsData achivementsData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetQuestData();
        SpawnStartQuestTrigger();
        HideAllQuestUI();
        GetQuestsAvailable();
        UpdatePlayerData();

        // Reset achivements status
        if (achivementsData is not null)
        {
            foreach (AchivementsItem v in achivementsData.items)
            {
                v.isUnblocked = false;
            }
        }
    }

    public void HideAllQuestUI()
    {
        QuestInfoUI.Instance.Hide();
        QuestCompletedUI.Instance.Hide();
        QuestConfirmUI.Instance.Hide();
        QuestNavigateUI.Instance.Hide();
    }

    public void Update()
    {
        if (!IsQuestConfirm)
        {
            return;
        }

        GetHeightObjective();
        QuestNavigateUI.Instance.UpdateDistance(GetDistanceToObjective().ToString());
    }

    private QuestItem[] QuestItems => data.questItems;

    public int QuestID => data.currentQuestID;
    public int QuestSubID => QuestItems[QuestID].currentObjectivesID;
    public string QuestName => QuestItems[QuestID].questName;

    public string QuestSubName
    {
        get
        {
            int currentSubTargetID = QuestItems[QuestID].currentObjectivesID;
            return QuestItems[QuestID].
                objectives[currentSubTargetID].
                name;
        }
    }

    public bool IsQuestConfirm => QuestItems[QuestID].isQuestConfirm;

    public void GetQuestInfoData()
    {
        QuestConfirmUI.Instance.UpdateQuestName(QuestName);
        QuestInfoUI.Instance.UpdateQuestName(QuestName);
        QuestInfoUI.Instance.UpdateQuestObjectives(QuestSubName);
    }

    public void SetFirstQuest()
    {
        GetQuestInfoData();
    }

    public void NextQuest()
    {
        GetQuestInfoData();
        data.currentQuestID++;
        GetQuestsAvailable();
        SpawnStartQuestTrigger();
    }

    public void NextQuestSub()
    {
        int currentobjectives = QuestItems[QuestID].objectives.Length;
        if (QuestSubID < currentobjectives)
            QuestItems[QuestID].currentObjectivesID++;

        GetQuestInfoData();
        SpawnObjectiveTrigger();
    }

    public void QuestStart()
    {
        QuestConfirmUI.Instance.Hide();
        QuestInfoUI.Instance.Show();
        QuestNavigateUI.Instance.Show();
    }

    public void GetQuestsAvailable()
    {
        int allQuestsNum = data.questItems.Length;
        int completedQuestsNum = data.currentQuestID;

        QuestAvailableUI.Instance.UpdateQuestsNum($"{allQuestsNum - completedQuestsNum}");
    }

    public void UpdatePlayerData()
    {
        if (XPModificator > 1.0f)
        {
            while (m_playerXP >= maxLvlXP)
            {
                m_playerLvl++;
                m_playerXP -= maxLvlXP;
                maxLvlXP *= XPModificator;
                QuestLvlUI.Instance.ShowLvlUpPanel(true);
                UpdateLvlData();
            }
        }
        else
        {
            Debug.LogError("Setting a value less than 1.0 for the 'XPModificator' will result in an infinite loop during the completion of the quest");
        }
        UpdateLvlData();
    }

    public void UpdateLvlData()
    {
        float _playerXP = Math.Abs((float)Math.Round(m_playerXP, 1));
        float _maxLvlXP = (float)Math.Round(maxLvlXP, 1);
        QuestLvlUI.Instance.UpdateLvl(m_playerLvl.ToString());
        QuestLvlUI.Instance.UpdateXP($"{_playerXP} / {_maxLvlXP}", Math.Abs(_playerXP) / _maxLvlXP);
    }

    public void QuestEnd(bool isDecline = false)
    {
        QuestConfirmUI.Instance.Hide();
        QuestInfoUI.Instance.Hide();
        QuestNavigateUI.Instance.Hide();

        if (!isDecline)
        {
            GetQuestsAvailable();
            m_playerXP += QuestItems[QuestID].xp;
            UpdatePlayerData();
            StartCoroutine(QuestCompleted());
        }
    }

    IEnumerator QuestCompleted()
    {
        QuestCompletedUI.Instance.Show();
        QuestCompletedUI.Instance.UpdateQuestName(QuestName);
        QuestCompletedUI.Instance.UpdateQuestXP(QuestItems[QuestID].xp);

        yield return new WaitForSeconds(QUEST_COMPLETED_MESSAGE_SECONDS);

        QuestCompletedUI.Instance.UpdateQuestName("");
        QuestCompletedUI.Instance.Hide();
        QuestLvlUI.Instance.ShowLvlUpPanel(false);
    }

    public void DeclineQuest()
    {
        QuestEnd(true);
    }

    public void SpawnStartQuestTrigger()
    {
        Vector3 position = QuestItems[QuestID].triggerPosition;
        Quaternion rotation = QuestItems[QuestID].triggerRotation;

        Instantiate(mainQuestTrigger, position, rotation);
    }

    public void SpawnObjectiveTrigger()
    {
        Vector3 position = QuestItems[QuestID].objectives[QuestSubID].triggerPosition;
        Quaternion rotation = QuestItems[QuestID].objectives[QuestSubID].triggerRotation;
        Vector3 scale = QuestItems[QuestID].objectives[QuestSubID].triggerScale;
        GameObject spawnObjective = QuestItems[QuestID].objectives[QuestSubID].spawnObjective;

        spawnObjective.transform.localScale = scale;

        Instantiate(spawnObjective, position, rotation);
    }


    // Reset currentQuestID, currentObjectivesID and isQuestConfirm on ScriptableObject in play mode start
    public void ResetQuestData()
    {
        data.currentQuestID = 0;
        for (int i = 0; i < data.questItems[QuestID].objectives.Length - 1; i++)
        {
            data.questItems[QuestID].currentObjectivesID = 0;
        }

        for (int i = 0; i < data.questItems.Length; i++)
        {
            data.questItems[i].isQuestConfirm = false;
        }
    }

    public void SetCursor(bool isShow = false)
    {
        Cursor.visible = isShow;
        Cursor.lockState = isShow ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public int GetDistanceToObjective()
    {
        Vector3 objectivePosition = data.questItems[QuestID].objectives[QuestID].triggerPosition;
        return ((int)(GameSystem.Instance.player.transform.position - objectivePosition).magnitude);
    }



    public void GetHeightObjective()
    {
        Vector3 objectivePosition = QuestItems[QuestID].objectives[QuestSubID].triggerPosition;

        if (objectivePosition.y >= GameSystem.Instance.player.transform.position.y + 2)
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Up");
        }
        else if (objectivePosition.y + 2 <= GameSystem.Instance.player.transform.position.y)
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Down");
        }
        else
        {
            QuestNavigateUI.Instance.UpdateHeightObjective("Same level");
        }
    }
}