# FastRender

Not usable yet still work in progress.

Preview screenshot 04.06.2024:
![grafik](https://github.com/Ati1707/FastRender/assets/152104750/febcf44f-3278-4780-9e34-d2fa2a54616a)

What is the purpose of this tool:  
The main focus will be on rendering performance.  
The tool will be easy to use. The only functionality the editor should have is to trim videos and render whatever is in the editor.  
I might add an option to mute certain parts of the audio, but other than that the tool won't have any fancy effects.  
Videos will be only loaded on demand. That means only when using the player or starting the rendering process. This idea of mine should make it faster to work with the editor and keep the RAM usage low.
If my implementation of my thought process causes more problems than benefits I will change my decision

What needs to be done:
- [x] Get video details properly(getting duration doesnt work on other formats other than mp4?)
-  MP4 and MKV works I didnt test other formats.
- [x] Fix the weird gap there is between the media element and the right border
- [x] Properly scale video editor when increasing/decreasing window size
- [ ] Implement timeline
- Should automatically expand to the right if there are more videos than space and allow the user to change the zoom should be able to edit videos frame by frame.
- [ ] Implement the editor
- For the editor the only things that are left is adding hotkey to cut the videos and to stick with other videos if they are close to each other and
  the editor sometimes gets cut out when the player takes too much space. Currently the user needs to resize it manually to show the AUDIO editor.
- [x] Implement usable video manager component.
- I consider this as almost done. Only thing that I need to fix is when you pause the video the seek to slider doesn't work properly.
- [ ] Add hotkeys for the editor
- Hotkey `S` for enabling/disabling the snapping functionality.
- [ ] More robust process for ffmpeg/ffmprobe binaries. Maybe even integrate libraries instead?
- While working on the project I noticed that sometimes when I dragndrop a video it freezes the program.
      Maybe ffmpeg or ffmprobe errors out need to investigate this.
- [ ] Rewrite the image extractor part. Currently it gets the first frame of the video and stores it in a folder.
- [ ] Restyle a lot of stuff.
- In early developments I will keep it simple because I rather have functional code instead of pretty controls/UI.
- [ ] Maybe rewrite to Avalonia?
- I want to keep the editor windows only. I don't wanna switch to avalonia because of cross-platform support but rather because I like to use "cutting edge" frameworks.
