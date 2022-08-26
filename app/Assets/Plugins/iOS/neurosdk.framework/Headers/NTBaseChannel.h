//
//  NTBaseChannel.h
//  bluetoothle
//
//  Created by admin on 23.12.2019.
//

#import <Foundation/Foundation.h>

#import "NTDevice.h"
#import "NTDeviceInfo.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTIndexValues : NSObject
@property (nonatomic, readonly) double AlphaRate;
@property (nonatomic, readonly) double BetaRate;
@property (nonatomic, readonly) double DeltaRate;
@property (nonatomic, readonly) double ThetaRate;
- (nonnull instancetype)initWithAlpha:(double)alpha beta:(double)beta delta:(double)delta theta:(double)theta;
@end

@interface NTBaseChannel : NSObject

/// @description Add subcribe when length is changed
/// @remark Pass nil to unsubcribe
- (void)subscribeLengthChangedWithSubscribe:(void (^_Nullable)(NSUInteger))subscribe;


@property (NS_NONATOMIC_IOSONLY, readonly) NSUInteger totalLength;
@property (NS_NONATOMIC_IOSONLY, readonly) float samplingFrequency;

@property (NS_NONATOMIC_IOSONLY, readonly, strong) NTChannelInfo *_Nullable info;
@end

NS_ASSUME_NONNULL_END
