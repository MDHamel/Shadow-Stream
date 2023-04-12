# Shadow Stream

## Audio Video Streaming Application

This is an audio video streaming application that reads from a video and audio source, specifically designed for use with the "ShadowCast" capture card by Genki. The application allows for streaming at 1080p and 720p resolutions, with options for 30 or 60 frames per second (fps) when streaming at 720p, and 30fps when streaming at 1080p. The application is designed to provide a minimal video lag and slight audio lag to enhance the game viewing experience.

## Features

- Stream your capture card experience to your computer with minimal processing and ram consumption. Powerful and lightweight. 
- Compatible with the "ShadowCast" capture card by Genki. The ShadowSense feature automatically detects your ShadowCast device.
- Support for streaming at 1080p and 720p resolutions
- Option to choose between 30fps or 60fps when streaming at 720p (Fullscreen not recommended at 60 Fps)
- Minimal video lag and audio lag for improved game viewing experience

## Keyboard Controls

- Double click video to go fullscreen
- Use the mouse wheel to adjust volume
- F1 - Opens the settings window
- F5 or R - Refreshes the audio and video streams
- Esc - Closes fullscreen
- M - Toggles muting the audio
- S - Takes a screenshot of your stream and saves it to the clipboard
- Ctrl + S - Saves the screenshot to your computer

## System Requirements

- "ShadowCast" capture card by Genki, or other capture card
- A video and audio source (e.g., gaming console, PC, etc.)
- .NET 6.0 or higher
- Windows 7 or higher

## Install

Go to the latest release and extract the .zip onto your computer. Run the ShadowStream.exe to launch application.

## Known Issues

- Possible audio or video syncing issues under certain conditions
- Application may crash at fullscreen, ***NOT RECOMMENDED TO DO 60 FPS AT FULLSCREEN***
- Performance may vary depending on the hardware specifications of your system
- Video or audio might disconnect or become choppy, try refreshing the stream with 'F5' or 'R'
- A memory leak may occur at fullscreen, if this keeps happening try changing to 1080p resolution.
