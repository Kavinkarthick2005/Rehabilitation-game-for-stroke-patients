using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalFish = 3;
    public int fishesFed = 0;

    public float totalGameDuration = 60f;
    private float totalTimer = 0f;

    public GameObject winUI;
    public GameObject loseUI;

    public TextMeshProUGUI timerText;

    private bool gameEnded = false;

    // --- Patient / Session Data Logging ---
    private string filePath;

    private int patientIndex;        // 1, 2, 3, ...
    private string patientID;        // "Patient01", "Patient02", ...

    void Awake()
    {
        instance = this;

        // Load next patient index from PlayerPrefs (default = 1)
        patientIndex = PlayerPrefs.GetInt("NextPatientIndex", 1);
        patientID = "Patient" + patientIndex.ToString("D2");   // Patient01, Patient02, ...

        // Setup CSV file path
        filePath = Application.persistentDataPath + "/PatientData.csv";

        // If file doesn’t exist, create it with headers
        if (!File.Exists(filePath))
        {
            string header = "PatientID,Level,TimeTaken,FishFed,Score,Date\n";
            File.WriteAllText(filePath, header);
        }

        Debug.Log("CSV File Path: " + filePath);
        // Optional while testing:
        //Application.OpenURL(Application.persistentDataPath);
    }

    void Update()
    {
        if (gameEnded) return;

        totalTimer += Time.deltaTime;

        float timeLeft = totalGameDuration - totalTimer;
        timeLeft = Mathf.Clamp(timeLeft, 0f, totalGameDuration);

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"Time Left: {minutes:00}:{seconds:00}";

        if (totalTimer >= totalGameDuration)
        {
            EndGame(false); // Time's up → lose
        }
    }

    public void FishFed()
    {
        if (gameEnded) return;

        fishesFed++;
        Debug.Log("Fish Fed! Total: " + fishesFed);

        if (fishesFed >= totalFish)
        {
            EndGame(true); // All fish fed → win
            timerText.gameObject.SetActive(false);
        }
    }

    public void EndGame(bool win)
    {
        gameEnded = true;

        float timeTaken = totalTimer;
        int score = (fishesFed * 100) - (int)timeTaken; // Simple scoring formula

        Debug.Log(win ? "🏆 You Win!" : "❌ You Lose!");

        int levelNumber = SceneManager.GetActiveScene().buildIndex + 1; // use actual build index

        // Log data whether win or lose
        LogPatientData(
            patientID,
            levelNumber,
            timeTaken,
            fishesFed,
            score
        );

        if (win)
        {
            winUI.SetActive(true);
            Invoke("HandleWinFlow", 2f);
        }
        else
        {
            loseUI.SetActive(true);
            Invoke("ReloadLevel", 2f);
        }
    }

    // Called only on WIN, after delay
    void HandleWinFlow()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Not last level → go to next level, same patient
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // ✅ Last level completed → move to next patient
            Debug.Log("🎉 All levels completed for " + patientID);

            patientIndex++;
            PlayerPrefs.SetInt("NextPatientIndex", patientIndex);
            PlayerPrefs.Save();

            // Restart from Level 1 (assume build index 0 is Level1)
            SceneManager.LoadScene(0);
        }
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- CSV Logger ---
    void LogPatientData(string patientID, int level, float timeTaken, int fishFed, int score)
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string entry = $"{patientID},{level},{timeTaken:F2},{fishFed},{score},{timeStamp}\n";

        try
        {
            File.AppendAllText(filePath, entry);
            Debug.Log("✅ Data Logged: " + entry);
        }
        catch (IOException ex)
        {
            Debug.LogError("⚠️ Could not write to CSV (is it open in Excel?): " + ex.Message);
        }
    }
}
