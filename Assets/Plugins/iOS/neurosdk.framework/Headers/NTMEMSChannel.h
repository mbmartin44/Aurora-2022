//
//  NTMEMSChannel.h
//  neurosdk
//
//  Created by admin on 21.01.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

struct Point3D {
    double X;
    double Y;
    double Z;
};

@interface NTMEMSValue : NSObject
@property (nonatomic, readonly) struct Point3D accelerometer;
@property (nonatomic, readonly) struct Point3D gyroscope;

//- (instancetype)init NS_UNAVAILABLE;

- (nonnull instancetype)initWithAccelerometer:(struct Point3D) accelerometer gyroscope:(struct Point3D) gyroscope NS_DESIGNATED_INITIALIZER;
@end

@interface NTMEMSChannel : NTBaseChannel

- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;
/// Read array of NTMEMSValue from MEMS channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTMEMSValue *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
