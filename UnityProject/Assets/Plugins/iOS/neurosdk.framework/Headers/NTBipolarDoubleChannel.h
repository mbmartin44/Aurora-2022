//
//  NTBipolarDoubleChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTBipolarDoubleChannel : NTBaseChannel
- (instancetype)init NS_UNAVAILABLE; 
- (nullable instancetype)initWithLeftChannel:(NTBaseChannel *_Nullable)leftChannel rightChannel:(NTBaseChannel *_Nullable)rightChannel NS_DESIGNATED_INITIALIZER;

/// Read array of Double from bipolar channels
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NSNumber *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
