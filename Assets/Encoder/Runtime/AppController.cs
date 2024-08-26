using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using Unity.Properties;
using Metavido.Decoder;
using Metavido.Encoder;
using Avfi;

namespace Metavido.Encoder {

sealed class AppController : MonoBehaviour
{
    #region Scene object references

    [Space]
    [SerializeField] FrameEncoder _encoder = null;
    [SerializeField] Camera _camera = null;
    [Space]
    [SerializeField] MetadataDecoder _decoder = null;
    [SerializeField] TextureDemuxer _demuxer = null;

    #endregion

    #region Private members

    VideoRecorder Recorder => GetComponent<VideoRecorder>();
    VisualElement UIRoot => GetComponent<UIDocument>().rootVisualElement;
    VisualElement UITally => UIRoot.Q("tally");
    Button UIRecordButton => UIRoot.Q<Button>("record-button");
    Button UIStopButton => UIRoot.Q<Button>("stop-button");

    FrameFeeder _feeder;

    #endregion

    #region UI properties and methods

    [CreateProperty] public float MinDepth => _encoder.minDepth;

    [CreateProperty] public float MaxDepth
      { get => _encoder.maxDepth; set => SetMaxDepth(value); }

    void SetMaxDepth(float z)
    {
        _encoder.maxDepth = z;
        _encoder.minDepth = z / 50;
        PlayerPrefs.SetFloat("max_depth", z);
    }

    void OnRecordButton()
    {
        // Camera position reset
        _camera.transform.parent.position = -_camera.transform.localPosition;

        Recorder.StartRecording();
        UIRecordButton.visible = false;
        UIStopButton.visible = true;
        UITally.visible = true;
    }

    void OnStopButton()
    {
        Recorder.EndRecording();
        UIRecordButton.visible = true;
        UIStopButton.visible = false;
        UITally.visible = false;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Recorder setup
        Recorder.source = (RenderTexture)_encoder.EncodedTexture;

        // Instant decoder setup
        _feeder = new FrameFeeder(_decoder, _demuxer);

        // FPS cap preference
        var fpsCap = PlayerPrefs.GetInt("fps_cap_preference") != 0;
        FindFirstObjectByType<ARSession>().matchFrameRateRequested = !fpsCap;

        // Max depth value
        MaxDepth = PlayerPrefs.GetFloat("max_depth", 5);

        // UI setup
        UIRoot.dataSource = this;
        UIRecordButton.clicked += OnRecordButton;
        UIRecordButton.visible = true;
        UIStopButton.clicked += OnStopButton;
        UIStopButton.visible = false;
        UITally.visible = false;
    }

    void OnDestroy()
    {
        _feeder?.Dispose();
        _feeder = null;
    }

    void OnApplicationPause(bool paused)
    {
        if (paused && Recorder.IsRecording) Recorder.EndRecording();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) SceneManager.LoadScene(0);
    }

    void Update()
    {
        // Monitor update
        _feeder.AddFrame(_encoder.EncodedTexture);
        _feeder.Update();
    }

    #endregion
}

} // namespace Metavido.Encoder
