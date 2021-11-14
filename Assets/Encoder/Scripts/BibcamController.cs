using UnityEngine;
using UnityEngine.UI;

namespace Bibcam {

sealed class BibcamController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] Camera _camera = null;
    [SerializeField] RawImage _mainView = null;
    [SerializeField] GameObject _uiRoot = null;
    [SerializeField] Slider _depthSlider = null;
    [SerializeField] Text _depthLabel = null;

    #endregion

    #region Public members (exposed for UI)

    public void ToggleUI()
      => _uiRoot.SetActive(!_uiRoot.activeSelf);

    public void ResetOrigin()
      => _camera.transform.parent.position = -_camera.transform.localPosition;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        Application.targetFrameRate = 60;
        _mainView.texture = GetComponent<BibcamEncoder>().EncodedTexture;
        _depthSlider.value = PlayerPrefs.GetFloat("DepthSlider", 5);
        _uiRoot.SetActive(false);
    }

    void LateUpdate()
    {
        var maxDepth = _depthSlider.value;
        var minDepth = maxDepth / 20;
        GetComponent<BibcamEncoder>().Encode(_camera, minDepth, maxDepth);
        _depthLabel.text = $"Depth Range: {minDepth:0.00} - {maxDepth:0.00}";
        PlayerPrefs.SetFloat("DepthSlider", maxDepth);
    }

    #endregion
}

} // namespace Bibcam
