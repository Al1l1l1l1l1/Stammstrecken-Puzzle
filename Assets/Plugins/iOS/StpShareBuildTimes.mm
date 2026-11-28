#import <UIKit/UIKit.h>
#import "UnityInterface.h"

extern "C" void StpShareBuildTimes(const char *path) {
    NSURL *file = [NSURL fileURLWithPath:[NSString stringWithUTF8String:path]];
    dispatch_async(dispatch_get_main_queue(), ^{
        UIViewController *host = UnityGetGLViewController();
        if (!host || host.presentedViewController) return;
        UIActivityViewController *sheet = [[UIActivityViewController alloc] initWithActivityItems:@[file] applicationActivities:nil];
        sheet.popoverPresentationController.sourceView = host.view;
        sheet.popoverPresentationController.sourceRect = CGRectMake(CGRectGetMidX(host.view.bounds), CGRectGetMidY(host.view.bounds), 1, 1);
        [host presentViewController:sheet animated:YES completion:nil];
    });
}
