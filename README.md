# AR Cooking Guide (HoloLens 2 · Unity · MRTK3)

A simple Mixed Reality cooking assistant built for **HoloLens 2** using\
**Unity 2022.3.62f2**, **MRTK3**, and **OpenXR**.

This repository contains the base Unity project structure for
development.

------------------------------------------------------------------------

## 🔧 Unity Version (Required)

To open this project correctly, please install:

**Unity Version: 2022.3.62f2 (LTS)**\
\> All team members must use the *exact same version* to avoid package
conflicts.

------------------------------------------------------------------------

## 📂 Project Structure

    Assets/
    Packages/
    ProjectSettings/

These three folders are required for Unity to recognize the project.

------------------------------------------------------------------------

## ▶️ How to Open the Project (Quick Guide)

1.  Clone this repository

        git clone https://github.com/<your-repo>

2.  Open **Unity Hub**

3.  Click **Open**

4.  Select the project folder (where
    `Assets / Packages / ProjectSettings` exist)

5.  Unity will load packages automatically

6.  After loading completes → open the main scene and press **Play**

------------------------------------------------------------------------

## 📦 Required Unity Packages

These are included automatically via `Packages/manifest.json`: - MRTK3
(Core, UX, Input) - OpenXR Plugin - TextMeshPro - Input System

No manual installation is needed unless Unity prompts for updates.

------------------------------------------------------------------------

## 📱 Build for HoloLens 2 (Optional)

-   Platform: **UWP**
-   Architecture: **ARM64**
-   Build in Unity → Open in Visual Studio → Deploy to device

------------------------------------------------------------------------

## 📄 Notes

-   If Unity shows errors, ensure:
    -   Unity version matches **2022.3.62f2**
    -   No missing packages in Package Manager
    -   ProjectSettings folder is intact

------------------------------------------------------------------------

## 🌱 Branch Strategy

- **main**  
  Stable release branch. Contains production-ready builds.

- **dev**  
  Integration branch. All features are merged here before going to main.

- **feature/***  
  Each team member creates a feature branch from `dev`, e.g.:

  `feature/step-panel-ui`  
  `feature/recipe-json-loader`  
  `feature/voice-command`  

### Workflow
1. Checkout dev  
2. Create your feature branch  
3. Commit & push  
4. Merge back into dev after review

------------------------------------------------------------------------
