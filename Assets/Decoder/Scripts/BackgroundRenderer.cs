using UnityEngine;

namespace Bibcam.Decoder {

sealed class BackgroundRenderer : MonoBehaviour
{
    #region External scene object references

    [SerializeField] MetadataDecoder _decoder = null;

    #endregion

    #region Hidden external asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private objects

    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _material = new Material(_shader);

    void OnDestroy()
      => Destroy(_material);

    void OnRenderObject()
    {
        // Run it only on the target camera.
        var camera = GetComponent<Camera>();
        if (camera != Camera.current) return;

        // Projection parameters
        var pm = camera.projectionMatrix;
        var pv = new Vector4(pm[0, 2], pm[1, 2], pm[0, 0], pm[1, 1]);

        // Inverse view matrix
        var v2w = Matrix4x4.TRS(camera.transform.position,
                                camera.transform.rotation,
                                new Vector3(1, 1, -1));

        // Material property update
        _material.SetVector(ShaderID.ProjectionVector, pv);
        _material.SetMatrix(ShaderID.InverseViewMatrix, v2w);
        _material.SetTexture(ShaderID.ColorTexture, _decoder.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _decoder.DepthTexture);

        // Fullscreen quad drawcall
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
