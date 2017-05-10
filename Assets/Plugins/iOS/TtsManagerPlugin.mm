#import <AVFoundation/AVFoundation.h>
#import <freetts-Swift.h>

extern "C" {
    void Speech(const char *text, const char *language, float rate, float pitch) {
        [[TtsManager shared] speak:[NSString stringWithUTF8String: text]
                          language:[NSString stringWithUTF8String: language]
                              rate:rate
                             pitch:pitch];
    }
    
    void Stop() {
        [[TtsManager shared] stop];
    }
    
    void Languages() {
        [[TtsManager shared] updateLanguages];
    }
}
