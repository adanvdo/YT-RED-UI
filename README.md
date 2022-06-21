# YT-RED-UI ALPHA BRANCH

![ytr-banner-1 0 1 1](https://user-images.githubusercontent.com/26498008/171912293-7d925959-aba1-4826-8da8-8fbbf2a35a8f.png)

## Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube, Reddit, Vimeo, Twitter, Instagram, Twitch, and other popular media hosts.

This project is UNLICENSED and uses licensed DevExpress WinForms Controls. 

- [ABOUT](#about)
- [REQUIREMENTS](#requirements)
- [CURRENT FEATURES](#current-features)
- [KNOWN ISSUES](#known-issues)
- [HOW TO USE](#how-to-use)
- [CHANGELOG](#changelog)

### About the Program <a name="about"></a>

YT-RED is a safe alternative to other popular youtube and reddit video downloaders found online.  Other popular reddit downlaod tools are full of trackers and data collection that the end user is not aware of.  YT-RED was personally built with a no-tracking and no-data-collection policy.  By keeping the source code open to the public, it is meant to be published as a safe and versatile media downloader for Windows

![Alpha-1 0 1 1](https://user-images.githubusercontent.com/26498008/171912368-544c8c4a-3389-437a-a758-5fbdf13b7d6f.png)

### Requirements <a name="requirements"></a>
- Windows 7, 8, 10
- .NET Framework 4.8

### Current Features (more info further below) <a name="current-features"></a>
- List Video Format Options
- Download Selected Format
- Download Best Quality Video
- Download Best Quality Audio
- Download History Log / File Browser
- Segment Downloads
- Video Cropping
- Format Conversion
- Minimize to System Tray
- Quick Download while in System Tray
- Quick Download Hotkey

### Known Issues <a name="known-issues"></a>
- GIF downloads are not supported

### How To Use <a name="how-to-use"></a>

YT-RED's main interface is pretty straight forward. Simply enter the URL of any video post from Youtube, Reddit, Vimeo, Twitter, Instagram or Twitch in the address bar

- #### List Formats
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
 
- #### Convert Format
  Specify a Video or Audio format for the resulting download.  YT-RED will convert the original media to the format of your choice.

- #### Quick Downloads
  Quick Download is only available when YT-RED has been minimized to the System Tray. Right-click on the YT-RED icon in the tray, and select "Quick Download" to open the Quick Download form.

- #### Quick Download Hotkey
  When the Quick Download Hotkey is enabled in advanced settings, YT-RED will register a custom Hotkey that initiates a Quick Download.

  To use, Highlight a youtube or reddit media post url in your browser, and press the configured hotkey. This will perform an automatic "Best Download" with the progress displayed above the system tray.

## CHANGELOG <a name="changelog"></a>

### 6/3/2022 v1.0.1.1
- Converted Program to Single-Tab Layout. All downloads are handled from the main tab.
- Added Official Support for Twitter, Vimeo, Instagram, and Twitch.
- Added Unofficial Support for many other Media Hosts
- Added Format Conversion to Post-Processing Options
- Updated yt-dlp and ffmpeg to latest releases
- Fixed Bug where Youtube storyboards were appearing in the format list
- Fixed Bug where Error Message was not displayed for failed downloads

### 5/26/2022 v1.0.0.11
- Added Duration to Youtube Format List
- Enabled Column Customization in Format Grids. Columns can now be hidden/shown by the user
- Replaced standard Message Boxes with Custom Message Box
- Fixed Bug where Segment and Crop panels did not expand/collapse when clicking the header button
- Fixed bug where "Downloads Cleared" message was displayed even though the clear had been cancelled
- Various UI and Behavior Tweaks

### 5/25/2022 v1.0.0.10
- Launching YT-RED VIA exe or shortcut now opens any running instance
- Added Download Cancellation support for Quick Downloads / Hotkey Downloads
- Added options to delete Video, Audio or All downloads to Settings Form
- Added Visual Style to Segment and Crop Toggles when they are enabled
- Clicking the Segment / Crop Headers now Collapse/Expand their panels
- Fixed bug where multiple instances of YT-RED could be run which resulted in HotKey registration errors
- Fixed bug where active downloads continued running if the Quick Download form was closed
- Fixed bug where Youtube Crop panel allowed crop input when it was not toggled on
- Various UI and Behavior Tweaks

### 5/8/2022 v1.0.0.9
- Fixed bug where error log file name contained illegal characters
- Fixed bug where the Download Hotkey was not serialized correctly on some machines

### 5/6/2022 v1.0.0.8
- Added Error Logging
- Added Voluntary Log Reporting
- Added Settings to System Tray Menu
- Fixed bug where hotkey crashed app if clipboard was empty
- Fixed bug where settings failed to load if hotkey was set to (none)

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
