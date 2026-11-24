# LSystemCourseworkL-System Coursework
Author: Xinan Li
Module: MATHEMATICS FOR GAMES AND V/AR

Submission Contents

WindowsBuild: Standalone Windows application and its required files.
UnityProject: Unity project source, including Assets, Packages and ProjectSettings.
Demonstration video: (https://xixiliii.itch.io/l-system-coursework-xinan-li)

Running the Application

Open the WindowsBuild folder and double-click LSystemCoursework.exe.

Keep all files and subfolders in WindowsBuild together.
If downloaded as a ZIP file, extract it before running the application.

Controls

1–6: Select Plant A–F.
7: Select the Koch Curve.
8: Select the Dragon Curve.

Left Arrow: Decrease the iteration.
Right Arrow: Increase the iteration.
Q / E: Decrease / increase the turning angle.
Z / X: Decrease / increase the segment length.

The on-screen buttons provide the same controls.

Auto Fit Camera: Automatically fit the model to the view.
Fit View: Fit the current model to the view.
Use Branch Colors: Toggle colouring based on branch depth.

Changing the segment length disables automatic camera fitting so that changes in size remain visible. Selecting another model restores its default settings.

Use Alt + F4 to close the application.

Opening the Unity Project

Unity version used: Unity 6.1 (6000.1.13f1).

In Unity Hub, add the UnityProject folder as an existing project.
Open Assets/Scenes/SampleScene.unity and press Play.

Configuration files are located in Assets/Resources/LSystems.
C# source files are located in Assets/Script.

Notes

Iteration limits depend on the selected model.
Plant D, E and F have no visible line segments at iteration 0 because their initial symbol X is not a drawing command.
