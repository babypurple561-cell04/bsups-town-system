using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Guild building - manages quests and player progression.
/// </summary>
public class Guild : Building
{
    [System.Serializable]
    public class Quest
    {
        public string questName;
        public string description;
        public int reward;
        public bool completed;
    }

    [SerializeField] private List<Quest> availableQuests = new List<Quest>();
    [SerializeField] private Transform questPanel;
    [SerializeField] private Text questListText;
    [SerializeField] private Text questDetailsText;

    private Quest selectedQuest;

    protected override void Start()
    {
        buildingName = "Guild";
        buildingDescription = "The adventurer's guild. Accept quests and track your progress.";
        InitializeQuests();
        base.Start();
    }

    /// <summary>
    /// Initialize sample quests
    /// </summary>
    private void InitializeQuests()
    {
        availableQuests.Add(new Quest 
        { 
            questName = "Gather Wood", 
            description = "Collect 10 pieces of wood from the forest.",
            reward = 100,
            completed = false
        });
        
        availableQuests.Add(new Quest 
        { 
            questName = "Mine Copper", 
            description = "Mine 5 copper ore from the mine.",
            reward = 150,
            completed = false
        });
        
        availableQuests.Add(new Quest 
        { 
            questName = "Forge a Tool", 
            description = "Create a tool at the forge.",
            reward = 200,
            completed = false
        });
    }

    /// <summary>
    /// Display all available quests
    /// </summary>
    public void DisplayQuests()
    {
        if (questListText == null) return;

        string questList = "Available Quests:\n";
        for (int i = 0; i < availableQuests.Count; i++)
        {
            string status = availableQuests[i].completed ? "✓" : "○";
            questList += $"{status} {availableQuests[i].questName} - Reward: {availableQuests[i].reward}\n";
        }
        questListText.text = questList;
    }

    /// <summary>
    /// Select a quest to view details
    /// </summary>
    public void SelectQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= availableQuests.Count)
            return;

        selectedQuest = availableQuests[questIndex];
        
        if (questDetailsText != null)
        {
            questDetailsText.text = $"<b>{selectedQuest.questName}</b>\n\n{selectedQuest.description}\n\nReward: {selectedQuest.reward} Gold";
        }
    }

    /// <summary>
    /// Accept the selected quest
    /// </summary>
    public void AcceptQuest()
    {
        if (selectedQuest != null)
        {
            Debug.Log($"Quest accepted: {selectedQuest.questName}");
            // Add quest to player's active quests
        }
    }

    /// <summary>
    /// Complete a quest
    /// </summary>
    public void CompleteQuest(int questIndex)
    {
        if (questIndex >= 0 && questIndex < availableQuests.Count)
        {
            availableQuests[questIndex].completed = true;
            Debug.Log($"Quest completed: {availableQuests[questIndex].questName}");
            DisplayQuests();
        }
    }

    public override void Open()
    {
        base.Open();
        DisplayQuests();
    }
}