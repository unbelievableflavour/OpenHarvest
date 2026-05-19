using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameArchery : MonoBehaviour
{
    public SpawnInArea enemySpawner;
    public Text hitsIndicator;
    public Text gameLengthIndicator;
    public float gameLength = 30.0f;
    private AudioSource audioSource;
    public AudioClip soundOnMiniGameStarted;
    public AudioClip soundOnMiniGameEnded;

    private float backupGameLength = 0f;

    private int killsTotal;
    private bool gameInSession;
    // Start is called before the first frame update
    void Start()
    {
        killsTotal = 0;
        gameInSession = false;
        backupGameLength = gameLength;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!gameInSession)
        {
            return;
        }

        gameLength -= Time.deltaTime;

        UpdateGameLengthLabel();
        if (gameLength < 0)
        {
            EndWave();
        }
    }

    public void StartWave()
    {
        if(gameInSession == true)
        {
            EndWave();
            return;
        }

        playSound(soundOnMiniGameStarted);
        killsTotal = 0;
        gameInSession = true;
        hitsIndicator.text = "0";
        enemySpawner.SpawnEnemy();
        return;
    }

    private void EndWave()
    {
        playSound(soundOnMiniGameEnded);
        gameInSession = false;
        UpdateGameLengthLabel();
        gameLength = backupGameLength;
        enemySpawner.RemoveActiveEnemies();   
    }

    public void handleDestroyTarget()
    {
        killsTotal++;
        UpdateHitsLabel();
        enemySpawner.SpawnEnemy();
    }

    void UpdateHitsLabel()
    {
        hitsIndicator.text = killsTotal.ToString();
    }

    void UpdateGameLengthLabel()
    {
        gameLengthIndicator.text = ((int)gameLength).ToString();
    }

    void playSound(AudioClip clip)
    {
        if (audioSource)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
