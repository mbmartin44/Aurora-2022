//
//  NTEmulationChannel.h
//  neurosdk
//
//  Created by admin on 13.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTEmulationSine : NSObject
@property (nonatomic, readonly) double AmplitudeV;
@property (nonatomic, readonly) double FrequencyHz;
@property (nonatomic, readonly) double PhaseShiftRad;

- (nonnull instancetype)initWithAmplitudeV:(double)amplitudeV frequencyHz:(double)frequencyHz phaseShiftRad: (double) phaseShiftRad;
@end

@interface NTEmulationChannel : NTBaseChannel
- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithChannel:(NSArray<NTEmulationSine*> *) components samplingFrequency:(float) samplingFrequency initialLength:(size_t) initialLength NS_DESIGNATED_INITIALIZER;


- (void) startTimer;
- (void) stopTimer;
- (void) resetTimer;


/// Read array of Double from emulation channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
