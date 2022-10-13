//
//  NTEegIndexChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSUInteger, NTEegIndexMode) {
    NTEegIndexModeLeftSide,
    NTEegIndexModeRightSide,
    NTEegIndexModeArtifacts
};

@interface NTEegIndexChannel : NTBaseChannel
- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithT3:(NTBaseChannel *_Nullable)t3 t4:(NTBaseChannel *_Nullable)t3 o1:(NTBaseChannel *_Nullable)o1 o2:(NTBaseChannel *_Nullable)o2 NS_DESIGNATED_INITIALIZER;

/// Read array of NTEegIndexValues from eeg index channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTIndexValues *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));

- (void)setDelayWithSeconds:(double)delay_seconds;
- (void)setWeightCoefficientsWithAlpha:(double)alpha beta:(double)beta delta:(double)delta theta:(double)theta;

@property (NS_NONATOMIC_IOSONLY, getter = getMode, readonly) NTEegIndexMode mode;
@property (NS_NONATOMIC_IOSONLY, getter = getBasePower, readonly) double basePowerLeft;
@property (NS_NONATOMIC_IOSONLY, getter = getBasePower, readonly) double basePowerRight;

@end

NS_ASSUME_NONNULL_END
