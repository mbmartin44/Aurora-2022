//
//  NTDeviceInfo.h
//  bluetoothle
//
//  Created by admin on 23.12.2019.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSUInteger, NTChannelType);

@interface NTDeviceInfo : NSObject

@property (nonatomic, readonly, copy) NSString *_Nonnull name;
@property (nonatomic, readonly, copy) NSString *_Nonnull address;
@property (nonatomic, readonly) uint64_t serialNumber;
- (nonnull instancetype)initWithName:(NSString *_Nonnull)name address:(NSString *_Nonnull)address serialNumber:(uint64_t)serialNumber NS_DESIGNATED_INITIALIZER;
@end

@interface NTChannelInfo : NSObject
@property (nonatomic, readonly, copy) NSString *_Nonnull name;
@property (nonatomic, readonly) enum NTChannelType type;
@property (nonatomic, readonly) NSInteger index;
@end

typedef NS_ENUM (NSUInteger, NTParamAccess);
typedef NS_ENUM (NSUInteger, NTParameter);

@interface NTParameterInfo : NSObject
@property (nonatomic, readonly) enum NTParameter parameter;
@property (nonatomic, readonly) enum NTParamAccess access;
@end

typedef NS_ENUM (NSUInteger, NTParamAccess) {
    NTParamAccessRead       = 0,
    NTParamAccessReadWrite  = 1,
    NTParamAccessReadNotify = 2,
    NTParamAccessNone       = 3,
};

typedef NS_ENUM (NSUInteger, NTParameter) {
    NTParameterName,
    NTParameterState,
    NTParameterAddress,
    NTParameterSerialNumber,
    NTParameterHardwareFilterState,
    NTParameterFirmwareMode,
    NTParameterSamplingFrequency,
    NTParameterGain,
    NTParameterOffset,
    NTParameterExternalSwitchState,
    NTParameterADCInputState,
    NTParameterAccelerometerSens,
    NTParameterGyroscopeSens,
    NTParameterStimulatorAndMAState,
    NTParameterStimulatorParamPack,
    NTParameterMotionAssistantParamPack,
    NTParameterFirmwareVersion,
    NTParameterMEMSCalibrationStatus,
    NTParameterNone
};

typedef NS_ENUM (NSUInteger, NTCommand) {
    NTCommandStartSignal,
    NTCommandStopSignal,
    NTCommandStartResist,
    NTCommandStopResist,
    NTCommandStartMEMS,
    NTCommandStopMEMS,
    NTCommandStartRespiration,
    NTCommandStopRespiration,
    NTCommandStartStimulation,
    NTCommandStopStimulation,
    NTCommandEnableMotionAssistant,
    NTCommandDisableMotionAssistant,
    NTCommandFindMe,
    NTCommandStartAngle,
    NTCommandStopAngle,
    NTCommandCalibrateMEMS,
    NTCommandResetQuaternion,
    NTCommandStartEnvelope,
    NTCommandStopEnvelope,
    NTCommandNone                 
};

typedef NS_ENUM (NSUInteger, NTChannelType) {
    NTChannelTypeSignal          = 0,
    NTChannelTypeBrainbitSync    = 1,
    NTChannelTypeBattery         = 2,
    NTChannelTypeElectrodesState = 3,
    NTChannelTypeRespiration     = 4,
    NTChannelTypeMEMS            = 5,
    NTChannelTypeOrientation     = 6,
    NTChannelTypeResistance      = 7,
    NTChannelTypePedometer       = 8,
    NTChannelTypeCustom          = 9,
    NTChannelTypeEnvelope        = 10,
    NTChannelTypeNone            = 11,
};

NS_ASSUME_NONNULL_END
