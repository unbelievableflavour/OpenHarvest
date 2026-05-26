using System.Collections;
using UnityEngine;
using static Definitions;

public class SoilBehaviour : MonoBehaviour
{
    private int count = 0;

    public Transform Tiles;
    public PlantManager plantManager;
    public GameObject breakableObjectPrefab;
    public UnityEngine.Events.UnityEvent beforeStateChange;
    public UnityEngine.Events.UnityEvent beforeWateredChange;

    private GameObject spawnedObject;

    [Header("States")]
    public GameObject m_GrassTile;
    public GameObject m_ShoveledTile;
    public GameObject m_PlowedTile;

    private bool isWatered;
    private SoilStates m_currentState;

    [Header("Colors")]
    public Color dryGroundColor;
    public Color wetGroundColor;

    void Awake()
    {
        m_GrassTile.SetActive(true);

        if (breakableObjectPrefab)
        {
            m_currentState = SoilStates.InitializedWithObject;
            spawnedObject = Instantiate(breakableObjectPrefab, this.transform.position, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
            return;
        }

        m_currentState = SoilStates.Initialized;
        return;
    }

    public void OnTriggerEnter(Collider other) {
        var collider_tag = other.tag;

        if (m_currentState == SoilStates.InitializedWithObject)
        {
            if (spawnedObject)
            {
                return;
            }

            m_currentState = SoilStates.Initialized;
        }

        if (collider_tag == "Shovel"){
            ShovelSoilPatch();
            return;
        }

        if (m_currentState == SoilStates.Shoveled){
            if (collider_tag == "Hoe"){
                PlowSoilPatch();
                return;
            }

            if (collider_tag == "Water")
            {
                NotifyParentToDecreaseWaterAmount(other.gameObject);
                setWatered(true);
                return;
            }
        }

        if (m_currentState == SoilStates.Plowed){
            if (collider_tag == "Seed"){
                NotifyParentToDecreaseSeedsAmount(other.gameObject);
                SowSoilPatch(other.name);
                return;
            }

            if (collider_tag == "Water")
            {
                NotifyParentToDecreaseWaterAmount(other.gameObject);
                setWatered(true);
                return;
            }
        }

        if (m_currentState == SoilStates.Sowed)
        {
            if (collider_tag == "Water")
            {
                NotifyParentToDecreaseWaterAmount(other.gameObject);
                WaterSoilPatch();
                return;
            }
        }
    }

    void ShovelSoilPatch(){
        AudioManager.Instance.PlayClip("shovel");

        if (count != 3) {
            count++;
            return;
        }

        count = 0;

        setCurrentState(SoilStates.Shoveled);
        plantManager.Reset();
    }

    void PlowSoilPatch(){
        AudioManager.Instance.PlayClip("hoe");

        if (count != 3)
        {
            count++;
            return;
        }

        count = 0;

        setCurrentState(SoilStates.Plowed);
    }

    void SowSoilPatch(string plant_type){
        AudioManager.Instance.PlayClip("seed");
        plantManager.SpawnPlant(plant_type);

        if (isWatered)
        {
            WaterSoilPatch();
        }

        setCurrentState(SoilStates.Sowed);
    }

    void WaterSoilPatch() {
        setWatered(true);
        plantManager.Water();
    }

    public SoilStates getCurrentState(){

        if(m_currentState == SoilStates.InitializedWithObject && !spawnedObject)
        {
            Debug.Log("Was destroyed. Return initialized");
            return SoilStates.Initialized;
        }

        return m_currentState;
    }

    public void setCurrentState(SoilStates patchState, bool callInvoke = true)
    {
        DeactivateAllTiles();

        if (beforeStateChange != null && callInvoke)
        {
            beforeStateChange.Invoke();
        }

        if (patchState == SoilStates.Initialized)
        {
            m_currentState = patchState;
            m_GrassTile.SetActive(true);
            return;
        }

        if (patchState == SoilStates.InitializedWithObject)
        {
            m_currentState = patchState;
            m_GrassTile.SetActive(true);

            if (breakableObjectPrefab)
            {
                spawnedObject = Instantiate(breakableObjectPrefab, this.transform.position, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
            }
            return;
        }

        if (patchState == SoilStates.Shoveled)
        {
            m_currentState = patchState;

            SetTile(m_ShoveledTile);
            return;
        }
        if (patchState == SoilStates.Plowed)
        {
            m_currentState = patchState;

            SetTile(m_PlowedTile);
            return;
        }

        if (patchState == SoilStates.Sowed)
        {
            m_currentState = patchState;

            SetTile(m_PlowedTile);
            return;
        }
    }

    public void setWatered(bool isWatered)
    {
        if (beforeWateredChange != null && this.isWatered != isWatered)
        {
            beforeWateredChange.Invoke();
        }

        this.isWatered = isWatered;
        setCurrentState(getCurrentState(), false);
    }

    public bool getWatered()
    {
        return isWatered;
    }

    private void DeactivateAllTiles()
    {
        m_GrassTile.SetActive(false);
        m_ShoveledTile.SetActive(false);
        m_PlowedTile.SetActive(false);
        if (spawnedObject)
        {
            Destroy(spawnedObject);
        }
    }
    public void SetTile(GameObject tile)
    {
        tile.SetActive(true);

        if (isWatered)
        {
            StartCoroutine(StartWetFadeIn(tile, 2f));
            return;
        }

        StartCoroutine(StartDryFadeIn(tile, 2f));
    }

    public IEnumerator StartWetFadeIn(GameObject wetTile, float duration)
    {
        float currentTime = 0;
        Renderer renderer = wetTile.GetComponentInChildren<Renderer>();
        if (!renderer) {
            yield break;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime / duration;
            renderer.material.color = Color.Lerp(renderer.material.color, wetGroundColor, currentTime);
            yield return null;
        }

        yield break;
    }

    public IEnumerator StartDryFadeIn(GameObject wetTile, float duration)
    {
        float currentTime = 0;
        Renderer renderer = wetTile.GetComponentInChildren<Renderer>();
        if (!renderer)
        {
            yield break;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime / duration;
            renderer.material.color = Color.Lerp(renderer.material.color, dryGroundColor, currentTime);
            yield return null;
        }

        yield break;
    }

    private void NotifyParentToDecreaseWaterAmount(GameObject colliderObject)
    {
        if (!getWatered() && colliderObject.GetComponent<WaterDropColliderController>()){
            colliderObject.GetComponent<WaterDropColliderController>().NotifyWateringCan();
        }
    }

    private void NotifyParentToDecreaseSeedsAmount(GameObject colliderObject)
    {
        if (colliderObject.GetComponent<SeedDropColliderController>())
        {
            colliderObject.GetComponent<SeedDropColliderController>().NotifyParentSeedBag();
        }
    }

    public void Reset()
    {
        DeactivateAllTiles();
        Awake();
    }
}
