using System.Collections.Generic;
using UnityEngine;

public class SceneDetails : ItemOfTheWeek
{
    public bool usesWeather;
}

public static class SceneInformationManager
{
    private static Dictionary<int, SceneDetails> scenes = new Dictionary<int, SceneDetails>
    {
        {0, new SceneDetails() { usesWeather = false } }, // Scenes/MainMenu
        {1, new SceneDetails() { usesWeather = true } }, // Scenes/Farm
        {2, new SceneDetails() { usesWeather = true } }, // Scenes/Village
        {3, new SceneDetails() { usesWeather = false } }, // Scenes/Home
        {4, new SceneDetails() { usesWeather = false } }, // Scenes/StoreSeeds
        {5, new SceneDetails() { usesWeather = false } }, // Scenes/StoreTools
        {6, new SceneDetails() { usesWeather = false } }, // Scenes/StoreArtisan
        {7, new SceneDetails() { usesWeather = false } }, // Scenes/GuideBook
        {8, new SceneDetails() { usesWeather = false } }, // Scenes/StoreAnimal
        {9, new SceneDetails() { usesWeather = false } }, // Scenes/StoreDecorations
        {10, new SceneDetails() { usesWeather = true } }, // Scenes/Beach
        {11, new SceneDetails() { usesWeather = false } }, // Scenes/StoreFishing
        {12, new SceneDetails() { usesWeather = false } }, // Scenes/Mine
        {13, new SceneDetails() { usesWeather = false } }, // Scenes/Greenhouse
        {14, new SceneDetails() { usesWeather = false } }, // Scenes/Windmill
        {15, new SceneDetails() { usesWeather = true } }, // Scenes/EventChristmas
        {16, new SceneDetails() { usesWeather = false } }, // Scenes/StoreArt
        {17, new SceneDetails() { usesWeather = true } }, // Scenes/Ranch
        {18, new SceneDetails() { usesWeather = true } }, // Scenes/Sandbox
        {19, new SceneDetails() { usesWeather = false } }, // Scenes/MagicMine
    };

    public static SceneDetails getSceneDetailsByIndex(int index)
    {
        if(!scenes.ContainsKey(index)) {
            Debug.Log("not yet in dictionary");
            return new SceneDetails();
        }

        return scenes[index];
    }

    public static int getSceneCount()
    {
        return scenes.Count;
    }
}
