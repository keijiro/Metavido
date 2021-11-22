using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Encoder {

public sealed class BibcamEncoder : MonoBehaviour
{
    #region Public accessors

    public float minDepth { get => _minDepth; set => _minDepth = value; }
    public float maxDepth { get => _maxDepth; set => _maxDepth = value; }
    public Texture EncodedTexture => _encoded;

    #endregion

    #region Editable attributes

    [SerializeField] BibcamXRDataProvider _xrSource = null;
    [SerializeField] float _minDepth = 0.025f;
    [SerializeField] float _maxDepth = 5;

    #endregion

    #region Hidden asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private objects

    Material _material;
    RenderTexture _encoded;
    GraphicsBuffer _metadata;
    Metadata[] _tempArray = new Metadata[1];

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _material = new Material(_shader);
        _encoded = GfxUtil.RGBARenderTexture(1920, 1080);
        _metadata = GfxUtil.StructuredBuffer(12, sizeof(float));
    }

    void OnDestroy()
    {
        Destroy(_material);
        Destroy(_encoded);
        _metadata.Dispose();
    }

    void LateUpdate()
    {
        var tex = _xrSource.TextureSet;
        if (tex.y == null) return;

        // Texture planes
        _material.SetTexture(ShaderID.TextureY, tex.y);
        _material.SetTexture(ShaderID.TextureCbCr, tex.cbcr);
        _material.SetTexture(ShaderID.EnvironmentDepth, tex.depth);
        _material.SetTexture(ShaderID.HumanStencil, tex.stencil);

        // Aspect ratio compensation (camera vs. 16:9)
        var aspectFix = 9.0f / 16 * tex.y.width / tex.y.height;
        _material.SetFloat(ShaderID.AspectFix, aspectFix);

        // Projection matrix
        var proj = _xrSource.ProjectionMatrix;
        proj[1, 1] = proj[0, 0] * 16 / 9; // Y-factor overriding (16:9)

        // Depth range
        var range = new Vector2(_minDepth, _maxDepth);
        _material.SetVector(ShaderID.DepthRange, range);

        // Metadata
        _tempArray[0] = new Metadata(_xrSource.CameraTransform, proj, range);
        _metadata.SetData(_tempArray);
        _material.SetBuffer(ShaderID.Metadata, _metadata);

        // Encoding and multiplexing
        Graphics.Blit(null, _encoded, _material);
    }

    #endregion
}

} // namespace Bibcam.Encoder
