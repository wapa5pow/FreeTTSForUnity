using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace FreeTTS
{
	public class TtsForm : MonoBehaviour
	{
		[SerializeField] private InputField _inputField;
		[SerializeField] private InputField _rateInputField;
		[SerializeField] private InputField _pitchInputField;
		[SerializeField] private InputField _languageInputField;
		[SerializeField] private Button _speakButton;
		[SerializeField] private Button _stopButton;

		[DllImport("__Internal")]
		private static extern void Speech (string text, string language, float rate, float pitch);
		[DllImport("__Internal")]
		private static extern void Stop ();

		// Use this for initialization
		void Start ()
		{
			_inputField.text = "我愛你";
			_rateInputField.text = "0.5";
			_pitchInputField.text = "1.0";
			_languageInputField.text = "zh-CN";
		}

		public void OnSpeakClick()
		{
			Debug.Log("OnSpeakClick");

			var text = _inputField.text;
			var language = _languageInputField.text;
			var rate = float.Parse(_rateInputField.text);
			var pitch = float.Parse(_pitchInputField.text);
			Debug.Log(rate);
			Debug.Log(pitch);

			if (Application.isEditor)
			{
				return;
			}

			switch (Application.platform)
			{
				case RuntimePlatform.IPhonePlayer:
					Speech(text, language, rate, pitch);
					break;
				case RuntimePlatform.Android:
					break;
			}
		}

		public void OnStopClick()
		{
			Debug.Log("OnStopClick");

			if (Application.isEditor)
			{
				return;
			}

			switch (Application.platform)
			{
				case RuntimePlatform.IPhonePlayer:
					Stop();
					break;
				case RuntimePlatform.Android:
					break;
			}
		}
	}
}
