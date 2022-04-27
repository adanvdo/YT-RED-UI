# YT-RED-UI ALPHA BRANCH

## Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube and Reddit

This project is UNLICENSED and uses licensed DevExpress WinForms Controls. 

### Requirements
- Windows 7, 8, 10
- .NET Framework 4.8

### Current Features
- List Video Format Options
- Download Selected Format
- Download Best Quality Video
- Download Best Quality in Preferred Format
- Download Best Quality Audio
- Download History Log / File Browser
- File Format Preference for Downloads
- Segment Downloads for YT and Reddit
- Video Cropping for YT (Reddit In Progress)


![image](https://user-images.githubusercontent.com/26498008/160806413-9cf735e9-ef8f-4492-af48-92b226cb210b.png)

![image](https://user-images.githubusercontent.com/26498008/160806436-2c31ab5e-4c51-406f-be0e-346dc7218569.png)

![image](https://user-images.githubusercontent.com/26498008/160806455-8a14d646-e87c-4515-9de4-80ecc8baf2d9.png)


## CHANGELOG

### 4/27/2022 v1.0.0.6
- Added Video Cropping for YouTube Downloads
- Added Function in Settings to Delete Downloaded Files
- Fixed bug where expired download history logs were not removed
- Fixed bug where audio segments were saved in the wrong location with the wrong file extension
- Fixed bug where videos processed in ffmpeg were not located by the auto-explorer after downloading
- Behavior and UI Tweaks

### 3/30/2022 v1.0.0.5
- Added Reddit Segment Download Support
- Added Settings Option to automatically open the location of completed downloads in Windows Explorer
- Fixed bug where "Use Preferences" option is not saved
- Fixed bug where double-clicking a download log does not open the correct folder
- Fixed bug where downloads are not saved to the correct folder
- Fixed bug where emoji in YT Video Titles prevent downloads from being located
- Fixed bug where download history is not consistently updated
- Fixed bug where various YT URL formats caused the application to crash

### 3/4/2022 v1.0.0.4
- Added Download History File Status Indicator
- Added Partial Segment Download support for YouTube
- Added Program Icon
- UI and Behavior Tweaks

## This is an experimental build.  Bugs and/or crashes are possible.  
Report new issues [here](https://github.com/adanvdo/YT-RED-UI/issues/new)
