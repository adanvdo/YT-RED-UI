# YT-RED-UI BETA BRANCH

![ytr-promo1](https://user-images.githubusercontent.com/26498008/167324218-a222b6b3-1a32-4aa5-966a-13357cec72e0.jpg)

## Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube and Reddit

This project is UNLICENSED and uses licensed DevExpress WinForms Controls. 

- [ABOUT](#about)
- [REQUIREMENTS](#requirements)
- [CURRENT FEATURES](#current-features)
- [KNOWN ISSUES](#known-issues)
- [HOW TO USE](#how-to-use)
- [CHANGELOG](#changelog)

### About the Program <a name="about"></a>

YT-RED is a safe alternative to other popular youtube and reddit video downloaders found online.  Other popular reddit downlaod tools are full of trackers and data collection that the end user is not aware of.  YT-RED was personally built with a no-tracking and no-data-collection policy.  By keeping the source code open to the public, it is meant to be published as a safe and versatile media downloader for Windows

![ytr-promo2](https://user-images.githubusercontent.com/26498008/167324533-f962636c-be97-4210-ac38-cbad8cf2195a.jpg)

### Requirements <a name="requirements"></a>
- Windows 7, 8, 10
- .NET Framework 4.8

### Current Features (more info further below) <a name="current-features"></a>
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

### Known Issues <a name="known-issues"></a>
- Reddit GIF downloads are not supported

### How To Use <a name="how-to-use"></a>

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

## CHANGELOG <a name="changelog"></a>

### 5/26/2022
- Beta Branch Created from v1.0.0.11-alpha

## This is a beta build.  Bugs and/or crashes are possible.  
Report new issues [here](https://github.com/adanvdo/YT-RED-UI/issues/new)
