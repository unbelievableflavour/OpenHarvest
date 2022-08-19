using BNG;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public Transform playerStartPositions;
    public BNGPlayerController playerController;

    public PlayerInvokes playerInvokes;
    public EventHandler beforeSceneSwitch;

    private Transform defaultPosition;

	public static SceneSwitcher Instance = null;

	// Initialize instance.
	private void Awake()
	{
        if (Instance != null){
            Destroy(Instance);
        }

        Instance = this;
	}

    void Start()
    {
        defaultPosition = GetDefaultSpawnPosition();
        GameState.currentSceneSwitcher = this;
        GameState.currentPlayerPosition = playerController.transform;
        SetPlayerStartPosition(GameState.enteredSceneThrough != null ? GameState.enteredSceneThrough : "Default");
    }

    public void SwitchToScene(int sceneIndex, string sceneEnterLocation)
    {
        GameState.enteredSceneThrough = sceneEnterLocation;

        FadeToBlack();
        
        beforeSceneSwitch?.Invoke(this, null);

        StartCoroutine(loadScene(sceneIndex));
    }

    IEnumerator loadScene(int sceneIndex)
    {
        yield return new WaitForSeconds(playerInvokes.screen.FadeOutSpeed);

        SceneManager.LoadScene(sceneIndex);
    }

    void FadeToBlack()
    {
        playerInvokes.screen.DoFadeIn();
    }

    void FadeFromBlack()
    {
        playerInvokes.screen.DoFadeOut();
    }

    public void Sleep()
    {
        StartCoroutine(awaitFadeDuration());
    }

    IEnumerator awaitFadeDuration()
    {
        FadeToBlack();
        yield return new WaitForSeconds(playerInvokes.screen.FadeOutSpeed);
        FadeFromBlack();
    }

    public void Respawn()
    {
        SetPlayerStartPosition(GameState.enteredSceneThrough != null ? GameState.enteredSceneThrough : "Default");
    }

    private Transform GetDefaultSpawnPosition()
    {
        foreach (Transform spawnPosition in playerStartPositions)
        {
            if (spawnPosition.name == "Default")
            {
                return spawnPosition;
            }
        }

        return null;
    }

    private void SetPlayerStartPosition(string enterLocation)
    {
        var playerTeleport = GameState.currentPlayerPosition.GetComponent<PlayerTeleport>();
        if (!playerTeleport)
        {
            return;
        }

        foreach (Transform playerPosition in playerStartPositions)
        {
            if (playerPosition.name == enterLocation)
            {
                playerTeleport.TeleportPlayerToTransform(playerPosition);
                //Teleport does the fadeOut
                return;
            }
        }

        playerTeleport.TeleportPlayerToTransform(defaultPosition);
        //Teleport does the fadeOut
    }
}
