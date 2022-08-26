//
//  NTSpectrumPowerDoubleChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTSpectrumPowerDoubleChannel : NTBaseChannel

- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithChannel:(NSArray<NTBaseChannel *> *_Nullable)channels lowFreq:(float)lowFreq highFreq:(float)highFreq duration:(double)duration NS_DESIGNATED_INITIALIZER;

/// Read array of Double from spectrum power channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));

- (void)setFrequencyBandWithLow:(float)low high:(float) high NS_SWIFT_NAME(setFrequencyBand(low:high:));

- (void)setWindowDurationWithSeconds:(double) seconds NS_SWIFT_NAME(setWindowDuration(seconds:));
- (void)setOverlappingCoefficientWithOverlap:(double) overlap NS_SWIFT_NAME(setOverlappingCoefficient(overlap:));
@end

NS_ASSUME_NONNULL_END
