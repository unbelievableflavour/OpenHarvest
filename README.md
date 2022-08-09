# OpenHarvest

# How to install
1. Install the required unity version: 2020.3.25f1
2. Download this project.
3. Open it in unity.
4. Install all packages below from the "Required Packages" section.

## Required packages

This repository contains a lot of packages that could not be included. You will have to install them manually to get the project working.

| Package | Folder Name | Version  | #2  |
| :---:   | :-: | :-: | :-: |
| VR Interaction Framework | BNG Framework | 1.81 | https://assetstore.unity.com/packages/templates/systems/vr-interaction-framework-161066 |
| Shader Pack : Cartoon Water | ZerinLabs_shaderPack_CartoonWater | 1.1 | https://assetstore.unity.com/packages/vfx/shaders/shader-pack-cartoon-water-178978 |
| Free Pixel Font - Thaleah | Thaleah_PixelFont | 1.1 | https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059 |
| Oculus Integration | Oculus | 38.0 | https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022 |
| Hierarchy Folders | Sisus| 1.2.6 | https://assetstore.unity.com/packages/tools/utilities/hierarchy-folders-157716 |
| SUPER Character Controller | FirstPerson AIO Pack | 2022.6.4f | https://assetstore.unity.com/packages/tools/game-toolkits/super-character-controller-135316 |

Put these in the "Assets/External” Folder.

# Custom URP

This application runs on a custom version of the Universal Rendering Pipeline specifically to run Application Spacewarp. When upgrading unity to a new major release, the custom URP should also be updated.
Current pipeline can be found here:
https://github.com/Oculus-VR/Unity-Graphics/tree/2020.3/oculus-app-spacewarp

2021 branch can be found here:
https://github.com/Oculus-VR/Unity-Graphics/tree/2021.2/oculus-app-spacewarp

# Running on Windows
The game DOES NOT RUN ON WINDOWS WITH APPLICATION SPACEWARP ENABLED. Make sure to disable it during development.
`Edit > Project Settings > XR Plug-in Management > Oculus > Android > Experimental > Application Spacewarp >` uncheck the box. (Dont forget to recheck it before commiting)

# Building for Android
When you build for android it's gonna give an error about a missing keystore. You will have to readd it manually in the settings.
`Edit > Project Settings > Player > Publishing Settings > Project keystore > select` (It's located under `Assets > keystore > harvest_vr.keystore`)
`Edit > Project Settings > Player > Publishing Settings > Project keystore > password` (It's located under `Assets > keystore > keystore_password`)
`Edit > Project Settings > Player > Publishing Settings > Project key > password `(It's located under `Assets > keystore > keystore_password`)

# Credits

* Sound effects obtained from Zapsplat - https://www.zapsplat.com
* Some icons obtained from Flaticon - www.flaticon.com

# Todo's

Deprecated scripts are still necessary because old save files depend on them (Tip: Write a migration to just save in a JSON file without an assembly, then we would never have this issue ever again!).
