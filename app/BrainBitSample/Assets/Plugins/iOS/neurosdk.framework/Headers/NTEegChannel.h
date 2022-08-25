//
//  NTEegChannel.h
//  bluetoothle
//
//  Created by admin on 23.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTEegChannel : NTBaseChannel

- (nullable instancetype)initWithBaseChannel: (NTBaseChannel*_Nullable) channel NS_DESIGNATED_INITIALIZER;
- (nullable instancetype)initWithDevice:(NTDevice *)device channelInfo:(NTChannelInfo *)channelInfo NS_DESIGNATED_INITIALIZER;

/// Read array of Double from eeg channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
