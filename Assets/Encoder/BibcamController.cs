using UnityEngine;
using UnityEngine.UI;

namespace Bibcam {

sealed class BibcamController : MonoBehaviour
{
    #region Scene object references

    [Space]
    [SerializeField] Camera _camera = null;
    [SerializeField] RawImage _mainView = null;

    #endregion

    #region Editable parameters

    [Space]
    [SerializeField] float _minDepth = 0.2f;
    [SerializeField] float _maxDepth = 5.0f;

    #endregion

    #region Public methods (UI callback)

    public void ResetOrigin()
      => _camera.transform.parent.position = -_camera.transform.localPosition;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // System settings
        Application.targetFrameRate = 60;

        // UI initialization
        _mainView.texture = GetComponent<BibcamEncoder>().EncodedTexture;
    }

    void LateUpdate()
      => GetComponent<BibcamEncoder>().Encode(_camera, _minDepth, _maxDepth);

    #endregion
}

} // namespace Bibcam
