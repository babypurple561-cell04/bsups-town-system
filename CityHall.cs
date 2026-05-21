using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// City Hall building - manage licenses, job changes, and major offices
/// </summary>
public class CityHall : Building
{
    [SerializeField] private Transform licenseOfficePanel;
    [SerializeField] private Text licenseStatusText;
    [SerializeField] private Text jobListText;
    [SerializeField] private Button renewLicenseButton;

    private List<string> majorOffices = new List<string> { "Tax Office", "Records", "Permits" };
    private bool isPlayerQuestingOffice = false;
    private bool canSneak = true;

    protected override void Start()
    {
        buildingName = "City Hall";
        buildingDescription = "Government offices for licenses, permits, and job management.";
        base.Start();
    }

    /// <summary>
    /// Display license status and available jobs
    /// </summary>
    public void DisplayLicenseOptions()
    {
        if (JobManager.Instance == null) return;

        PlayerJobData playerData = JobManager.Instance.GetPlayerJobData();
        
        if (licenseStatusText != null)
        {
            string status = playerData.hasActiveLicense ? "✓ Active" : "✗ Expired";
            float daysRemaining = playerData.licenseExpiryGameDays - JobManager.Instance.GetCurrentGameDay();
            licenseStatusText.text = $"License Status: {status}\nDays Remaining: {daysRemaining:F1}\nCurrent Job: {playerData.currentJob} ({playerData.currentClass})";
        }

        DisplayAvailableJobs();
    }

    /// <summary>
    /// Display available jobs to switch to
    /// </summary>
    private void DisplayAvailableJobs()
    {
        if (JobManager.Instance == null) return;

        List<JobData> allJobs = JobManager.Instance.GetAllJobs();
        PlayerJobData playerData = JobManager.Instance.GetPlayerJobData();

        if (jobListText != null)
        {
            string jobList = "Available Jobs:\n";
            foreach (JobData job in allJobs)
            {
                bool unlocked = playerData.unlockedJobs.Contains(job.jobName);
                string locked = unlocked ? "" : " (LOCKED)";
                jobList += $"• {job.jobName}{locked}\n";
            }
            jobListText.text = jobList;
        }
    }

    /// <summary>
    /// Renew current job license
    /// </summary>
    public void RenewLicense(LicenseType licenseType)
    {
        PlayerJobData playerData = JobManager.Instance.GetPlayerJobData();
        float licenseCost = licenseType switch
        {
            LicenseType.Short => 50,
            LicenseType.MidLong => 100,
            LicenseType.HighLong => 200,
            _ => 50
        };

        if (JobManager.Instance.PurchaseJobLicense(playerData.currentJob, licenseType, licenseCost))
        {
            Debug.Log($"License renewed for {licenseType}!");
            DisplayLicenseOptions();
        }
    }

    /// <summary>
    /// Change job at the License Office
    /// </summary>
    public void ChangeJobAtLicenseOffice(string newJobName, string newClass)
    {
        PlayerJobData playerData = JobManager.Instance.GetPlayerJobData();
        float jobChangeCost = 75; // Cost to change jobs

        if (JobManager.Instance.ChangeJob(newJobName, newClass, jobChangeCost))
        {
            Debug.Log($"Job changed to {newJobName}!");
            DisplayLicenseOptions();
        }
    }

    /// <summary>
    /// Enter a major office to access quest
    /// </summary>
    public void EnterMajorOffice(string officeName)
    {
        if (!majorOffices.Contains(officeName)) return;

        Debug.Log($"Entered {officeName}...");
        isPlayerQuestingOffice = true;

        // Trigger quest or sneak option
        StartQuestOrSneak(officeName);
    }

    /// <summary>
    /// Start a quest in the office or attempt to sneak
    /// </summary>
    private void StartQuestOrSneak(string officeName)
    {
        if (Random.value > 0.5f && canSneak)
        {
            // Try to sneak
            if (AttemptSneak())
            {
                Debug.Log("Successfully sneaked into the office!");
                HandleOfficeQuest(officeName);
            }
            else
            {
                Debug.Log("Caught sneaking! Officer confronts you.");
                // Handle caught scenario
            }
        }
        else
        {
            // Normal quest path
            HandleOfficeQuest(officeName);
        }
    }

    /// <summary>
    /// Attempt to sneak (stealth check)
    /// </summary>
    private bool AttemptSneak()
    {
        int stealthCheck = Random.Range(1, 21); // d20 roll
        return stealthCheck > 10; // DC 10
    }

    /// <summary>
    /// Handle office-specific quests
    /// </summary>
    private void HandleOfficeQuest(string officeName)
    {
        switch (officeName)
        {
            case "Tax Office":
                Debug.Log("Tax Officer: Help us with some accounting!");
                TriggerTaxQuest();
                break;
            case "Records":
                Debug.Log("Records Officer: Find this missing file!");
                TriggerRecordsQuest();
                break;
            case "Permits":
                Debug.Log("Permits Officer: Process these applications!");
                TriggerPermitsQuest();
                break;
        }
    }

    /// <summary>
    /// Tax Office Quest
    /// </summary>
    private void TriggerTaxQuest()
    {
        int diceRoll = JobManager.Instance.RollForSecretQuest();
        string difficulty = JobManager.Instance.GetSecretQuestDifficulty(diceRoll);
        
        Debug.Log($"Secret Quest Triggered! Difficulty: {difficulty}");
        
        // Quest reward based on difficulty
        float respectReward = diceRoll * 5;
        JobManager.Instance.AddRespect(respectReward);
    }

    /// <summary>
    /// Records Quest
    /// </summary>
    private void TriggerRecordsQuest()
    {
        int diceRoll = JobManager.Instance.RollForSecretQuest();
        string difficulty = JobManager.Instance.GetSecretQuestDifficulty(diceRoll);
        
        Debug.Log($"Find Missing Records! Difficulty: {difficulty}");
        
        float respectReward = diceRoll * 5;
        JobManager.Instance.AddRespect(respectReward);
    }

    /// <summary>
    /// Permits Quest
    /// </summary>
    private void TriggerPermitsQuest()
    {
        int diceRoll = JobManager.Instance.RollForSecretQuest();
        string difficulty = JobManager.Instance.GetSecretQuestDifficulty(diceRoll);
        
        Debug.Log($"Process Applications! Difficulty: {difficulty}");
        
        float respectReward = diceRoll * 5;
        JobManager.Instance.AddRespect(respectReward);
    }

    public override void Open()
    {
        base.Open();
        DisplayLicenseOptions();
    }
}