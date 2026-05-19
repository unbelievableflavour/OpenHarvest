using BNG;
using UnityEngine;
using UnityEngine.UI;

public class WateringCanController : MonoBehaviour
{
    [Header("References")]
    public GameObject spawnedObject;
    public GameObject pouringParticle;
    public Transform spawnLocation;
    public AudioSource pouringSound;

    [Header("Pouring angles")]
    public int minimalPouringAngle = 30;
    public int maximumPouringAngle = 180;

    [Header("Water")]
    public int waterAmount = 10;
    public Text waterAmountIndicator;
    public GameObject waterMesh;

    private int refillWaterAmount = 10;

    private Grabbable grabbable;
    private float timer;

    private void Start()
    {
        grabbable = GetComponent<Grabbable>();
        UpdateWaterAmount();
    }

    void Update()
    {
        if (grabbable && !grabbable.BeingHeld)
        {
            pouringSound.volume = 0;
            pouringParticle.SetActive(false);
            return;
        }

        if(waterAmount == 0)
        {
            pouringSound.volume = 0;
            pouringParticle.SetActive(false);
            return;
        }

        timer += Time.deltaTime;
        if (timer < 0.5)
        {
            return;
        }
        timer = 0;

        if (this.transform.rotation.eulerAngles.x > minimalPouringAngle && this.transform.rotation.eulerAngles.x < maximumPouringAngle)
        {
            pouringSound.volume = 1;
            pouringParticle.SetActive(true);
            GameObject water = Instantiate(spawnedObject, spawnLocation.position, spawnLocation.rotation);
            water.GetComponent<WaterDropColliderController>().SetWateringCanController(this);

            Destroy(water, 2.0f);
            return;
        }
        pouringSound.volume = 0;
        pouringParticle.SetActive(false);
    }

    public void DecreaseWaterAmount()
    {
        if(waterAmount > 0)
        {
            waterAmount--;
            UpdateWaterAmount();
            return;
        }

        if(waterAmount < 0)
        {
            waterAmount = 0;
            UpdateWaterAmount();
        }
    }

    public void Refill()
    {
        waterAmount = refillWaterAmount;
        UpdateWaterAmount();
    }

    private void UpdateWaterAmount()
    {
        waterAmountIndicator.text = waterAmount.ToString();
        waterMesh.SetActive(waterAmount == 0 ? false: true);
    }
}
