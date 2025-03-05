using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public static MenuUIHandler Instance { get; private set; }
    public GameObject MenuUI;
    public InputField NameInputField;
    public Text ScoreText;
    public string PlayerName;
    public int PlayerScore;


    private void Start()
    {
        if (MenuUI != null)
        {
            MenuUI.SetActive(true);
        }
        LoadData();
        if (NameInputField != null)
        {
            NameInputField.text = PlayerName;
        }
        if (ScoreText != null && ScoreText.gameObject != null)
        {
            ScoreText.gameObject.SetActive(false); // Inicialmente invisible

        }
    }
    public void StartNew()
    {
        SaveData();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        SaveData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void Awake()
    {
        Debug.Log("MenuUIHandler Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveData()
    {
        PlayerName = NameInputField.text;
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.SetInt("PlayerScore", PlayerScore);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "DefaultName");
        PlayerScore = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreText(); // Llamada para actualizar el texto del marcador
        if (ScoreText != null && ScoreText.gameObject != null)
        {
            ScoreText.gameObject.SetActive(true); // Hacer visible el ScoreText

        }
    }

    public void UpdateHighScore(int newScore)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (newScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.SetString("HighScoreName", PlayerName);
            PlayerPrefs.Save();
            ScoreText.text = "High Score: " + PlayerName + " : " + newScore.ToString();
        }
    }

    public void ShowMenu()
    {
        Debug.Log("ShowMenu called");
        if (MenuUI != null)
        {
            MenuUI.SetActive(true);
        }
    }

    public void HideMenu()
    {
        Debug.Log("HideMenu called");
        if (MenuUI != null)
        {
            MenuUI.SetActive(false);
        }
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetString("HighScoreName", "DefaultName");
        PlayerPrefs.Save();
        if (ScoreText != null)
        {
            ScoreText.text = "High Score: DefaultName : 0";
        }
    }

    public void UpdateScoreText()
    {
        if (ScoreText != null)
        {
            string highScoreName = PlayerPrefs.GetString("HighScoreName", "DefaultName");
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            ScoreText.text = "High Score: " + highScoreName + " : " + highScore.ToString();
        }
    }
}