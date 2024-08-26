using UnityEngine;
using UnityEngine.UIElements;
using Metavido.Decoder;

namespace Metavido.UI {

sealed class MetadataDisplay : MonoBehaviour
{
    [SerializeField] MetadataDecoder _decoder = null;

    string GetMetadataString()
    {
        var data = _decoder.Metadata;
        if (!data.IsValid) return "(Invalid)";
        return $"Position: {data.CameraPosition}\n" +
               $"Rotation: {data.CameraRotation.eulerAngles}\n" +
               $"Center:   {data.CenterShift}\n" +
               $"FoV:      {data.FieldOfView * Mathf.Rad2Deg:F2}\n" +
               $"Range:    {data.DepthRange}\n" +
               $"Hash:     {data.Hash:F7}\n";
    }

    void Update()
      => GetComponent<UIDocument>().rootVisualElement
           .Q<Label>("metadata-label").text = GetMetadataString();
}

} // namespace Metavido.UI
