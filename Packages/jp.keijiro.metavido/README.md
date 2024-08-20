Metavido - Metadata Embedding Video Subformat
==============================================

![gif](https://user-images.githubusercontent.com/343936/142789278-4ede7318-e789-4d32-ad99-06ff90e91b20.gif)
![gif](https://user-images.githubusercontent.com/343936/142789286-f7ba3b96-c176-4687-aa58-170f2e166855.gif)

**Metavido** is a video subformat that embeds camera metadata directly into
video frames using a burnt-in-barcode technique. It also integrates non-color
planes, such as depth information and human stencil, into the frame through a
squeezing method.

![Planes](https://user-images.githubusercontent.com/343936/142789292-9bba9330-0fa0-49f8-b270-9bcefe326278.png)

Metavido enables recording, editing, and playback of AR-ready video clips
without the need to worry about desynchronization with external tracking data.

System Requirements
-------------------

- Unity 6
- A LiDAR-enabled iOS device for recording

How to Try
----------

To try Metavido, start by building and running the `Encoder` scene on a
LiDAR-enabled iOS device. Once the application is running, press the “Record”
button to capture Metavido clips, which will be automatically saved to your
camera roll.

To play back the recorded clips, copy the clips into your project directory
(the directory that contains `Assets`, `Packages`, etc.), and rename the file
to `Test.mp4`. You can then use the `Decoder` scene to play back the clip.

Tips
----

- The encoder application caps the frame rate at 30fps to minimize energy
  consumption and prevent thermal throttling. You can adjust this setting in
  the application settings (Settings -> Metavido -> “Cap Frame Rate”).
