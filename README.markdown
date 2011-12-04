ShoeVox
===============

By [Robert Baskin](http://www.rsbaskin.com) and [Max Cutler](http://maxcutler.com)

Announcement: [http://blog.rsbaskin.com/post/3717628154/shoevox-voice-control-for-your-pc](http://blog.rsbaskin.com/post/3717628154/shoevox-voice-control-for-your-pc)

---------------

Do you ever lie on your bed watching a movie and wish you could pause it without moving? Do you ever listen to music as you're
doing things around your room and wish you could go to the next song? Do you ever recline in your desk chair to watch a TV show
and wish you could mute it without reaching for the mouse?

_ShoeVox_ is a Windows application that allows you to control your media software using your voice when you're watching
movies and TV shows or listening to music.

Usage
---------------

When _ShoeVox_ starts, it detects what media programs you have running and starts listening for user commands.

To say a command, you first have to say a prefix. By default, this is "PC". If you'd like to change the prefix, right-click
on the _ShoeVox_ icon in your system tray and click "Set Prefix".

After saying your prefix, you can then say a command. Out of the box, the following commands are available:

 * Play
 * Pause
 * Stop
 * Mute
 * Volume Up
 * Volume Down
 * Next
 * Previous
 
So, to pause your music or video, you would say "PC Pause". If you'd like to add more commands or programs, you can
edit the media.xml file in the _ShoeVox_ directory.

By default, _ShoeVox_ doesn't run automatically when Windows starts. If you'd like it to, right-click on the System Tray
icon and check "Run on Startup".

Program Support
---------------

Out of the box, _ShoeVox_ supports the following media programs:

* Windows Media Player
* Windows Media Center
* iTunes
* Zune
* VLC
* Media Player Classic

Users can add support for additional programs and commands by modifying the included `media.xml` file.

Requirements
---------------

* Windows Vista (or greater)
* .NET Framework 4.0(or greater)


Installation
---------------

1. Download zip file from GitHub project page.
2. Extract zip file to location of your choice.
3. Run "ShoeVox.exe".

Kinect Support
---------------

_ShoeVox_ now supports using a Kinect sensor as its audio input source. 

We are using the [Kinect for Windows SDK Beta 2](http://www.kinectforwindows.org), so developers must install that in order to compile ShoeVox.

For users, in addition to the device itself, the following software must be installed:

* [Microsoft Speech Platform SDK](http://www.microsoft.com/download/en/details.aspx?id=14373)
* [Microsoft Speech Platform Runtime](http://www.microsoft.com/download/en/details.aspx?id=10208)
* [Kinect for Windows Runtime Language Pack](http://go.microsoft.com/fwlink/?LinkId=220942)

If any of the software is missing or a Kinect is not connected to the PC (including the separate power connection), _ShoeVox_ will instead use the PC's default microphone input.

How it works
---------------

_ShoeVox_ leverages [WMI](http://msdn.microsoft.com/en-us/library/aa394582.aspx) to monitor running processes,
and matches them against its list of known media programs. 

If (and only if) _ShoeVox_ detects that one of the known media programs is running, it will start listening
for commands via the computer's default audio input device (i.e., microphone). Because the speech recognition engine
is active only when media programs are running, _ShoeVox_ should use minimal system resources if the user is
not consuming media; users can also explicitly turn off listening using the system tray menu.

When a Kinect sensor is present (and all necessary software requirements are met), the Kinect for Windows SDK's 
[Microsoft.Speech.Recognition.SpeechRecognitionEngine](http://msdn.microsoft.com/en-us/library/microsoft.speech.recognition.speechrecognitionengine.aspx)
is used to perform the actual speech recognition. Otherwise, the PC's default microphone input is processed using
the standard Windows/.NET [System.Speech.Recognition.SpeechRecognitionEngine](http://msdn.microsoft.com/en-us/library/system.speech.recognition.speechrecognitionengine.aspx).

A command prefix, "PC" by default, is used to avoid confusing background noise with intentional user commands.

When a command is recognized, the media program is briefly brought to the foreground (if it isn't already) and sent 
simulated keystrokes corresponding to the desired action.  After the keys have been sent, the previously active window 
(e.g., a web browser) is restored to the foreground.

For example, in Windows Media Player, Control+P is the keyboard shortcut for Play/Pause; when a user speaks "PC play", 
WMP would be brought to the foreground, and we would send fake Control+P keystrokes to the program (using either
[SendKeys](http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx) or 
[Windows Input Simulator](http://inputsimulator.codeplex.com/)).


What's with the name - ShoeVox?
---------------
_ShoeVox_ is named after the IBM [Shoebox](http://www-03.ibm.com/ibm/history/exhibits/specialprod1/specialprod1_7.html), a device
demonstrated by IBM in the early 1960s that was one of the first to do speech recognition.