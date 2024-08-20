using UnityEngine;
using UnityEngine.UIElements;

namespace Bibcam.UI {

public static class CustomConverter
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    #else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    #endif
    public static void RegisterConverters()
    {
        var grp = new ConverterGroup("Float to String (Two Decimal)");
        grp.AddConverter((ref float v) => $"{v:0.00}");
        ConverterGroups.RegisterConverterGroup(grp);
    }
}

} // namespace Bibcam.UI
