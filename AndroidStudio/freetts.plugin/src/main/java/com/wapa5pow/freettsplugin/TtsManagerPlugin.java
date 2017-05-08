package com.wapa5pow.freettsplugin;

import android.content.Context;

public class TtsManagerPlugin {
    private static TtsManager ttsManager = TtsManager.getInstance();

    static public void initialize(Context context)
    {
        ttsManager.initialize(context);
    }

    static public void speech(String text, String language, float rate, float pitch)
    {
        ttsManager.speechText(text, language, rate, pitch);
    }

    static public void stop()
    {
        ttsManager.stop();
    }
}
