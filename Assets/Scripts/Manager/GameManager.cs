using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Values;

public class GameManager : MonoBehaviour
{
    [Header("Difficulty Related")] 
    private int level;

    [SerializeField] private float diffScale;
    public string landMarks;

    public enum Difficulty
    {
        veryEasy,
        easy,
        normal,
        hard,
        veryHard
        
        
    }

    public bool Paused
    {
        get => paused;
    }

    [SerializeField] private TextMeshProUGUI riddleBoard;
    [SerializeField]
    private bool useGPT;

    private bool paused;
    
    public bool UseGpt
    {
        get => useGPT;
        set => useGPT = value;
    }

    public Difficulty difficulty;

    public float DiffScale
    {
        get => diffScale;
        set => diffScale = value;
    }

    [SerializeField]
    private string riddle;

    public string Riddle
    {
        get => riddle;
        set => riddle = value;
    }

    public Difficulty Diff
    {
        get => difficulty;
        set => difficulty = value;
    }

    public string LandMark
    {
        get => landMarks;
        set => landMarks=value;
    }

    public string chest;

    public string Chest
    {
        get=> chest;
        set=>chest=value;
    }

    private Vector3 chestLocation;
    public Vector3 ChestLocation
    {
        get=> chestLocation;
        set=>chestLocation=value;
    }

    [Header("Scoring Related")] [SerializeField]
    private int score;

    [SerializeField]
    private int maxLives;
    private int livesLeft;

    [Header("Screens")]
    public GameObject gameOverScreen; // Assign this in the inspector
    public GameObject successScreen;  // Assign this in the inspector
    public GameObject pauseScreen;  // Assign this in the inspector
    public int Score
    {
        get => score;
        set => score = value;
    }

    public int LivesLeft
    {
        get => livesLeft;
        set => livesLeft = value;
    }

    public void PublishRiddle()
    {
        riddleBoard.text = riddle;
        FindObjectOfType<TreasureFinder>().Initiate(ChestLocation);
    }


    public void DiffScaler()
    {
        diffScale = level / 50.0f;
        if (diffScale <= 0.2f)
        {
            difficulty = Difficulty.veryEasy;
        } else if (diffScale is > 0.2f and <= 0.4f)
        {
            difficulty = Difficulty.easy;
        } else if (diffScale is > 0.4f and <= 0.6f)
        {
            difficulty = Difficulty.normal;
        } else if (diffScale is > 0.6f and <= 0.8f)
        {
            difficulty = Difficulty.hard;
        } else if (diffScale is > 0.8f and <= 1.0f)
        {
            difficulty = Difficulty.veryHard;
        } 
    }

    private void Start()
    {
        Time.timeScale = 1;
        paused = false;
        livesLeft = maxLives;
        Score = SavedValues.Score;
        level = SavedValues.Level+1;

        InfoPanelUpdate();
    }

    private void InfoPanelUpdate()
    {
        var infoPanel = FindObjectOfType<InfoPanel>();
        infoPanel.UpdateLives(livesLeft);
        infoPanel.UpdateScore(score);
        infoPanel.UpdateLevel(level);
    }

    public void DecreaseLives()
    {
        if (LivesLeft > 0)
        {
            LivesLeft--;
            Debug.Log($"Life lost! Lives left: {LivesLeft}");
            InfoPanelUpdate();

        }
        if (LivesLeft <= 0)
        {
            Debug.Log("Game Over!");
            paused = true;
            gameOverScreen.SetActive(true); 
            Time.timeScale = 0; 
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Resume()
    {
        paused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1; 
    }
    public void Pause()
    {
        paused = true;
        pauseScreen.SetActive(true); 
        Time.timeScale = 0; 
    }
    public void IncreaseScore(int increment)
    {
        Score += increment;
        Debug.Log($"Score updated: {Score}");
        paused = true;
        successScreen.SetActive(true); 
        Time.timeScale = 0;
    }

    public void CommitValues()
    {
        SavedValues.Score = Score;
        SavedValues.Level = level;
    }
    public void ResetValues()
    {
        SavedValues.Score = 0;
        SavedValues.Level = 0;
    }

    public void NextLevel()
    {
        CommitValues();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        ResetValues();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Exit()
    {
        ResetValues();
        SceneManager.LoadScene(0);
    }
    
   
}
