using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Job system - manages player jobs, classes, licenses, and progression
/// </summary>
[System.Serializable]
public class JobData
{
    public string jobName;
    public string jobDescription;
    public JobType jobType;
    public int requiredLevel;
    public float licenseExpiryDays;
    public bool isStarterJob;
    public List<string> availableClasses;
    public List<string> availableSubclasses;
}

[System.Serializable]
public class PlayerJobData
{
    public string currentJob;
    public string currentClass;
    public string currentSubclass;
    public int jobLevel;
    public float jobExperience;
    public float licenseExpiryGameDays;
    public bool hasActiveLicense;
    public List<string> unlockedJobs;
    public List<string> completedQuests;
    public float respect; // for teachers/recruiters
}

public enum JobType
{
    Laborer,      // Town/Bar - gathering, basic work
    Adventurer,   // Guild - fighting, combat
    Crafter       // Forge - crafting, smithing
}

public enum LicenseType
{
    Short,    // 30 game days
    MidLong,  // 60 game days
    HighLong  // 120 game days
}

public class JobManager : MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    [SerializeField] private List<JobData> availableJobs = new List<JobData>();
    private PlayerJobData playerJobData;
    
    private float currentGameDayTimer = 0f;
    private float gameSpeedMultiplier = 1f; // 1 real second = X game days

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeJobs();
    }

    private void Start()
    {
        // Load player job data or create new
        if (playerJobData == null)
        {
            playerJobData = new PlayerJobData();
            playerJobData.unlockedJobs = new List<string>();
            playerJobData.completedQuests = new List<string>();
        }
    }

    private void Update()
    {
        UpdateLicenseExpiry();
    }

    /// <summary>
    /// Initialize all available jobs
    /// </summary>
    private void InitializeJobs()
    {
        // LABORER JOB - Town/Bar People
        availableJobs.Add(new JobData
        {
            jobName = "Laborer",
            jobDescription = "Work in the town and bar. Gather resources and serve customers.",
            jobType = JobType.Laborer,
            requiredLevel = 0,
            licenseExpiryDays = 0, // Starter job - no license needed
            isStarterJob = true,
            availableClasses = new List<string> { "Bartender", "Waiter", "Cleaner" },
            availableSubclasses = new List<string> { "Charismatic Worker", "Efficient Worker" }
        });

        // ADVENTURER JOB - Guild/Fighting
        availableJobs.Add(new JobData
        {
            jobName = "Adventurer",
            jobDescription = "Fight monsters, complete quests, and earn glory.",
            jobType = JobType.Adventurer,
            requiredLevel = 5,
            licenseExpiryDays = 30,
            isStarterJob = false,
            availableClasses = new List<string> { "Warrior", "Rogue", "Mage" },
            availableSubclasses = new List<string> { "Tank", "Damage Dealer", "Support" }
        });

        // CRAFTER JOB - Forge/Armor Smith
        availableJobs.Add(new JobData
        {
            jobName = "Crafter",
            jobDescription = "Craft weapons, armor, and tools. Master the forge.",
            jobType = JobType.Crafter,
            requiredLevel = 3,
            licenseExpiryDays = 30,
            isStarterJob = false,
            availableClasses = new List<string> { "Blacksmith", "Armorsmith", "Weaponsmith" },
            availableSubclasses = new List<string> { "Mastercrafter", "Enchanter" }
        });
    }

    /// <summary>
    /// Start the game with starter job
    /// </summary>
    public void StartAsLaborer(string starterClass)
    {
        JobData laborerJob = availableJobs.Find(j => j.jobName == "Laborer");
        if (laborerJob == null) return;

        playerJobData.currentJob = "Laborer";
        playerJobData.currentClass = starterClass;
        playerJobData.jobLevel = 1;
        playerJobData.jobExperience = 0;
        playerJobData.hasActiveLicense = true;
        playerJobData.unlockedJobs.Add("Laborer");

        Debug.Log($"Started as {starterClass} ({playerJobData.currentJob})");
    }

    /// <summary>
    /// Purchase a job license at City Hall
    /// </summary>
    public bool PurchaseJobLicense(string jobName, LicenseType licenseType, float cost)
    {
        JobData targetJob = availableJobs.Find(j => j.jobName == jobName);
        if (targetJob == null) return false;

        float licenseExpiryDays = licenseType switch
        {
            LicenseType.Short => 30,
            LicenseType.MidLong => 60,
            LicenseType.HighLong => 120,
            _ => 30
        };

        // TODO: Deduct cost from player inventory/gold
        playerJobData.licenseExpiryGameDays = currentGameDayTimer + licenseExpiryDays;
        playerJobData.hasActiveLicense = true;

        Debug.Log($"Purchased {licenseType} license for {jobName}. Expires in {licenseExpiryDays} game days.");
        return true;
    }

    /// <summary>
    /// Change to a different job (requires license at City Hall)
    /// </summary>
    public bool ChangeJob(string newJobName, string newClass, float cost)
    {
        JobData targetJob = availableJobs.Find(j => j.jobName == newJobName);
        if (targetJob == null)
        {
            Debug.LogWarning($"Job {newJobName} not found!");
            return false;
        }

        if (!playerJobData.unlockedJobs.Contains(newJobName))
        {
            Debug.LogWarning($"Job {newJobName} not unlocked!");
            return false;
        }

        if (!targetJob.availableClasses.Contains(newClass))
        {
            Debug.LogWarning($"Class {newClass} not available for {newJobName}!");
            return false;
        }

        // TODO: Deduct cost from player inventory/gold
        playerJobData.currentJob = newJobName;
        playerJobData.currentClass = newClass;
        playerJobData.currentSubclass = "";

        Debug.Log($"Changed job to {newJobName} as {newClass}!");
        return true;
    }

    /// <summary>
    /// Choose or change subclass (requires meeting requirements)
    /// </summary>
    public bool ChooseSubclass(string subclassName)
    {
        JobData currentJobData = availableJobs.Find(j => j.jobName == playerJobData.currentJob);
        if (currentJobData == null) return false;

        if (!currentJobData.availableSubclasses.Contains(subclassName))
        {
            Debug.LogWarning($"Subclass {subclassName} not available!");
            return false;
        }

        // Check requirements (level, respect, etc.)
        if (!CheckSubclassRequirements(subclassName))
        {
            Debug.LogWarning($"Requirements not met for {subclassName}!");
            return false;
        }

        playerJobData.currentSubclass = subclassName;
        Debug.Log($"Subclass changed to {subclassName}!");
        return true;
    }

    /// <summary>
    /// Check if player meets subclass requirements
    /// </summary>
    private bool CheckSubclassRequirements(string subclassName)
    {
        return subclassName switch
        {
            "Mastercrafter" => playerJobData.jobLevel >= 10,
            "Enchanter" => playerJobData.jobLevel >= 15 && playerJobData.respect >= 50,
            "Tank" => playerJobData.jobLevel >= 5,
            "Damage Dealer" => playerJobData.jobLevel >= 7 && playerJobData.respect >= 30,
            "Support" => playerJobData.jobLevel >= 8 && playerJobData.respect >= 40,
            _ => false
        };
    }

    /// <summary>
    /// Unlock a new job (through quests, tournaments, saving NPCs, etc.)
    /// </summary>
    public void UnlockJob(string jobName)
    {
        if (!playerJobData.unlockedJobs.Contains(jobName))
        {
            playerJobData.unlockedJobs.Add(jobName);
            Debug.Log($"Unlocked job: {jobName}");
        }
    }

    /// <summary>
    /// Add respect points (earned from quests, tournaments, NPCs)
    /// </summary>
    public void AddRespect(float amount)
    {
        playerJobData.respect += amount;
        Debug.Log($"Respect +{amount}. Total: {playerJobData.respect}");
    }

    /// <summary>
    /// Roll for secret/special quest (dice roll 6-10)
    /// </summary>
    public int RollForSecretQuest()
    {
        int roll = Random.Range(6, 11); // 6-10
        Debug.Log($"Secret quest roll: {roll}");
        return roll;
    }

    /// <summary>
    /// Get secret quest difficulty based on roll
    /// </summary>
    public string GetSecretQuestDifficulty(int roll)
    {
        return roll switch
        {
            6 => "Uncommon",
            7 => "Rare",
            8 => "Epic",
            9 => "Legendary",
            10 => "Mythic",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Update license expiry
    /// </summary>
    private void UpdateLicenseExpiry()
    {
        currentGameDayTimer += Time.deltaTime * gameSpeedMultiplier;

        if (playerJobData.hasActiveLicense && currentGameDayTimer >= playerJobData.licenseExpiryGameDays)
        {
            playerJobData.hasActiveLicense = false;
            Debug.LogWarning("Job license has expired!");
        }
    }

    /// <summary>
    /// Gain job experience and level up
    /// </summary>
    public void GainJobExperience(float amount)
    {
        playerJobData.jobExperience += amount;
        float expToLevelUp = playerJobData.jobLevel * 100;

        while (playerJobData.jobExperience >= expToLevelUp)
        {
            playerJobData.jobExperience -= expToLevelUp;
            playerJobData.jobLevel++;
            Debug.Log($"Level up! Job level: {playerJobData.jobLevel}");
            expToLevelUp = playerJobData.jobLevel * 100;
        }
    }

    // Getters
    public PlayerJobData GetPlayerJobData() => playerJobData;
    public JobData GetJobData(string jobName) => availableJobs.Find(j => j.jobName == jobName);
    public List<JobData> GetAllJobs() => availableJobs;
    public float GetCurrentGameDay() => currentGameDayTimer;
}