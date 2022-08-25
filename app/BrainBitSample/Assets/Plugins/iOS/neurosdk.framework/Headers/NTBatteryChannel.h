//
//  NTBatteryChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTBatteryChannel : NTBaseChannel

- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;

/// Read array of Int from battery channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
