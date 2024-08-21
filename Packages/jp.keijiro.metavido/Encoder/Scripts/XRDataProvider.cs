using UnityEngine;
using Metavido.Common;

#if METAVIDO_HAS_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace Metavido.Encoder {

[AddComponentMenu("Metavido/Encoding/Metavido XR Data Provider")]
public sealed class XRDataProvider : MonoBehaviour
{
#if METAVIDO_HAS_ARFOUNDATION

    #region Scene object references

    [SerializeField] ARCameraManager _cameraManager = null;
    [SerializeField] AROcclusionManager _occlusionManager = null;

    #endregion

    #region Public methods

    public Matrix4x4 ProjectionMatrix { get; private set; }

    public Transform CameraTransform => _cameraManager.transform;

    public (Texture y, Texture cbcr, Texture depth, Texture stencil)
      TextureSet => _textures;

    #endregion

    #region Private data

    (Texture y, Texture cbcr, Texture depth, Texture stencil) _textures;

    #endregion

    #region AR foundation callbacks

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // No operation for no texture
        if (args.textures.Count == 0) return;

        // Y/CbCr textures
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.TextureY)
                _textures.y = tex;
            else if (id == ShaderID.TextureCbCr)
                _textures.cbcr = tex;
        }

        // Projection matrix
        if (args.projectionMatrix.HasValue)
            ProjectionMatrix = args.projectionMatrix.Value;
    }

    void OnOcclusionFrameReceived(AROcclusionFrameEventArgs args)
    {
        // Stencil/depth textures
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.EnvironmentDepth)
                _textures.depth = tex;
            else if (id == ShaderID.HumanStencil)
                _textures.stencil = tex;
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void OnEnable()
    {
        // Camera callback setup
        _cameraManager.frameReceived += OnCameraFrameReceived;
        _occlusionManager.frameReceived += OnOcclusionFrameReceived;
    }

    void OnDisable()
    {
        // Camera callback termination
        _cameraManager.frameReceived -= OnCameraFrameReceived;
        _occlusionManager.frameReceived -= OnOcclusionFrameReceived;
    }

    #endregion

#else

    public Matrix4x4 ProjectionMatrix { get; }
    public Transform CameraTransform => null;
    public (Texture y, Texture cbcr, Texture depth, Texture stencil)
      TextureSet { get; }

#endif
}

} // namespace Metavido.Encoder
