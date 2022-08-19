using UnityEngine;
using UnityEditor;
using System.IO;
 
[InitializeOnLoad]
public class PreloadCredentials
{
 
    static PreloadCredentials ()
    {
        PlayerSettings.Android.keystorePass = "KwcF5KkSQGGHlmcKuvb6ySIN4";
        PlayerSettings.Android.keyaliasName = "harvest_key_1";
        PlayerSettings.Android.keyaliasPass = "KwcF5KkSQGGHlmcKuvb6ySIN4";
    }
}