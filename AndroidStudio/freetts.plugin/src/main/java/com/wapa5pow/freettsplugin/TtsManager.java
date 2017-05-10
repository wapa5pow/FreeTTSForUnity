package com.wapa5pow.freettsplugin;

import android.annotation.TargetApi;
import android.content.Context;
import android.os.Build;
import android.speech.tts.TextToSpeech;
import android.speech.tts.UtteranceProgressListener;
import android.text.TextUtils;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;

class TtsManager implements TextToSpeech.OnInitListener {
    private final String TAG = "TtsManager";

    private static final TtsManager instance = new TtsManager();
    private TextToSpeech tts;

    static TtsManager getInstance() {
        return instance;
    }

    void initialize(Context context) {
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
            List<String> languages = new ArrayList<>();
            for (Locale locale : Locale.getAvailableLocales()) {
                if (tts.isLanguageAvailable(locale) > 0) {
                    languages.add(toBcp47Language(locale));
                }
            }
            Collections.sort(languages);
            UnityPlayer.UnitySendMessage(
                    "TtsForm", "AddLanguageButtons", TextUtils.join(",", languages));
        } else {
            Log.d(TAG, "Error init");
        }
    }

    void speechText(String text, String language, float rate, float pitch) {
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

    void stop() {
        tts.stop();
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
        String utteranceId = this.hashCode() + "";
        HashMap<String, String> params = new HashMap<>();
        params.put(TextToSpeech.Engine.KEY_PARAM_UTTERANCE_ID, utteranceId);
        tts.speak(text, TextToSpeech.QUEUE_FLUSH, params);
    }

    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    private void ttsGreater21(String text) {
        String utteranceId = this.hashCode() + "";
        tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, utteranceId);
    }

    /**
     * Modified from:
     * https://github.com/apache/cordova-plugin-globalization/blob/master/src/android/Globalization.java
     *
     * Returns a well-formed ITEF BCP 47 language tag representing this locale string
     * identifier for the client's current locale
     *
     * @return String: The BCP 47 language tag for the current locale
     */
    public static String toBcp47Language(Locale loc) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            return loc.toLanguageTag();
        }

        // we will use a dash as per BCP 47
        final char SEP = '-';
        String language = loc.getLanguage();
        String region = loc.getCountry();
        String variant = loc.getVariant();

        // special case for Norwegian Nynorsk since "NY" cannot be a variant as per BCP 47
        // this goes before the string matching since "NY" wont pass the variant checks
        if (language.equals("no") && region.equals("NO") && variant.equals("NY")) {
            language = "nn";
            region = "NO";
            variant = "";
        }

        if (language.isEmpty() || !language.matches("\\p{Alpha}{2,8}")) {
            language = "und";       // Follow the Locale#toLanguageTag() implementation
            // which says to return "und" for Undetermined
        } else if (language.equals("iw")) {
            language = "he";        // correct deprecated "Hebrew"
        } else if (language.equals("in")) {
            language = "id";        // correct deprecated "Indonesian"
        } else if (language.equals("ji")) {
            language = "yi";        // correct deprecated "Yiddish"
        }

        // ensure valid country code, if not well formed, it's omitted
        if (!region.matches("\\p{Alpha}{2}|\\p{Digit}{3}")) {
            region = "";
        }

        // variant subtags that begin with a letter must be at least 5 characters long
        if (!variant.matches("\\p{Alnum}{5,8}|\\p{Digit}\\p{Alnum}{3}")) {
            variant = "";
        }

        StringBuilder bcp47Tag = new StringBuilder(language);
        if (!region.isEmpty()) {
            bcp47Tag.append(SEP).append(region);
        }
        if (!variant.isEmpty()) {
            bcp47Tag.append(SEP).append(variant);
        }

        return bcp47Tag.toString();
    }

}
