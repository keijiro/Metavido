Bibcam - Burnt-in barcode metadata camera
=========================================

![gif](https://user-images.githubusercontent.com/343936/142789278-4ede7318-e789-4d32-ad99-06ff90e91b20.gif)
![gif](https://user-images.githubusercontent.com/343936/142789286-f7ba3b96-c176-4687-aa58-170f2e166855.gif)

**Bibcam** is a proof-of-concept project where I tried the idea of "burnt-in
barcode metadata" carrying camera tracking data within a single video stream.

![Planes](https://user-images.githubusercontent.com/343936/142789292-9bba9330-0fa0-49f8-b270-9bcefe326278.png)

By using this format, you can record/edit/playback AR-ready video clips without
worrying about desynchronization with external tracking data.

System Requirements
-------------------

This project uses a LiDAR enabled iOS device as a camera. I'd recommend iPhone
13 Pro/Pro Max for better quality.

You can playback recorded video clips on any Unity-supported platform.

I created this project on Unity 2021.2.

How To Try
----------

Build and play the `Encoder` scene on a LiDAR enabled iOS device. You can
record Bibcam video clips by just pressing the "Record" button. It stores
recorded clips in the camera roll.

Transfer a video clip to your computer. Copy it into the `StreamingAssets`
directory, and rename it to `Test.mp4`. Then you can play it back in the
`Decoder` scene.

For more advanced usage, see the [BibcamVfx] repository.

[BibcamVfx]: https://github.com/keijiro/BibcamVfx
