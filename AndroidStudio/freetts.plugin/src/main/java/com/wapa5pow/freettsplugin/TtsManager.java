package com.wapa5pow.freettsplugin;

import android.annotation.TargetApi;
import android.content.Context;
import android.os.Build;
import android.speech.tts.TextToSpeech;
import android.speech.tts.UtteranceProgressListener;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Locale;
import java.util.Set;

public class TtsManager implements TextToSpeech.OnInitListener {
    private final String TAG = "TtsManager";

    private static final TtsManager instance = new TtsManager();
    private TextToSpeech tts;

    public static TtsManager getInstance() {
        return instance;
    }

    public void initialize(Context context) {
        tts = new TextToSpeech(context, this);
        tts.setOnUtteranceProgressListener(new UtteranceProgressListener()
        {
            @Override
            public void onStart(String utteranceId)
            {
                Log.d(TAG, "progress on Start " + utteranceId);
                UnityPlayer.UnitySendMessage("FreeTtsManager", "OnCallBack", "start");
            }

            @Override
            public void onDone(String utteranceId)
            {
                Log.d(TAG, "progress on Done " + utteranceId);
                UnityPlayer.UnitySendMessage("FreeTtsManager", "OnCallBack", "finish");
            }

            @Override
            public void onError(String utteranceId)
            {
                Log.d(TAG, "progress on Error " + utteranceId);
                UnityPlayer.UnitySendMessage("FreeTtsManager", "OnCallBack", "finish");
            }

            @Override
            public void onStop(String utteranceId, boolean interrupted)
            {
                Log.d(TAG, "progress on Stop " + utteranceId);
                UnityPlayer.UnitySendMessage("FreeTtsManager", "OnCallBack", "cancel");
            }
        });
    }

    @Override
    public void onInit(int status) {
        if (TextToSpeech.SUCCESS == status) {
        } else {
            Log.d(TAG, "Error init");
        }
    }

    public void speechText(String text, String language, float rate, float pitch) {
        if (0 < text.length()) {
            tts.setSpeechRate(rate);
            tts.setPitch(pitch);
            tts.setLanguage(getLocal(language));

            if (tts.isSpeaking()) {
                tts.stop();
            }
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                ttsGreater21(text);
            } else {
                ttsUnder20(text);
            }
        }
    }

    public void stop() {
        tts.stop();
    }

    public ArrayList<String> getAvailableLanguages() {
        Set<Locale> locales = new HashSet<>();
        ArrayList<String> languages = new ArrayList<>();

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            locales = tts.getAvailableLanguages();

            for (Locale locale: locales) {
                languages.add(locale.toLanguageTag());
            }
        }

        return languages;
    }

    private Locale getLocal(String local) {
        Locale locale;
        try {
            String[] localStrings = local.split("-");
            locale = new Locale(localStrings[0], localStrings[1]);
        } catch (Exception e) {
            locale = new Locale("en", "US");
        }
        return locale;
    }

    @SuppressWarnings("deprecation")
    private void ttsUnder20(String text) {
        tts.speak(text, TextToSpeech.QUEUE_FLUSH, null);
    }

    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    private void ttsGreater21(String text) {
        String utteranceId = this.hashCode() + "";
        tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, utteranceId);
    }

}
