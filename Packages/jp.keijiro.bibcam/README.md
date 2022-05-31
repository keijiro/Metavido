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

- Unity 2021 LTS
- LiDAR-enabled iOS device for recording

How to Try
----------

Build and play the `Encoder` scene on a LiDAR-enabled iOS device. You can
record Bibcam video clips by pressing the "Record" button. It saves recorded
clips into the camera roll.

To try playback with the recorded clips, copy them into the project directory
(the directory that contains `Assets`, `Packages`, etc.) and rename it to
`Test.mp4`. Then you can play it back with the `Decoder` scene.

Tips
----

- The encoder application caps the frame rate at 30fps to reduce excessive
  energy consumption and thermal throttling. You can switch this behavior in
  the application settings (Settings -> Bibcam -> "Cap Frame Rate").

Related Repositories
--------------------

- [BibcamVfx] -- Advanced Bibcam VFX with HDRP
- [BibcamUrp] -- Bibcam renderer on URP
- [BibcamTimeline] -- Bibcam frame-accurate playback with Timeline

[BibcamVfx]: https://github.com/keijiro/BibcamVfx
[BibcamUrp]: https://github.com/keijiro/BibcamUrp
[BibcamTimeline]: https://github.com/keijiro/BibcamTimeline
