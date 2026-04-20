# Mobile AR Viewer for 3D STL Iterations

## 📌 Project Overview
This project is a mobile Augmented Reality (AR) application designed to visualize and simulate 3D model iterations in real-world physical space. It was built as a solution to bridge the gap between static 3D files (like those generated from Topology Optimization) and interactive, spatial understanding.

## 🚀 Key Features
* **Surface Detection:** Utilizes AR Foundation to scan and detect flat physical surfaces for model placement.
* **Dynamic STL Parsing:** A custom C# script reads and parses raw `.stl` files directly from local storage and converts them into Unity meshes at runtime.
* **Iteration Playback Engine:** Users can manually step through model iterations or use the "Play" feature to watch the 3D model evolve dynamically.
* **Adjustable Speed:** A custom UI slider allows users to control the speed of the iteration transitions.
* **Interactive AR Controls:** Includes dual-touch input for pinch-to-zoom and swipe-to-rotate manipulation.

## 🛠️ Technologies Used
* **Game Engine:** Unity 6
* **Framework:** AR Foundation (ARCore)
* **Language:** C#
* **Target Platform:** Android

## 📱 How to Test the App
A pre-compiled `.apk` file and a demonstration video have been uploaded to the shared project directory for the final evaluation. 
1. Download the `.apk` file to an Android device.
2. Install the application (allow "Install from Unknown Sources" if prompted).
3. Open the app, slowly pan the camera to detect a flat surface, and tap the screen to place the 3D model.
4. Use the bottom UI panel to play, pause, or reset the simulation.

## 🔮 Future Work
* **Direct File Input:** Allow users to browse their device storage to upload new STL files directly into the app in real-time.
* **Grouped Iterations:** Develop a system to group specific sets of iterations together, allowing for side-by-side comparison of different load cases in the same AR space.

---
*Developed for the Final Project Evaluation.*
