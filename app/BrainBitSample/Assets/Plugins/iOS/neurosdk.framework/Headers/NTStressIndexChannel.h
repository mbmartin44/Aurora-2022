//
//  StressIndexChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTRPeakChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTStressIndexChannel : NTBaseChannel
- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithRPeakChannel:(NTRPeakChannel *_Nullable) rpeakChannel NS_DESIGNATED_INITIALIZER;

/// Read array of Double from stress index channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
