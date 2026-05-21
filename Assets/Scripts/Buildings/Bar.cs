using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Bar building - serves consumables and provides NPC interactions.
/// </summary>
public class Bar : Building
{
    [SerializeField] private List<string> npcNames = new List<string> { "Bartender", "Drunkard", "Traveler" };
    [SerializeField] private List<string> dialogues = new List<string> 
    { 
        "Welcome to the bar!",
        "What can I get you?",
        "Great day, isn't it?"
    };
    [SerializeField] private Transform dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text npcNameText;

    private int currentNPCIndex = 0;

    protected override void Start()
    {
        buildingName = "Bar";
        buildingDescription = "A cozy tavern where adventurers gather.";
        base.Start();
    }

    /// <summary>
    /// Serve a drink to the player
    /// </summary>
    public void ServeDrink()
    {
        Debug.Log("Served a drink!");
        // Add consumable to player inventory
    }

    /// <summary>
    /// Interact with an NPC
    /// </summary>
    public void InteractWithNPC(int npcIndex)
    {
        if (npcIndex < 0 || npcIndex >= npcNames.Count)
            return;

        currentNPCIndex = npcIndex;
        
        if (npcNameText != null)
            npcNameText.text = npcNames[npcIndex];
        
        if (dialogueText != null)
            dialogueText.text = dialogues[npcIndex];

        Debug.Log($"Talking to {npcNames[npcIndex]}: {dialogues[npcIndex]}");
    }

    /// <summary>
    /// Show next NPC dialogue
    /// </summary>
    public void NextDialogue()
    {
        currentNPCIndex = (currentNPCIndex + 1) % npcNames.Count;
        InteractWithNPC(currentNPCIndex);
    }

    public override void Open()
    {
        base.Open();
        InteractWithNPC(0); // Start with first NPC
    }
}