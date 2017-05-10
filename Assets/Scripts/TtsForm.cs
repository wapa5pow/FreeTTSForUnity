using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using FreeTts;

public class TtsForm : MonoBehaviour
{
	[SerializeField] private InputField _inputField;
	[SerializeField] private InputField _rateInputField;
	[SerializeField] private InputField _pitchInputField;
	[SerializeField] private InputField _languageInputField;
	[SerializeField] private GameObject _content;
	[SerializeField] private GameObject _languageList;
	[SerializeField] private Button _chooseLanguageButton;

	private FreeTtsManager _tts;
	private List<string> _texts = new List<string>();

	[DllImport("__Internal")]
	private static extern void Languages ();

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

		_languageList.SetActive(false);
		_chooseLanguageButton.enabled = false;
		if (Application.isEditor)
		{
			AddLanguageButtons("ja-JP,en-US");
		} else
		{
			Languages();
		}
	}

	public void AddLanguageButtons(string languages)
	{
		foreach (Transform c in _content.transform)
		{
			Destroy(c.gameObject);
		}

		foreach (var language in languages.Split(','))
		{
			var button = Instantiate(Resources.Load<GameObject>("LanguageButton")).GetComponent <Button>();
			var l = language;
			var t = _content.transform;
			button.onClick.AddListener(() => SetLanguage(l));
			button.transform.SetParent(t);
			button.transform.localScale = Vector3.one;
			button.GetComponentInChildren<Text>().text = l;
		}

		_chooseLanguageButton.enabled = true;
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

	public void OnChooseLanguageClick()
	{
		Debug.Log("OnChooseLanguageClick");

		_languageList.SetActive(true);
	}

	public void SetLanguage(string language)
	{
		Debug.Log("SetLanguage: " + language);
		_languageInputField.text = language;
		_languageList.SetActive(false);
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

	public void CloseLanguageList()
	{
		_languageList.SetActive(false);
	}
}
