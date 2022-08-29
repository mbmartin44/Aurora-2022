//
//  NTSignalChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTBaseChannel.h"

typedef NS_ENUM (NSUInteger, NTFilterEnum) {
    NTFilterEnumLowPass_1Hz_SF125,
    NTFilterEnumLowPass_1Hz_SF125_Reverse,
    NTFilterEnumLowPass_5Hz_SF125,
    NTFilterEnumLowPass_5Hz_SF125_Reverse,
    NTFilterEnumLowPass_15Hz_SF125,
    NTFilterEnumLowPass_15Hz_SF125_Reverse,
    NTFilterEnumLowPass_27Hz_SF125,
    NTFilterEnumLowPass_27Hz_SF125_Reverse,
    NTFilterEnumLowPass_30Hz_SF250,
    NTFilterEnumLowPass_30Hz_SF250_Reverse,

    NTFilterEnumHighPass_2Hz_SF250,
    NTFilterEnumHighPass_2Hz_SF250_Reverse,
    NTFilterEnumHighPass_3Hz_SF125,
    NTFilterEnumHighPass_3Hz_SF125_Reverse,
    NTFilterEnumHighPass_5Hz_SF125,
    NTFilterEnumHighPass_5Hz_SF125_Reverse,
    NTFilterEnumHighPass_11Hz_SF125,
    NTFilterEnumHighPass_11Hz_SF125_Reverse,

    NTFilterEnumBandStop_45_55Hz_SF250
};

NS_ASSUME_NONNULL_BEGIN

@interface NTFilter : NSObject

@property (nonatomic) enum NTFilterEnum value;

- (nonnull instancetype)initWithFilter:(NTFilterEnum) filter;
@end

@interface NTSignalChannel : NTBaseChannel

- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;
- (nullable instancetype)initWithDevice:(NTDevice *)device filters: (NSArray<NTFilter*>*) filters NS_DESIGNATED_INITIALIZER;
- (nullable instancetype)initWithDevice:(NTDevice *)device channelInfo:(NTChannelInfo *)channelInfo NS_DESIGNATED_INITIALIZER;

/// Read array of Double from signal channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));

@end

NS_ASSUME_NONNULL_END
