# YT-RED-UI

## Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube, Reddit, Vimeo, Twitter, Instagram, Twitch, and other popular media hosts. 

![ytr-banner](https://user-images.githubusercontent.com/26498008/170678451-c993fe6e-3399-4949-969e-d6d18ab2aa56.png)

# **News**
## **10/19/2022 : Beta 1.0.2.0 Official Release Now Available**
## **10/17/2022 : Beta 1.0.2.0 Release Coming 10/19**
## **5/26/2022 : YT-RED IS NOW IN BETA STAGE!**
We are out of alpha testing and moving to Beta.  Please send any issues or enhancement requests by adding a new issue on GitHub.

[View All Releases on GitHub](https://github.com/adanvdo/YT-RED-UI/releases)

# Table of Contents
- [PROJECT STATUS](#status)
- [ROADMAP](#future)
- [ABOUT](#about)
- [ACKNOWLEDGEMENTS](#acknowledgements)
- [REQUIREMENTS](#requirements)
- [CURRENT FEATURES](#current-features)
- [CONTACT](#contact)

# **Overview**

## Project Status: **In Beta** <a name="status"></a>

### **Latest Stable**: *N/A*

### **Latest Beta: [v1.0.2.0](https://github.com/adanvdo/YT-RED-UI/releases/tag/v1.0.2.0-beta)**

### **Latest Alpha: [v1.0.1.20](https://github.com/adanvdo/YT-RED-UI/releases/tag/v1.0.1.20-alpha)**

*Updated 10/19/2022*

# **Roadmap: The Future of YT-RED** <a name="future"></a>

## Windows
YT-RED was built for Windows, targeting .NET Framework 4.8. 
Development of the original .NET Framework version will continue for the foreseeable future.

While YT-RED can only run on Windows at this time, a multi-platform version will be available in the future.

While development continues on the current Windows version, I will be collecting feedback from those who are using the Beta releases. All features and enhancements during this time will be used as the foundation of a new multi-platform version of the program.

## Multi-Platform
Initial development has started on the multi-platform project **YT-RED-MAUI** 
However, further development will depend on the status of Linux support. If complications prevent development of a Linux compatible program, development will be restarted on a different App UI framework.

YT-RED Multi-Platform intended support:
- Windows
- Linux
- Android
- Chrome OS (Android)

*I hope to create iOS and MacOS compatible versions at a later date. Due to Apple policies and requirements, development for MacOS and iOS is not feasible at this time*
  

# **About YT-RED** <a name="about"></a>

### YT-RED is a Windows .NET Framework GUI Application for Downloading Video and Audio Files from YouTube and Reddit
  \
*Screenshots from v1.0.1.20-alpha*

![YTRMain](https://user-images.githubusercontent.com/26498008/196805373-45dd6736-c6bf-41d1-8a27-507437300b00.png)

![2022-08-21_0-03-18](https://user-images.githubusercontent.com/26498008/185775468-58de4d42-dfba-425b-b370-6f59f7091bf7.png)

## **Requirements** <a name="requirements"></a>
- Windows 7, 8, 10
- .NET Framework 4.8

## **Acknowledgements** <a name="acknowledgements"></a>
**Most of the the hard work that makes this possible was done thanks to many other awesome developers**

### **YT-RED is built using the following open-source libraries**
- [**YoutubeDLSharp**](https://github.com/Bluegrams/YoutubeDLSharp)
- [**Xabe.FFmpeg**](https://github.com/tomaszzmuda/Xabe.FFmpeg)
- [**Newtonsoft.Json**](https://github.com/JamesNK/Newtonsoft.Json)
- [**HtmlAgilityPack**](https://github.com/zzzprojects/html-agility-pack/)
- [**URIScheme**](https://github.com/HMBSbige/URIScheme)

A lot of magic happens behind the scenes. 
### **YT-RED uses the following open-source applications for downloads and post-processing**
- [**yt-dlp**](https://github.com/yt-dlp/yt-dlp)
- [**FFmpeg**](https://github.com/FFmpeg/FFmpeg)

## **Current Features** <a name="current-features"></a>
- ### List Formats
  This will retrieve all video formats available to download.
  An individual format can be selected and downloaded.

- ### Format Selection Mode - Choose from one of two available selection modes
  1. Preset - Select a single format from one of the automatically generated format options  
  2. Custom - Combine the audio and video format of your choice for a single merged file

- ### Download Best
  The "Download Best" option evaluates all available video and audio formats before downloading the best available.

  *Note* Download Best often requires downloading separate video and audio files, which are then merged after downloading. This can take a little longer than downloading a specific format.

- ### Download Segment
  The "Download Segment" option can be toggled on and off. This feature is only available when downloading a specific format in order to improve performance and reduce resource usage.

  Specify the start time of the segment, and the duration.

- ### Automatic ID3 Tagging
  When downloading audio, video thumbnails are automatically embedded as cover art, and titles are set to the video title.

- ### Crop Video
  The "Crop Video" option is available for all video downloads. The feature only accepts crop sizes in pixels at this time. 

  Enter the number of pixels to crop on each desired side, and then start the download.
 
- ### Convert Format
  Specify a Video or Audio format for the resulting download.  YT-RED will convert the original media to the format of your choice.

- ### Download History (Optional)
  When enabled, YT-RED will locally record each download, including settings. Download History can be configured to store download logs for the number of days specified.
  Recorded downloads can be redownloaded by right-clicking on the associated log and choosing the "re-download" option, or you can run a new download by choosing the "Use URL for New Download" option.
  Download History can be manually cleared in Settings, with the option to delete all associated download files.

- ### Quick Downloads
  Quick Download is only available when YT-RED has been minimized to the System Tray. Right-click on the YT-RED icon in the tray, and select "Quick Download" to open the Quick Download form.

- ### Quick Download Hotkey
  When the Quick Download Hotkey is enabled in advanced settings, YT-RED will register a custom Hotkey that initiates a Quick Download.

  To use, Highlight a youtube or reddit media post url in your browser, and press the configured hotkey. This will perform an automatic "Best Download" with the progress displayed above the system tray.

- ### Layout Options
  Change the position of certain panels for the layout you like.

- ### Update Assistant
  Check for updates and download release packages directly in the app. 
  Optional Updater for one-click update installs
  Open Settings -> About and click the "Check for Update" button.
  
  ![2022-08-21_0-03-35](https://user-images.githubusercontent.com/26498008/185775490-4ab0e3c0-b980-4f4b-8b36-b11f08dad838.png)

## **Contact**
### **To report bugs or request enhancements, please  [submit a new Issue on GitHub](https://github.com/adanvdo/YT-RED-UI/issues/new)**
### **For all other inquiries, email [jesse@jmconcepts.net](mailto:jesse@jmconcepts.net)**

Thank you for your support!
  
### YT-RED -  Copyright © 2022 JAMGALACTIC - [WEAKNPC.COM](https://www.weaknpc.com)