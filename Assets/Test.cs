using UnityEngine;
using UnityEngine.UI;

namespace Bibcam {

sealed class Test : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] Text _text = null;

    public static string Format(Vector3 v)
      => $"({v.x,7:F2}, {v.y,7:F2}, {v.z,7:F2})";

    void Update()
      => _text.text =
           "Position: " + Format(_camera.transform.position) + "\n" +
           "Rotation: " + Format(_camera.transform.rotation.eulerAngles);
}

} // namespace Bibcam
