using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int score;
    public GameObject enemy, player;
    public int numObjects = 10;
    public TextMeshProUGUI scoreText, gameOverText, pauseText, counting;
    public Button restartButton, resumeButton, soundButton, fireButton, boostButton, respawnButton;
    public static bool playerDead = false;
    CanvasGroup restartGroup, respawnGroup;
    public GameObject enemySpawner, gameCanvas, menuCanvas, creditsCanvas;
    Player revive;
    bool muted = false;
    public static bool isPlaying = false;
    public Sprite soundOn, soundOff;
    AudioManager audioManager;
    public AudioMixer audioMixer;
    EnemySpawner clones;
    private void Start()
    {
        clones = enemySpawner.GetComponent<EnemySpawner>();
        restartGroup = restartButton.GetComponent<CanvasGroup>();
        respawnGroup = respawnButton.GetComponent<CanvasGroup>();
        revive = player.GetComponent<Player>();
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No AudioManager");
        }
    }
    private void Update()
    {
        scoreText.text = "Score: " + score;
        if(playerDead == true)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.alpha += Time.deltaTime / 2;
            restartButton.gameObject.SetActive(true);
            restartGroup.alpha += Time.deltaTime / 2;
            respawnButton.gameObject.SetActive(true);
            respawnGroup.alpha += Time.deltaTime / 2;
        }
        else
        {
            gameOverText.gameObject.SetActive(false);
        }
    }  
    public void ReviveGame()
    {
        AdManager.ShowRewardedAd(AdRevive, AdSkip, AdFailed);        
    }
    public void StartGame()
    {
        gameOverText.alpha = 0;
        restartGroup.alpha = 0;
        respawnGroup.alpha = 0;
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        respawnButton.gameObject.SetActive(false);
        isPlaying = true;
        menuCanvas.SetActive(false);
        enemySpawner.SetActive(true);
        gameCanvas.SetActive(true);
    }
    public void Settings()
    {
        fireButton.GetComponent<Button>().enabled = false;
        boostButton.GetComponent<Button>().enabled = false;
        pauseText.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Mute()
    {
        if(!muted)
        {
            audioMixer.SetFloat("volume", -80);
            soundButton.image.sprite = soundOff;
            muted = true;
        }
        else
        {
            audioMixer.SetFloat("volume", 0);
            soundButton.image.sprite = soundOn;
            muted = false;
        }
        
    }
    public void ResumeGame()
    {
        pauseText.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        StartCoroutine("ResumeCounting");
    }
    IEnumerator ResumeCounting()
    {
        counting.gameObject.SetActive(true);
        counting.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        counting.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        counting.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        counting.gameObject.SetActive(false);
        fireButton.GetComponent<Button>().enabled = true;
        boostButton.GetComponent<Button>().enabled = true;
        Time.timeScale = 1f;

    }
    void AdRevive()
    {
        gameOverText.alpha = 0;
        restartGroup.alpha = 0;
        restartButton.gameObject.SetActive(false);
        respawnGroup.alpha = 0;
        respawnButton.gameObject.SetActive(false);
        fireButton.GetComponent<Button>().enabled = true;
        boostButton.GetComponent<Button>().enabled = true;
        player.SetActive(true);
        playerDead = false;
        audioManager.PlaySound("respawn");
        revive.Revive();
    }
    void AdSkip()
    {
        Debug.Log("Ad skipped");
    }
    void AdFailed()
    {
        Debug.Log("Ad failed to load");
    }
    public void RestartGame()
    {                
        isPlaying = false;
        playerDead = false;
        score = 0;
        EnemySpawner.enemyCount = 0;
        EnemySpawner.waveNumber = 0;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void CreditsButton()
    {
        menuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }
    public void BackButton()
    {
        menuCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }
}
