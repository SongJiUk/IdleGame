using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;

public class iOSPostProcessBuild
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict root = plist.root;

        // ATT 권한 요청 문구 (기기 언어 무관하게 표시될 기본값)
        root.SetString("NSUserTrackingUsageDescription",
            "맞춤형 광고 제공을 위해 추적 권한을 요청합니다.\nWe use tracking to provide personalized ads and improve your experience.");

        plist.WriteToFile(plistPath);
        Debug.Log("[iOSPostProcessBuild] NSUserTrackingUsageDescription 추가 완료");
    }
}
#endif
