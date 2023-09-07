# Geophysical-flows-in-VR
This Github contains the UNITY project and files and the Python scripts to suplement the to the paper titled “Virtual Reality Visualization of Geophysical Flows: A framework “. 
## 1. Python Scripts 
This folder contains 3 python scripts namely Read_CSV_2D, Read_CSV_3D, and ConcaveHull.py. To use these scripts, one has to use Anaconda distribution (see https://www.anaconda.com/ ) and install all necessary python packages. In addition, the ConcaveHull.py should be always in the same folder with Read_CSV_3D.py and imported as a packaged. We enabled a user to use a simple GUI to manage the input files and output folder. Running either of the Read CSV scripts prompts the user to enter the original folder which contains the CSV files and the destination folder (it should be created before running) in which the preprocessed ASCII files are going to be placed. After the end of running the python scripts, the contents of the destination folder should be like that as shown in figure b below. 
In many occasions numerical models use their own coordinate system, for example making the origin of the computation domain at the center of it. This creates discrepancy with the GIS coordinate systems in case the user needs to see the ASCII files in any of the GIS tools. Here, we use the X and Y offset inputs. One can use, zero if he/she needs to check the results in GIS tools.  The number of grids of the ASCII files will be controlled by the resolution input.  We have used X-offset = 612907.50, Y offset =6658839.00 and Resolution 2 for 3D CSV files. Similarly, for the 2D CSV samples, X-offset = 481270, Y offset =6746670 and Resolution 2 can be used. 

![Configure](Media/Python_GUI.png)

 ## 2. Unity project 
This is a sample Unity project based on which the Unity Build is made.  There are four scenes in the project three of which are similar with ones presented in the journal paper. The only difference is that some of the assets are stripped off for copy right reasons.  The fourth one is a sample scene based on which one can build a new one. Four of the scenes are located under Assets->breach->Scenes->DynamicScenes. Below shows the components of the sample scene. 
![Configure](Media/Sample_Scene.png)

 ## 2.1 Pre-processing Module 
Before stating working with the pre-processing module, one needs to organize the data into a format 
that the pre-process module can work with. There are 4 required components that need to be in the same parent folder:

       • Elevation Directory
       • Velocity_X Directory
       • Veloity_Y Directory
       • Terrain.asc File
       • Frames directory

Below is an example of a directory setup for being preprocessed.
![Configure](Media/Input_Folder.png)