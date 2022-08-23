# OpenHarvest

# How to install
1. Install the required unity version: 2020.3.25f1
2. Download this project.
3. Open it in unity.
4. Install all packages below from the "Required Packages" section.

## Required packages

This repository contains a lot of packages that could not be included. You will have to install them manually to get the project working.

| Package | Folder Name | Version  | Url  |
| :---:   | :---: | :---: | :---: |
| VR Interaction Framework | BNG Framework | 1.81 | https://assetstore.unity.com/packages/templates/systems/vr-interaction-framework-161066 |
| Shader Pack : Cartoon Water | ZerinLabs_shaderPack_CartoonWater | 1.1 | https://assetstore.unity.com/packages/vfx/shaders/shader-pack-cartoon-water-178978 |
| Oculus Integration | Oculus | 38.0 | https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022 |

Put these in the "Assets/External” Folder.

# Custom URP

This application runs on a custom version of the Universal Rendering Pipeline specifically to run Application Spacewarp. When upgrading unity to a new major release, the custom URP should also be updated.
Current pipeline can be found here:
https://github.com/Oculus-VR/Unity-Graphics/tree/2020.3/oculus-app-spacewarp

2021 branch can be found here:
https://github.com/Oculus-VR/Unity-Graphics/tree/2021.2/oculus-app-spacewarp

# Platforms

Open Harvest can build for Windows & Android.

## Running on Windows
The game DOES NOT RUN ON WINDOWS WITH APPLICATION SPACEWARP ENABLED. Make sure to disable it during development.
`Edit > Project Settings > XR Plug-in Management > Oculus > Android > Experimental > Application Spacewarp >` uncheck the box. (Dont forget to recheck it before commiting)

# Harvest Settings

Harvest specific settings can be found in a scriptable object under `Assets/ScriptableObjects/HarvestSettings`.

| Setting | Description |
| :---: | :---: |
| Is PC Mode | Plays harvest in Pancake mode. |
| Enable Sandbox | Makes the Sandbox scene accessible through main menu. |

# Credits

## Packages

Thank you devs for offering these awesome packages and letting us include them in our project.

| Package | Folder Name | Author | Version  | Url  |
| :---:   | :---: | :---: | :---: | :---: |
| Free Pixel Font - Thaleah | Thaleah_PixelFont | TinyWorlds | 1.1 | https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059 |
| Modular First Person Controller | ModularFirstPersonController | JeCase | 1.0.1 | https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884 |

## Other credits

* Sound effects obtained from Zapsplat - https://www.zapsplat.com
* Some icons obtained from Flaticon - www.flaticon.com
