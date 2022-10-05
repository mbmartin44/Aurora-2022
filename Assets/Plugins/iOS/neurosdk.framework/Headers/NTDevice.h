//
//  NTDevice.h
//  bluetoothle
//
//  Created by admin on 23.12.2019.
//

#import <Foundation/Foundation.h>

#import "NTDeviceEnumerator.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTDevice : NSObject
- (instancetype)init NS_UNAVAILABLE;

- (nullable instancetype)initWithEnumerator:(NTDeviceEnumerator *_Nonnull) enumerator deviceInfo:(NTDeviceInfo *_Nonnull)deviceInfo NS_DESIGNATED_INITIALIZER;


/// Subscribe to parameter changed
/// @param subscriber To unsubscribe pass nil
/// @note Return YES if success
- (BOOL)subscribeParameterChangedWithSubscriber:(void (^_Nullable)(enum NTParameter))subscriber;

/// Connect to device (brainbit or calibri)
/// @return Return YES if success
- (BOOL)connect;

/// Disconnect to device (brainbit or calibri)
/// @return Return YES if success
- (BOOL)disconnect;

/// Execute device command (brainbit or calibri)
/// @return Return YES if success
- (BOOL)executeWithCommand:(enum NTCommand) command NS_SWIFT_NAME(execute(command:));

/// @return nil if exception occur
@property (NS_NONATOMIC_IOSONLY, readonly, copy) NSArray<NTChannelInfo *> *_Nullable channels;
/// @return nil if exception occur
@property (NS_NONATOMIC_IOSONLY, readonly, copy) NSArray<NSNumber *> *_Nullable commands;
/// @return nil if exception occur
@property (NS_NONATOMIC_IOSONLY, readonly, copy) NSArray<NTParameterInfo *> *_Nullable parameters;
@end
NS_ASSUME_NONNULL_END
