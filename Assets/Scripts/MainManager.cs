using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public Text PlayerNameText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private int m_BestPoints;
    private string m_PlayerName;
    private string m_BestPlayerName;

    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MainManager Start");
        LoadScore();
        LoadPlayerData();
        UpdateScoreText();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        if (MenuUIHandler.Instance != null && MenuUIHandler.Instance.MenuUI != null)
        {
            Debug.Log("Hiding Menu");
            MenuUIHandler.Instance.HideMenu();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetHighScore();
            if (MenuUIHandler.Instance != null)
            {
                MenuUIHandler.Instance.ResetHighScore();
            }


        }
            if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Menu");
                if (MenuUIHandler.Instance != null)
                {
                    Debug.Log("Showing Menu");
                    MenuUIHandler.Instance.ShowMenu(); // Muestra el menú al regresar al menú

                }
            }
        }
    }



    void AddPoint(int point)
    {
        m_Points += point;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (ScoreText != null)
        {
            ScoreText.text = $"Score : {m_Points}";
        }
        if (BestScoreText != null)
        {
            BestScoreText.text = $"Best Score : {m_BestPlayerName} : {m_BestPoints}";
        }
        if (PlayerNameText != null)
        {
            PlayerNameText.text = $"Player : {m_PlayerName}";
        }
        
    }

public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > m_BestPoints)
        {
            m_BestPoints = m_Points;
            m_BestPlayerName = m_PlayerName;
            SaveScore();
        }
        MenuUIHandler.Instance.UpdateHighScore(m_Points);

    }

    class SaveData
    {
        public int HighScore;
        public string PlayerName;
    }

    public void SaveScore()
    {
        //Con esto guarda el archivo del color creando una instancia
        SaveData data = new SaveData();
        data.HighScore = m_BestPoints;
        data.PlayerName = m_BestPlayerName;

        //transformo esa instancia en JSON con el codigo "JsonUtility.ToJson"
        string json = JsonUtility.ToJson(data);

        //Como ultimose creo "File.WriteAllText" para escrivir una cadena de archivos
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        //Este es el metodo invertido del metodo save
        string path = Application.persistentDataPath + "/savefile.json";
        //Con esto muestra si existe un archivo guardado "File.Exists"
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            //JsonUtility.FromJson con esto activara el codigo guardado
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            //Finalmente activa el color guardado y lo conecta a la partida.
            m_BestPoints = data.HighScore;
            m_BestPlayerName = data.PlayerName; // Cargas el nombre del jugador
            UpdateScoreText();

        }
    }
    void LoadPlayerData()
    {
        m_PlayerName = PlayerPrefs.GetString("PlayerName", "DefaultName");
        m_Points = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    void ResetHighScore()
    {
        m_BestPoints = 0; // Reiniciar la mejor puntuación a 0
        m_BestPlayerName = ""; // Reiniciar el nombre del jugador
        SaveScore(); // Guardar el nuevo mejor puntaje
        UpdateScoreText(); // Actualizar el texto de la puntuación
    }
}