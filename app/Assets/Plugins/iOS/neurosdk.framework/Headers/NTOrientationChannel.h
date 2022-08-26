//
//  NTOrientationChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTOrientationValue : NSObject
@property (nonatomic) float W;
@property (nonatomic) float X;
@property (nonatomic) float Y;
@property (nonatomic) float Z;

- (nonnull instancetype)initWithW:(float) w x:(float) x y:(float) y z:(float) z;
@end

@interface NTOrientationChannel : NTBaseChannel

- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;
/// Read array of NTOrientationValue from Orientation channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTOrientationValue *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));

@end

NS_ASSUME_NONNULL_END
