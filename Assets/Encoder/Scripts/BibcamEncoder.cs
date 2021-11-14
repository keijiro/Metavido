using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Bibcam {

sealed class BibcamEncoder : MonoBehaviour
{
    #region Scene object references

    [SerializeField] ARCameraManager _cameraManager = null;
    [SerializeField] AROcclusionManager _occlusionManager = null;

    #endregion

    #region Hidden asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private objects

    Matrix4x4 _projection;
    Material _material;
    RenderTexture _encoded;

    #endregion

    #region Public members

    public Texture EncodedTexture => _encoded;

    public void Encode(Camera camera, float minDepth, float maxDepth)
    {
        // Aspect ratio compensation (camera vs. 16:9)
        var proj = _projection;
        proj[1, 1] *= (16.0f / 9) / camera.aspect;

        // Blit with the encoding/muxing shader
        const float margin = 0.05f;
        var range = new Vector3(minDepth, maxDepth / (1 - margin), margin);
        var meta = new Metadata(camera.transform, proj, range);
        _material.SetVector(ShaderID.DepthRange, range);
        _material.SetMatrix(ShaderID.Metadata, meta.AsMatrix);
        Graphics.Blit(null, _encoded, _material);
    }

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
                _material.SetTexture(ShaderID.TextureY, tex);
            else if (id == ShaderID.TextureCbCr)
                _material.SetTexture(ShaderID.TextureCbCr, tex);
        }

        // Projection matrix
        if (args.projectionMatrix.HasValue)
            _projection = args.projectionMatrix.Value;

        // Source texture aspect ratio
        var tex1 = args.textures[0];
        var texAspect = (float)tex1.width / tex1.height;

        // Aspect ratio compensation factor for the multiplexer
        var aspectFix = texAspect / (16.0f / 9);
        _material.SetFloat(ShaderID.AspectFix, aspectFix);
    }

    void OnOcclusionFrameReceived(AROcclusionFrameEventArgs args)
    {
        // Stencil/depth textures.
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.HumanStencil)
                _material.SetTexture(ShaderID.HumanStencil, tex);
            else if (id == ShaderID.EnvironmentDepth)
                _material.SetTexture(ShaderID.EnvironmentDepth, tex);
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _material = new Material(_shader);
        _encoded = new RenderTexture(Screen.width, Screen.height, 0);
    }

    void OnDestroy()
    {
        Destroy(_material);
        Destroy(_encoded);
    }

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
}

} // namespace Bibcam
