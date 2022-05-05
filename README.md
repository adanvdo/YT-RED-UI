# YT-RED-UI ALPHA BRANCH

## Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube and Reddit

This project is UNLICENSED and uses licensed DevExpress WinForms Controls. 

### Requirements
- Windows 7, 8, 10
- .NET Framework 4.8

### Current Features (Scroll Down for More Info)
- List Video Format Options
- Download Selected Format
- Download Best Quality Video
- Download Best Quality in Preferred Format
- Download Best Quality Audio
- Download History Log / File Browser
- File Format Preference for Downloads
- Segment Downloads for YT and Reddit
- Video Cropping
- Minimize to System Tray
- Quick Download while in System Tray
- Quick Download Hotkey

### Known Issues
- Reddit GIF downloads are not supported





## YT-RED Youtube & Reddit Video Downloader
### How To Use

YT-RED's main interface is pretty straight forward. Simply enter the URL of any Youtube video or reddit video post in address bar of the corresponding tab.

- #### List Formats
  Both Youtube and Reddit tabs have a "List Formats" button.
  This will retrieve all video formats available to download.
  An individual format can be selected and downloaded.

- #### Download Best
  The "Download Best" option evaluates all available video and audio formats before downloading the best available.

  *Note* Download Best often requires downloading separate video and audio files, which are then merged after downloading. This can take a little longer than downloading a specific format.

- #### Download Segment
  The "Download Segment" option can be toggled on and off. This feature is only available when downloading a specific format in order to improve performance and reduce resource usage.

  Specify the start time of the segment, and the duration.

- #### Crop Video
  The "Crop Video" option is available for all video downloads. The feature only accepts crop sizes in pixels at this time. 

  Enter the number of pixels to crop on each desired side, and then start the download.

- #### Quick Downloads
  Quick Download is only available when YT-RED has been minimized to the System Tray. Right-click on the YT-RED icon in the tray, and select "Quick Download" to open the Quick Download form.

- #### Quick Download Hotkey
  When the Quick Download Hotkey is enabled in advanced settings, YT-RED will register a custom Hotkey that initiates a Quick Download.

  To use, Highlight a youtube or reddit media post url in your browser, and press the configured hotkey. This will perform an automatic "Best Download" with the progress displayed above the system tray.

## CHANGELOG

### 5/5/2022 v1.0.0.7
- Added System Tray Support - Can now be minimized to System Tray
- Added Quick Download Hotkey Feature
- Added "About" Section in Settings
- Fully Implemented Crop Feature for Youtube and Reddit downloads
- Fixed bug where youtube "Best" downloads were not converted to preferred format

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
