#import <UIKit/UIKit.h>

// Neutral user interaction feedback only. Unity's composition owns preference and modal/lifecycle gating.
extern "C" int stp_haptic_interaction(void) {
    if (![NSThread isMainThread]) return 0;
    if (@available(iOS 10.0, *)) {
        UISelectionFeedbackGenerator *feedback = [[UISelectionFeedbackGenerator alloc] init];
        [feedback prepare];
        [feedback selectionChanged];
#if !__has_feature(objc_arc)
        [feedback release];
#endif
        return 1;
    }
    return 0;
}
