//
//  IdfaAuthorizationNativeWrapper.mm
//  IdfaAuthorizationNativeWrapper
//
//  Created by Voodoo on 2020/07/20.
//  Copyright © 2020 Voodoo. All rights reserved.
//
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>

extern "C" {

const int kAuthorizationStatusDenied = 2;
const int kAuthorizationStatusAuthorized = 3;

const int _authorizationStatus() {
    if (@available(iOS 14, *)) {
        return (int)ATTrackingManager.trackingAuthorizationStatus;
    } else {
        return ASIdentifierManager.sharedManager.isAdvertisingTrackingEnabled ? kAuthorizationStatusAuthorized : kAuthorizationStatusDenied;
    }
}

typedef void (*RequestAuthorizationNativeCallback)();

void _requestAuthorization(RequestAuthorizationNativeCallback requestAuthorizationCallback) {
    if (@available(iOS 14, *)) {
        // the user never has been asked to authorize -> request authorization
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            if (NSThread.isMainThread)
            {
                requestAuthorizationCallback();
            } else {
                dispatch_async(dispatch_get_main_queue(),^{ requestAuthorizationCallback();});
            }
        }];
    }
}

void _redirectToAppSettings() {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]
                                       options:@{}
                             completionHandler:NULL];
}

}
