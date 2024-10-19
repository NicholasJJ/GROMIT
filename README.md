# SceneMancer
IMPORTANT: the customGPT component in the gptTest gameobject has a field called apiKey. This is supposed to have the api key associated with an openAI account, and currently has my personal key.
so please don't charge a huge tab if you can avoid it.
The main Scene is called 'humanoid', The 'manager' gameobject handles all the chatgpt calls, and the 'world' gameobject makes the scene graph json.
The scene graph logic is split between the 'textTree' component in worldGeometry, and 'textDescription' components attarched to the relevant gameobjects.

## How to run this project
Video Tutorial Here: https://youtu.be/OM7mJw4I61Q
to run the system, download and install the Unity3D download manager UnityHub from their website here: https://unity.com/download
then, use UnityHub to download unity version 2021.3.8f1. In the projects tab of UnityHub click Add->Add Project From Disk, and select the github repo. This should add the unity project to your system.

Once you launch the unity project, navigate to the Humanoid or Traffic scenes. They're both in the file system under Assets/Scenes/
To run the system, click the "play" arrow on the top of the unity editor to start the scene. The Humanoid scene has standard FPS controls, and the traffic scene controls like a car.
To activate scenemancer, hold down the spacebar and speak into the microphone. If you're in the Humanoid scene, you can "point" at objects by looking at them and pressing 'e'.
We usually use the phrase "make the apple spin" as our "hello world" to make sure the system is working properly. If you ask scenemance to spin the apple and the apple doesn't spin, something has been set up wrong.

## Other Notes
This class project was built on top of an existing repository that was made prior to the beginning of the semester. There may still be some code in the repository (mostly relating to procedural animation) that isn't scenemancer related.
Also, we are intending to continue working on this project over winter break in the hopes of eventually publishing a paper on it. We will try not to push any changes to the main branch.
