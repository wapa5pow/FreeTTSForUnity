using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FreeTts
{
	public class FreeTtsManager: MonoBehaviour
	{
		public Action<FreeTtsResult> OnSpeakComplete = delegate {};

		[DllImport("__Internal")]
		private static extern void Speech (string text, string language, float rate, float pitch);
		[DllImport("__Internal")]
		private static extern void Stop ();

		public static FreeTtsManager Create(string text, string language, float rate, float pitch)
		{
			var tts = new GameObject("FreeTtsManager").AddComponent<FreeTtsManager>();
			if (!Application.isEditor)
			{
				switch (Application.platform)
				{
					case RuntimePlatform.IPhonePlayer:
						Speech(text, language, rate, pitch);
						break;
					case RuntimePlatform.Android:
						var nativeDialog = new AndroidJavaClass ("com.wapa5pow.freettsplugin.TtsManagerPlugin");
						var unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
						var context = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

						context.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
							nativeDialog.CallStatic (
								"speech",
								text,
								language,
								rate,
								pitch
							);
						}));
						break;
				}
			} else
			{
				tts.CallCallbackLater();
			}

			return tts;
		}

		public void StopSpeech()
		{
			if (!Application.isEditor)
			{
				switch (Application.platform)
				{
					case RuntimePlatform.IPhonePlayer:
						Stop();
						break;
					case RuntimePlatform.Android:
						var nativeDialog = new AndroidJavaClass ("com.wapa5pow.freettsplugin.TtsManagerPlugin");
						var unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
						var context = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

						context.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
							nativeDialog.CallStatic (
								"stop"
							);
						}));
						break;
				}
			} else
			{
				OnCallBack("cancel");
			}
		}

		public void CallCallbackLater()
		{
			StartCoroutine(OnEditorCallBack("finish"));
		}

		IEnumerator OnEditorCallBack(string result)
		{
			yield return new WaitForSeconds(1);
			OnCallBack(result);
		}


		public void OnCallBack(string result)
		{
			Debug.Log("OnCallBack: " + result);
			if (result == "finish")
			{
				OnSpeakComplete(FreeTtsResult.Finish);
				Destroy(gameObject);
			} else if (result == "cancel")
			{
				OnSpeakComplete(FreeTtsResult.Cancel);
				Destroy(gameObject);
			}
		}
	}

}