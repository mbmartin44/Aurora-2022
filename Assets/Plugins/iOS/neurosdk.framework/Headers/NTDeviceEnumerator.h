//
//  NTDeviceEnumerator.h
//  bluetoothle
//
//  Created by admin on 23.12.2019.
//

#import <Foundation/Foundation.h>
#import "NTDeviceInfo.h"

typedef NS_ENUM (NSUInteger, NTDeviceType) {
    TypeBrainbit,
    TypeCallibri,
    TypeAny,
    TypeBrainbitBlack,
    TypeBrainbitAny
};

NS_ASSUME_NONNULL_BEGIN

@interface NTDeviceEnumerator : NSObject

- (instancetype)init NS_UNAVAILABLE;

- (nullable instancetype)initWithDeviceType:(enum NTDeviceType) deviceType NS_DESIGNATED_INITIALIZER;

/// @description Add subcriber when device found
/// @remark Pass nil to unsubcribe
- (BOOL)subscribeFoundDeviceWithSubscriber:(void (^_Nullable)(NTDeviceInfo *_Nonnull))subscriber;
@end

NS_ASSUME_NONNULL_END
