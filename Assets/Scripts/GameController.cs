using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public GameObject restartButton;

    private int score;
    private bool gameOver;
    private bool restart;

    void Awake() {
        // 设置游戏为窗口模式，分辨率为分辨率为600x900
        //Screen.SetResolution(600, 900, false);
    }

    void Start() {
        restart = false;
        gameOver = false;
        restartText.text = "";
        gameOverText.text = "";
        restartButton.SetActive(false);
        score = 0;
        UpdateScore();
        StartCoroutine( SpawnWaves() );
    }

    void Update() {
        if (restart) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    IEnumerator SpawnWaves() {
        yield return new WaitForSeconds(startWait);

        while (!gameOver) {
            for (int i = 0; i < hazardCount; i++) {
                GameObject hazard = hazards[Random.Range(0,hazards.Length)];
                float size = Random.Range(0.7f, 1.8f);
                if ( !hazard.name.Contains("Enemy Ship") ) {
                    hazard.transform.localScale = new Vector3(size, size, size);
                }
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }

            yield return new WaitForSeconds(waveWait);
        }

        restartText.text = "按 'R' 键重新开始";
        restartButton.SetActive(true);
        restart = true;
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore() {
        scoreText.text = "得分: " + score;
    }

    public void GameOver() {
        gameOverText.text = "游戏结束!";
        gameOver = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
