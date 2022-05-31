#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

//
// Xcode project modifier for iOS builds
//
// - Copy the settings bundle (Settings.bundle) to the project directory.
// - Add the bundle file to the build settings.
//

public class PbxModifier
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;

        // Bundle copy
        var bundleName = "Settings.bundle";
        var bundleSource = $"Extra/{bundleName}";
        var bundlePath = $"{path}/{bundleName}";

        if (!Directory.Exists(bundlePath))
            FileUtil.CopyFileOrDirectory(bundleSource, bundlePath);

        // Bundle addition to pbx
        var pbxPath = $"{path}/Unity-iPhone.xcodeproj/project.pbxproj";
        var pbx = new PBXProject();

        pbx.ReadFromFile(pbxPath);
        var bundleGuid = pbx.AddFile(bundlePath, bundleName);
        pbx.AddFileToBuild(pbx.GetUnityMainTargetGuid(), bundleGuid);
        pbx.WriteToFile(pbxPath);
    }
}

#endif
