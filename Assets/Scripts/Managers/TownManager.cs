using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central manager for the town system.
/// Handles building registration and town-wide events.
/// </summary>
public class TownManager : MonoBehaviour
{
    public static TownManager Instance { get; private set; }

    [SerializeField] private List<Building> buildings = new List<Building>();
    private Building currentOpenBuilding;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Auto-register all buildings in the scene
        Building[] allBuildings = FindObjectsOfType<Building>();
        foreach (Building building in allBuildings)
        {
            RegisterBuilding(building);
        }
    }

    /// <summary>
    /// Register a building with the town manager
    /// </summary>
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
            Debug.Log($"Building registered: {building.GetBuildingName()}");
        }
    }

    /// <summary>
    /// Open a building's interface
    /// </summary>
    public void OpenBuilding(Building building)
    {
        // Close previous building if open
        if (currentOpenBuilding != null && currentOpenBuilding != building)
        {
            currentOpenBuilding.Close();
        }

        building.Open();
        currentOpenBuilding = building;
    }

    /// <summary>
    /// Close the currently open building
    /// </summary>
    public void CloseCurrentBuilding()
    {
        if (currentOpenBuilding != null)
        {
            currentOpenBuilding.Close();
            currentOpenBuilding = null;
        }
    }

    /// <summary>
    /// Get all registered buildings
    /// </summary>
    public List<Building> GetAllBuildings() => new List<Building>(buildings);
}