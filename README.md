# FastRender

Not usable yet still work in progress.

What needs to be done:
- [x] Get video details properly(getting duration doesnt work on other formats other than mp4?)
-  MP4 and MKV works I didnt test other formats.
- [x] Fix the weird gap there is between the media element and the right border
- [x] Properly scale video editor when increasing/decreasing window size
- [ ] Implement timeline
- [ ] Implement the editor
- [ ] Implement usable video manager component.
- Right now you can only drag and drop your videos. Clicking or dragging loaded videos doesn't do anything yet.
- [ ] Add hotkeys for the editor
- [ ] More robust process for ffmpeg/ffmprobe binaries. Maybe even integrate libraries instead?
- [ ] Rewrite the image extractor part. Currently it gets the first frame of the video and stores it in a folder.

This is how it currently looks like:
![grafik](https://github.com/Ati1707/FastRender/assets/152104750/7b4417c1-5c4d-4d5c-a10d-d13a6e91b2cb)



My idea was to have a container type of thing at the top left where you can drag and drop the videos.
Top right will have the video player itself and the bottom part will be timeline and the editor.
