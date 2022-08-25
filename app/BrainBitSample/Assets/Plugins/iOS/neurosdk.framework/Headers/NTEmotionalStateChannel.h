//
//  NTEmotionalStateChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTEegIndexChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTEmotionalState : NSObject
@property (nonatomic, readonly) double RelaxationRate;
@property (nonatomic, readonly) double ConcentrationRate;
@end

@interface NTEmotionalStateChannel : NTBaseChannel
- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithIndexChannel:(NTEegIndexChannel *_Nullable)indexChannel NS_DESIGNATED_INITIALIZER;

/// Read array of NTEmotionalState from emotional state channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTEmotionalState *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));

@end

NS_ASSUME_NONNULL_END
