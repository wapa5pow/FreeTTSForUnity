//
//  TtsManager.swift
//  TTSSample
//
//  Created by Koichi Ishida on 2017/04/27.
//  Copyright © 2017年 Koichi Ishida. All rights reserved.
//

import AVFoundation

public class TtsManager: NSObject, AVSpeechSynthesizerDelegate {
    static let shared = TtsManager()
    let synthesizer = AVSpeechSynthesizer()
    
    private override init() {
        super.init()
        synthesizer.delegate = self
    }
    
    public func speak(_ text: String, language: String, rate: Float, pitch: Float)
    {
        if (text.characters.count < 1) {
            return
        }
        
        let speechUtterance = AVSpeechUtterance(string: text)
        speechUtterance.voice = AVSpeechSynthesisVoice(language: language)
        speechUtterance.rate = rate
        speechUtterance.pitchMultiplier = pitch
        if synthesizer.isSpeaking {
            synthesizer.stopSpeaking(at: .immediate)
        }
        synthesizer.speak(speechUtterance)
    }
    
    public func stop()
    {
        synthesizer.stopSpeaking(at: .immediate)
    }
    
    public func updateLanguages()
    {
        var languages = AVSpeechSynthesisVoice.speechVoices().map { $0.language }
        languages = Array(Set(languages))
        languages.sort()
        UnitySendMessage("TtsForm", "AddLanguageButtons", languages.joined(separator: ","))
    }
    
    // MARK: AVSpeechSynthesizerDelegate
    public func speechSynthesizer(_ synthesizer: AVSpeechSynthesizer, didStart utterance: AVSpeechUtterance) {
        print("start")
        UnitySendMessage("FreeTtsManager", "OnCallBack", "start")
    }
    
    public func speechSynthesizer(_ synthesizer: AVSpeechSynthesizer, didCancel utterance: AVSpeechUtterance) {
        print("cancel")
        UnitySendMessage("FreeTtsManager", "OnCallBack", "cancel")
    }
    
    public func speechSynthesizer(_ synthesizer: AVSpeechSynthesizer, didFinish utterance: AVSpeechUtterance)
    {
        print("finish")
        UnitySendMessage("FreeTtsManager", "OnCallBack", "finish")
    }
    
    
}
