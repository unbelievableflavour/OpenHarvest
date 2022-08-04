using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayNightController : MonoBehaviour
{
    public AudioClip nightSoundtrack;
    public AudioClip daySoundTrack;

    public NightLightController nightLightController;

    public Color dayColor = Color.white;
    public Color nightColor = Color.blue;
    private Color endColor = Color.blue;
    public float fadeTime = 5.0f;
   //public Material skyBoxDayMaterial;
    //public Material skyBoxNightMaterial;
    public Image moon;

    private Color currentColor;
    private float timer;

    public Renderer stars;
    private Color visibleColor;
    private Color invisibleColor = new Color(0, 0, 0, 0);
    private Material starsMaterial;    // Used to store material reference.
    private Color starsEndColor;            // Used to store color reference.
    private Color starsCurrentColor;            // Used to store color reference.

    private void Start()
    {
        starsMaterial = stars.material;
        visibleColor = starsMaterial.color;

        timer = 5;// Initialize timer to skip the first if in update

        if (TimeController.isNight())
        {
            this.GetComponent<Light>().color = nightColor;
            currentColor = this.GetComponent<Light>().color;
            //RenderSettings.skybox = skyBoxDayMaterial;
            moon.CrossFadeAlpha(1.0f, 0.0f, false);
            starsMaterial.color = visibleColor;

            if (daySoundTrack && nightSoundtrack)
            {
                AudioManager.Instance.PlayMusic(nightSoundtrack);
            }
        }
        else
        {
            if (nightLightController)
            {
                nightLightController.DisableLights();
            }

            this.GetComponent<Light>().color = dayColor;
            currentColor = this.GetComponent<Light>().color;
            moon.CrossFadeAlpha(0.0f, 0.0f, false);
            if (daySoundTrack && nightSoundtrack)
            {
                AudioManager.Instance.PlayMusic(daySoundTrack);
            }
            starsMaterial.color = invisibleColor;
        }
    }

    void SetNight()
    {
        if (nightLightController)
        {
            nightLightController.EnableLights();
        }

        starsCurrentColor = starsMaterial.color;
        starsEndColor = visibleColor;

        if (daySoundTrack && nightSoundtrack)
        {
            AudioManager.Instance.PlayMusic(nightSoundtrack);
        }

        currentColor = this.GetComponent<Light>().color;
        endColor = nightColor;
        FadeToMoon();
        timer = 0; // reset timer
    }

    void SetDay()
    {
        if (nightLightController)
        {
            nightLightController.DisableLights();
        }
        
        starsCurrentColor = starsMaterial.color;
        starsEndColor = invisibleColor;

        if (daySoundTrack && nightSoundtrack)
        {
            AudioManager.Instance.PlayMusic(daySoundTrack);
        }

        currentColor = this.GetComponent<Light>().color;
        endColor = dayColor;
        FadeFromMoon();
        timer = 0; // reset timer
    }

    void Update()
    {
        if (timer <= 1)
        {
            timer += Time.deltaTime / fadeTime;
            this.GetComponent<Light>().color = Color.Lerp(currentColor, endColor, timer);
            starsMaterial.color = Color.Lerp(starsCurrentColor, starsEndColor, timer);
            return;
        }

        if (TimeController.isDay() && currentColor != dayColor)
        {
            SetDay();
            return;
        }

        if (TimeController.isNight() && currentColor != nightColor)
        {
            SetNight();
            return;
        }
    }

    void FadeToMoon()
    {
        moon.CrossFadeAlpha(1.0f, fadeTime, false);
    }

    void FadeFromMoon()
    {
        moon.CrossFadeAlpha(0.0f, fadeTime, false);     
    }
}
