# FastRender

Not usable yet still work in progress.

What needs to be done:
- [ ] Get video details properly(getting duration doesnt work on other formats other than mp4?)
- [ ] Fix the weird gap there is between the media element and the right border
- [ ] Properly scale video editor when increasing/decreasing window size
- [ ] Implement timeline
- [ ] Implement the editor
- [ ] Add hotkeys for the editor
- [ ] More robust process for ffmpeg/ffmprobe binaries. Maybe even integrate libraries instead?
- [ ] Rewrite the image extractor part. Currently it gets the first frame of the video and stores it in a folder.

This is how it currently looks like:
![grafik](https://github.com/Ati1707/FastRender/assets/152104750/473e9162-3398-480c-a64d-f3175bbdb300)


My idea was to have a container type of thing at the top left where you can drag and drop the videos.
Top right will have the video player itself and the bottom part will be timeline and the editor.
