//
//  NTEcgChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTEcgChannel : NTBaseChannel

- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;
- (nullable instancetype)initWithDevice: (NTDevice *)device channelInfo: (NTChannelInfo *)channelInfo NS_DESIGNATED_INITIALIZER;

/// Read array of Double from ecg channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
