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

    void Update()
    {
        var camera = GetComponent<Camera>();

        var pm = camera.projectionMatrix;
        var pv = new Vector4(pm[0, 2], pm[1, 2], pm[0, 0], pm[1, 1]);

        var v2w = Matrix4x4.TRS(camera.transform.position,
                                camera.transform.rotation,
                                new Vector3(1, 1, -1));

        _material.SetVector(ShaderID.ProjectionVector, pv);
        _material.SetMatrix(ShaderID.InverseViewMatrix, v2w);

        _material.SetTexture(ShaderID.ColorTexture, _decoder.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _decoder.DepthTexture);
    }

    void OnRenderObject()
    {
        var camera = GetComponent<Camera>();
        if (camera != Camera.current) return;
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
