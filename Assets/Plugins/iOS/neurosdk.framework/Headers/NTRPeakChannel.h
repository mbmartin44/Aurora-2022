//
//  NTRPeakChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTSignalChannel.h"
#import "NTElectrodeStateChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTRPeakChannel : NTBaseChannel


- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithSignalChannel: (NTSignalChannel *_Nullable) channel NS_DESIGNATED_INITIALIZER;
- (nullable instancetype)initWithSignalChannel: (NTSignalChannel *_Nullable) signalChannel electrodeStateChannel: (NTElectrodeStateChannel*_Nullable) electrodeChannel NS_DESIGNATED_INITIALIZER;

/// Read array of Int from rpeak channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
