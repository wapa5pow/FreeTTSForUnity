using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// XCodeビルド出力後に呼び出されるメソッド
/// XCode側で別途設定が必要な場合は、こちらで対応
/// </summary>
public class XCodeSetting
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		// iOS以外は未処理
		if (buildTarget != BuildTarget.iOS) {
			return;
		}

		// 後々XCodeEditor-for-Unity利用するためフルパス
		var proj = new PBXProject ();
		var projectPath = PBXProject.GetPBXProjectPath (path);
		proj.ReadFromFile (projectPath);

		//        var target = proj.TargetGuidByName (PBXProject.GetUnityTargetName());
		Debug.Log (PBXProject.GetUnityTargetName ());
		string target = proj.TargetGuidByName ("Unity-iPhone");

		// ビルド設定
		proj.SetBuildProperty (target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
		proj.SetBuildProperty (target, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
		proj.SetBuildProperty (target, "SWIFT_OBJC_BRIDGING_HEADER", "$(SRCROOT)/Libraries/Plugins/iOS/UnitySwift-Bridging-Header.h");
		proj.SetBuildProperty (target, "SWIFT_VERSION", "3.0");

		// 設定を保存
		File.WriteAllText (projectPath, proj.WriteToString ());

		// plist設定
//		var plistPath = Path.Combine (path, "Info.plist");
//		var plist = new PlistDocument ();
//		plist.ReadFromFile (plistPath);
//		plist.root["NSPhotoLibraryUsageDescription"] = new PlistElementString("It is used for card image");
//		plist.root["NSCameraUsageDescription"] = new PlistElementString("It is used for card image");
//		plist.root["NSMicrophoneUsageDescription"] = new PlistElementString("It is used for card voice");
//		plist.WriteToFile (plistPath);
	}
}