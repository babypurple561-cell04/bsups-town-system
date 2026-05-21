using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all town buildings.
/// Handles opening/closing and player interactions.
/// </summary>
public abstract class Building : MonoBehaviour
{
    [SerializeField] protected string buildingName;
    [SerializeField] protected string buildingDescription;
    [SerializeField] protected float interactionRadius = 5f;
    [SerializeField] protected UnityEvent onOpen;
    [SerializeField] protected UnityEvent onClose;

    protected bool isOpen = false;
    protected bool isPlayerNearby = false;
    protected GameObject uiPanel;

    protected virtual void Start()
    {
        // Register with town manager
        if (TownManager.Instance != null)
        {
            TownManager.Instance.RegisterBuilding(this);
        }
    }

    protected virtual void Update()
    {
        // Check if player is nearby for interaction prompt
        CheckPlayerProximity();
    }

    /// <summary>
    /// Check if player is close enough to interact
    /// </summary>
    protected virtual void CheckPlayerProximity()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            isPlayerNearby = distance <= interactionRadius;
        }
    }

    /// <summary>
    /// Open the building interface
    /// </summary>
    public virtual void Open()
    {
        if (isOpen) return;

        isOpen = true;
        ShowUI();
        onOpen?.Invoke();
        Debug.Log($"{buildingName} opened.");
    }

    /// <summary>
    /// Close the building interface
    /// </summary>
    public virtual void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        HideUI();
        onClose?.Invoke();
        Debug.Log($"{buildingName} closed.");
    }

    /// <summary>
    /// Toggle building open/close state
    /// </summary>
    public virtual void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    /// <summary>
    /// Show the building UI
    /// </summary>
    protected virtual void ShowUI()
    {
        if (uiPanel != null)
            uiPanel.SetActive(true);
    }

    /// <summary>
    /// Hide the building UI
    /// </summary>
    protected virtual void HideUI()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    public string GetBuildingName() => buildingName;
    public string GetBuildingDescription() => buildingDescription;
    public bool IsOpen() => isOpen;
    public bool IsPlayerNearby() => isPlayerNearby;
}