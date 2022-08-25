//
//  NTRespirationChannel.h
//  neurosdk
//
//  Created by Vladimir on 16.03.2021.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTRespirationChannel : NTBaseChannel

- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithDevice:(NTDevice *)device channelInfo:(NTChannelInfo *__nullable)channelInfo NS_DESIGNATED_INITIALIZER;

/// Read array of Double from resistance channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
