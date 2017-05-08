using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FreeTts;

public class TtsForm : MonoBehaviour
{
	[SerializeField] private InputField _inputField;
	[SerializeField] private InputField _rateInputField;
	[SerializeField] private InputField _pitchInputField;
	[SerializeField] private InputField _languageInputField;
	[SerializeField] private Button _speakButton;
	[SerializeField] private Button _stopButton;
	private FreeTtsManager _tts;
	private List<string> _texts = new List<string>();

	void Start ()
	{
		_inputField.text = "我愛你 我恨你";
		_rateInputField.text = "0.5";
		_pitchInputField.text = "1.0";
		_languageInputField.text = "zh-CN";

		if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
		{
			var nativeDialog = new AndroidJavaClass ("com.wapa5pow.freettsplugin.TtsManagerPlugin");
			var unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			var context = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			context.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
				nativeDialog.CallStatic (
					"initialize",
					context
				);
			}));
		}
	}

	public void OnSpeakClick()
	{
		Debug.Log("OnSpeakClick");

		var inputTexts = _inputField.text;
		_texts.Clear();
		foreach (var text in inputTexts.Split(' '))
		{
			_texts.Add(text);
		}
		SpeakTextsIfExists();
	}

	public void OnStopClick()
	{
		Debug.Log("OnStopClick");
		_texts.Clear();

		if (Application.isEditor)
		{
			return;
		}

		if (_tts != null)
		{
			_tts.StopSpeech();
		}
	}

	private void SpeakTextsIfExists()
	{
		if (_texts.Count < 1)
		{
			return;
		}

		var language = _languageInputField.text;
		var rate = float.Parse(_rateInputField.text);
		var pitch = float.Parse(_pitchInputField.text);

		var text = _texts.First();
		_texts.RemoveAt(0);
		_tts = FreeTtsManager.Create(text, language, rate, pitch);
		_tts.OnSpeakComplete += OnSpeakComplete;
	}

	private void OnSpeakComplete(FreeTtsResult result) {
		Debug.Log("OnFinishSpeaking");
		//parsing result
		switch(result) {
			case FreeTtsResult.Finish:
				Debug.Log ("Finish!!!");
				SpeakTextsIfExists();
				break;
			case FreeTtsResult.Cancel:
				Debug.Log ("Finish!!!");
				break;
		}
	}

}
