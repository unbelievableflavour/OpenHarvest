using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public Transform playerStartPositions;

    public PlayerInvokes playerInvokes { get; private set; }
    public EventHandler beforeSceneSwitch;
    public bool invokeHooksInThisScene = true;
    // this is used to stop hooks for instance in main menu and guidebook to block saving items

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
        Transform rig = FindCurrentPlayerRigTransform();
        GameState.Instance.currentPlayerPosition = rig;
        playerInvokes = rig?.root?.GetComponentInChildren<PlayerInvokes>(true);
        SetPlayerStartPosition(GameState.Instance.enteredSceneThrough ?? "Default");
    }

    public void SwitchToScene(int sceneIndex, string sceneEnterLocation)
    {
        GameState.Instance.enteredSceneThrough = sceneEnterLocation;

        FadeToBlack();
        
        if (invokeHooksInThisScene) {
            beforeSceneSwitch?.Invoke(this, null);
        }

        StartCoroutine(loadScene(sceneIndex));
    }

    IEnumerator loadScene(int sceneIndex)
    {
        yield return new WaitForSeconds(playerInvokes?.screen?.FadeOutSpeed ?? 0f);

        SceneManager.LoadScene(sceneIndex);
    }

    void FadeToBlack()
    {
        playerInvokes?.screen?.DoFadeIn();
    }

    void FadeFromBlack()
    {
        playerInvokes?.screen?.DoFadeOut();
    }

    public void Sleep()
    {
        StartCoroutine(awaitFadeDuration());
    }

    IEnumerator awaitFadeDuration()
    {
        FadeToBlack();
        yield return new WaitForSeconds(playerInvokes?.screen?.FadeOutSpeed ?? 0f);
        FadeFromBlack();
    }

    public void Respawn()
    {
        SetPlayerStartPosition(GameState.Instance.enteredSceneThrough ?? "Default");
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
        if (IsFps() && GameState.Instance.currentPlayerPosition != null)
        {
            Transform root = GameState.Instance.currentPlayerPosition;
            foreach (Transform playerPosition in playerStartPositions)
            {
                if (playerPosition.name == enterLocation)
                {
                    root.SetPositionAndRotation(playerPosition.position, playerPosition.rotation);
                    return;
                }
            }

            root.SetPositionAndRotation(defaultPosition.position, defaultPosition.rotation);
            return;
        }

        BNG.PlayerTeleport playerTeleport = GameState.Instance.currentPlayerPosition?.GetComponent<BNG.PlayerTeleport>();
        if (!playerTeleport)
        {
            return;
        }

        foreach (Transform playerPosition in playerStartPositions)
        {
            if (playerPosition.name == enterLocation)
            {
                playerTeleport.TeleportPlayerToTransform(playerPosition);
                // Teleport does the fadeOut
                return;
            }
        }

        playerTeleport.TeleportPlayerToTransform(defaultPosition);
        // Teleport does the fadeOut
    }

    private static bool IsFps()
    {
        return HarvestInputManager.Instance?.harvestSettings?.playerMode == PlayerMode.FPS;
    }

    /// <summary>VR: first <see cref="BNG.BNGPlayerController"/> in the scene. FPS: first <see cref="FirstPersonController"/>.</summary>
    private static Transform FindCurrentPlayerRigTransform()
    {
        if (HarvestInputManager.Instance?.harvestSettings?.playerMode == PlayerMode.FPS)
        {
            return FindFirstObjectByType<FirstPersonController>()?.transform;
        }

        return FindFirstObjectByType<BNG.BNGPlayerController>()?.transform;
    }
}
