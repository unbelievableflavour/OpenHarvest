using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance = null;

    public ParticleSystem dustParticles;
    public ParticleSystem rainParticles;

    public static string RAINY = "rainy";
    public static string SUNNY = "sunny";

    public static string weatherToday = SUNNY; // Starting weather
    public static string weatherTomorrow = RAINY; // Starting weather of second day
    
    private bool currentSceneUsesWeather = true;

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentSceneUsesWeather = SceneInformationManager.getSceneDetailsByIndex(sceneIndex).usesWeather;

        TimeController.Instance.ListenToDayChange(handleNewDayStarted);
        SetWeather(weatherToday);
    }

    void UpdateSeason()
    {
        SetWeather(GetWeatherForToday());
        SetWeatherForTomorrow();
    }

    string GetWeatherForToday()
    {
        return weatherTomorrow;
    }

    void SetWeatherForTomorrow() {
        weatherTomorrow = PickRandomWeather();
    }

    string PickRandomWeather()
    {
        List<string> possibleWeathers = new List<string>();

        possibleWeathers.Add(SUNNY);
        possibleWeathers.Add(RAINY);
        
        return possibleWeathers[UnityEngine.Random.Range(0, possibleWeathers.Count)];
    }

    void SetWeather(string newWeather)
    {
        weatherToday = newWeather;
        StopAllWeather();

        if (!currentSceneUsesWeather)
        {
            return;
        }

        dustParticles.Play();
        return;

        //Komt nog!
        if (newWeather == SUNNY)
        {
            dustParticles.Play();
            return;
        }
        if (newWeather == RAINY)
        {
            WaterAllPatches();
            rainParticles.Play();
            return;
        }
    }

    void StopAllWeather()
    {
        dustParticles.Stop();
        rainParticles.Stop();
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        UpdateSeason();
    }

    public void WaterAllPatches()
    {
        //not implemented yet. Only soilgrid for now.
    }
}
