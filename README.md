# FastRender

Not usable yet still work in progress.
This is how it currently looks like 03.12.2024:
![grafik](https://github.com/Ati1707/FastRender/assets/152104750/5a5e3b82-b31b-43d7-b466-c828383e0d1b)
What needs to be done:
- [x] Get video details properly(getting duration doesnt work on other formats other than mp4?)
-  MP4 and MKV works I didnt test other formats.
- [x] Fix the weird gap there is between the media element and the right border
- [x] Properly scale video editor when increasing/decreasing window size
- [ ] Implement timeline
- [ ] Implement the editor
- [x] Implement usable video manager component.
- I consider this as almost done. Only thing that I need to fix is when you pause the video the seek to slider doesn't work properly.
- [ ] Add hotkeys for the editor
- [ ] More robust process for ffmpeg/ffmprobe binaries. Maybe even integrate libraries instead?
- While working on the project I noticed that sometimes when I dragndrop a video it freezes the program.
      Maybe ffmpeg or ffmprobe errors out need to investigate this.
- [ ] Rewrite the image extractor part. Currently it gets the first frame of the video and stores it in a folder.
- [ ] Restyle a lot of stuff.
- In early developments I will keep it simple because I rather have functional code instead of pretty controls.
- [ ] Maybe rewrite to Avalonia?
- I want to keep the editor windows only. I don't wanna switch to avalonia because of cross-platform support but rather because I like to use "cutting edge" frameworks.
