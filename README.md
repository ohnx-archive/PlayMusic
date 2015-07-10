# PlayMusic
Play Music in c#!

## about
The app automatically scans your music directory for music (in .mp3 form only) and starts playing.

The goal for this app is to be a simple music player. You run it, and forget about it (that is, unless you don't like the music being played). Sometimes when I'm coding, I like a bit of music playing, and this is the perfect app for that.

Within the UI, you can see the current song name and artist, see time elapsed/total time, skip the current song, pause/play the current song and enable/disable shuffle mode.

### mini vs large
in the UI, there is an arrow. This allows you to toggle between a "mini" and "larger" mode.
In mini mode, only the controls and song name are shown.
In the larger mode, a song list is shown and you can also select a song that you want to listen to.

### media keys
You can use the media keys to play/pause or skip the current song. (Only Play/pause and skip work, no skipping backwards or stopping or volume).

Please note that this functionality does technically mean that there is a keylogger built into this app to detect when a media key is pressed, but solely for that purpose. Nothing is done in this app if the key pressed isn't a media key. Luckily, it's open source, so if you don't trust me (which I sure hope you do!), you can examine the source code.

### naming convention
When getting the song name and artist name, it is assumed that all files are in a (pretty standard) format `artist name` - `song name`

### shuffle
Shuffle is the difference between going to a random song and going to the next song on the list (alphabetical by filename)

## screenshots
Mini mode:

![Mini mode](http://i.imgur.com/n6tYa0f.png)

Larger mode:

![Larger mode](http://i.imgur.com/QirpXuR.png)

## download
You can compile the source, or you can download the precompiled binary [here](https://github.com/ohnx/PlayMusic/releases/download/v1.0/PlayMusic.exe).
