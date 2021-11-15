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
        proj[1, 1] /= aspectFix;

        // Depth range
        var range = new Vector2(_minDepth, _maxDepth);
        _material.SetVector(ShaderID.DepthRange, range);

        // Metadata
        var meta = new Metadata(_xrSource.CameraTransform, proj, range);
        _material.SetMatrix(ShaderID.Metadata, meta.AsMatrix);

        // Encoding and multiplexing
        Graphics.Blit(null, _encoded, _material);
    }

    #endregion
}

} // namespace Bibcam.Encoder
