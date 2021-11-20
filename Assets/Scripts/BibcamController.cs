using UnityEngine;
using UnityEngine.UI;
using Bibcam.Encoder;
using Avfi;

sealed class BibcamController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamEncoder _encoder = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] RawImage _mainView = null;
    [SerializeField] Slider _depthSlider = null;
    [SerializeField] Text _depthLabel = null;
    [SerializeField] Text _recordLabel = null;

    #endregion

    #region Hidden asset reference

    [SerializeField, HideInInspector] Shader _monitorShader = null;

    #endregion

    #region Private members

    VideoRecorder Recorder => GetComponent<VideoRecorder>();

    RenderTexture _monitorTexture;
    Material _monitorMaterial;

    #endregion

    #region Public members (exposed for UI)

    public void OnRecordButton()
    {
        if (Recorder.IsRecording)
        {
            // Stop recording
            Recorder.EndRecording();
            _recordLabel.text = "Record";
            _recordLabel.color = Color.black;
        }
        else
        {
            // Reset the camera position.
            _camera.transform.parent.position
              = -_camera.transform.localPosition;

            // Start recording
            Recorder.StartRecording();
            _recordLabel.text = "Stop";
            _recordLabel.color = Color.red;
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // We have a good phone. Crank it up to 60 fps.
        Application.targetFrameRate = 60;

        // Recorder setup
        Recorder.source = (RenderTexture)_encoder.EncodedTexture;

        // Monitor setup
        _monitorTexture = new RenderTexture(1920, 1080, 0);
        _monitorMaterial = new Material(_monitorShader);

        // UI setup
        _mainView.texture = _monitorTexture;
        _depthSlider.value = PlayerPrefs.GetFloat("DepthSlider", 5);
    }

    void OnDestroy()
    {
        Destroy(_monitorTexture);
        Destroy(_monitorMaterial);
    }

    void Update()
    {
        // Depth range settings update
        var maxDepth = _depthSlider.value;
        var minDepth = maxDepth / 20;
        (_encoder.minDepth, _encoder.maxDepth) = (minDepth, maxDepth);

        // Monitor update
        Graphics.Blit
          (_encoder.EncodedTexture, _monitorTexture, _monitorMaterial);

        // UI update
        _depthLabel.text = $"Depth Range: {minDepth:0.00} - {maxDepth:0.00}";
        PlayerPrefs.SetFloat("DepthSlider", maxDepth);
    }

    #endregion
}
