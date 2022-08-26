//
//  NTElectrodeStateChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSUInteger, NTElectrodeStateEnum) {
    NTElectrodeStateEnumStateNormal,
    NTElectrodeStateEnumStateHighResistance,
    NTElectrodeStateEnumStateDetached
};

@interface NTElectrodeState : NSObject
@property (nonatomic, readonly) enum NTElectrodeStateEnum type;
@end

@interface NTElectrodeStateChannel : NTBaseChannel

- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;

/// Read array of NTElectrodeState from electrode state channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTElectrodeState *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
