//
//  NTDevice+Extension.h
//  neurosdk
//
//  Created by admin on 26.12.2019.
//  Copyright © 2019 NeuroMD. All rights reserved.
//

#import "NTDevice.h"

typedef NS_ENUM (NSUInteger, NTState) {
    NTStateDisconnected,
    NTStateConnected
};

typedef NS_ENUM (NSUInteger, NTFirmwareMode) {
    NTFirmwareModeApplication,
    NTFirmwareModeBootloader
};

typedef NS_ENUM (NSUInteger, NTSamplingFrequency) {
    NTSamplingFrequencyHz125,
    NTSamplingFrequencyHz250,
    NTSamplingFrequencyHz500,
    NTSamplingFrequencyHz1000,
    NTSamplingFrequencyHz2000,
    NTSamplingFrequencyHz4000,
    NTSamplingFrequencyHz8000
};

typedef NS_ENUM (NSUInteger, NTGain) {
    NTGainG1,
    NTGainG2,
    NTGainG3,
    NTGainG4,
    NTGainG6,
    NTGainG8,
    NTGainG12
};

typedef NS_ENUM (NSUInteger, NTExternalSwitchInput) {
    NTExternalSwitchInputMioElectrodesRespUSB,
    NTExternalSwitchInputMioElectrodes,
    NTExternalSwitchInputMioUSB,
    NTExternalSwitchInputRespUSB
};

typedef NS_ENUM (NSUInteger, NTADCInput) {
    NTADCInputElectrodes,
    NTADCInputShort,
    NTADCInputTest,
    NTADCInputResistance
};

typedef NS_ENUM (NSUInteger, NTAccelerometerSensitivity) {
    NTAccelerometerSensitivitySens2g,
    NTAccelerometerSensitivitySens4g,
    NTAccelerometerSensitivitySens8g,
    NTAccelerometerSensitivitySens16g
};

typedef NS_ENUM (NSUInteger, NTGyroscopeSensitivity) {
    NTGyroscopeSensitivitySens250Grad,
    NTGyroscopeSensitivitySens500Grad,
    NTGyroscopeSensitivitySens1000Grad,
    NTGyroscopeSensitivitySens2000Grad
};

typedef NS_ENUM (NSUInteger, NTStimulationDeviceState) {
    NTStimulationDeviceStateNoParams,
    NTStimulationDeviceStateDisabled,
    NTStimulationDeviceStateEnabled
};

typedef NS_ENUM (NSUInteger, NTMotionAssistantLimb) {
    NTMotionAssistantLimbRightLeg,
    NTMotionAssistantLimbLeftLeg,
    NTMotionAssistantLimbRightArm,
    NTMotionAssistantLimbLeftArm
};

NS_ASSUME_NONNULL_BEGIN

@interface NTStimulatorAndMaState : NSObject
@property (nonatomic) enum NTStimulationDeviceState StimulatorState;
@property (nonatomic) enum NTStimulationDeviceState MAState;

- (nonnull instancetype)initWithStimulatorState:(enum NTStimulationDeviceState) stimulatorState andMAState:(enum NTStimulationDeviceState)MAState;

@end

@interface NTStimulationParams : NSObject
@property (nonatomic) int current;
@property (nonatomic) int pulseWidth;
@property (nonatomic) int frequency;
@property (nonatomic) int stimulusDuration;

- (nonnull instancetype)initWithCurrent:(int)current pulseWidth:(int)pulseWidth frequency:(int)frequency stimulusDuration:(int)stimulusDuration;

@end

@interface NTMotionAssistantParams : NSObject
@property (nonatomic) int gyroStart;
@property (nonatomic) int gyroStop;
@property (nonatomic) enum NTMotionAssistantLimb limb;
@property (nonatomic) int minPause;

- (nonnull instancetype)initWithGyroStart:(int)gyroStart gyroStop:(int)gyroStop limb:(enum NTMotionAssistantLimb) limb minPause:(int)minPause;
@end

@interface NTFirmwareVersion : NSObject
@property (nonatomic) unsigned int version;
@property (nonatomic) unsigned int build;

- (nonnull instancetype)initWithVersion:(unsigned int)version build:(unsigned int)build;
@end

@interface NTDevice (Extension)

- (NSString *)readName:(NSError **) error  NS_SWIFT_NAME(name(error:));
- (enum NTState) readState:(NSError **) error                    NS_SWIFT_NAME(state(error:));
- (NSString *)readAddress:(NSError **) error                  NS_SWIFT_NAME(address(error:));
- (NSString *)readSerialNumber:(NSError **) error             NS_SWIFT_NAME(serialNumber(error:));
- (BOOL)readHardwareFilterState:(NSError **) error      NS_SWIFT_NAME(hardwareFilterState(error:));
- (NTFirmwareMode)readFirmwareMode:(NSError **) error             NS_SWIFT_NAME(firmwareMode(error:));
- (NTSamplingFrequency)readSamplingFrequency:(NSError **) error        NS_SWIFT_NAME(samplingFrequency(error:));
- (NTGain)readGain:(NSError **) error                     NS_SWIFT_NAME(gain(error:));
- (unsigned char)readOffset:(NSError **) error                  NS_SWIFT_NAME(offset(error:));
- (NTExternalSwitchInput)readExternalSwitchState:(NSError **) error      NS_SWIFT_NAME(externalSwitchState(error:));
- (NTADCInput)readADCInputState:(NSError **) error           NS_SWIFT_NAME(ADCInputState(error:));
- (NTAccelerometerSensitivity)readAccelerometerSens:(NSError **) error        NS_SWIFT_NAME(accelerometerSens(error:));
- (NTGyroscopeSensitivity)readGyroscopeSens:(NSError **) error            NS_SWIFT_NAME(gyroscopeSens(error:));
- (NTStimulatorAndMaState *)readStimulatorAndMAState:(NSError **) error     NS_SWIFT_NAME(stimulatorAndMAState(error:));
- (NTStimulationParams *)readStimulatorParamPack:(NSError **) error      NS_SWIFT_NAME(stimulatorParamPack(error:));
- (NTMotionAssistantParams *)readMotionAssistantParamPack:(NSError **) error NS_SWIFT_NAME(motionAssistantParamPack(error:));
- (NTFirmwareVersion *)readFirmwareVersion:(NSError **) error          NS_SWIFT_NAME(firmwareVersion(error:));
- (bool)readMEMSCalibrationStatus:(NSError **) error    NS_SWIFT_NAME(MEMSCalibrationStatus(error:));

- (void)setHardwareFilterState:(BOOL)hardwareFilterState error: (NSError **) error;
- (void)setFirmwareMode:(enum NTFirmwareMode)firmwareMode error: (NSError **) error;
- (void)setSamplingFrequency:(enum NTSamplingFrequency)samplingFrequency error: (NSError **) error;
- (void)setGain:(enum NTGain)gain error: (NSError **) error;
- (void)setOffset:(unsigned char)offset error: (NSError **) error;
- (void)setExternalSwitchInput:(enum NTExternalSwitchInput)externalSwitchInput error: (NSError **) error;
- (void)setADCInputState:(enum NTADCInput)ADCInput error: (NSError **) error;
- (void)setAccelerometerSens:(enum NTAccelerometerSensitivity)accelerometerSensitivity error: (NSError **) error;
- (void)setGyroscopeSens:(enum NTGyroscopeSensitivity)gyroscopeSensitivity error: (NSError **) error;
- (void)setStimulatorParamPack:(NTStimulationParams *)stimulationParams error: (NSError **) error;
- (void)setMotionAssistantParamPack:(NTMotionAssistantParams *)motionAssistantParams error: (NSError **) error;
@end

@interface NTDeviceTraits : NSObject

+ (BOOL)HasChannelsWithType:(NTDevice *)device channelType:(NTChannelType)channelType error: (NSError **) error;

+ (NSArray<NTChannelInfo *> *)GetChannelInfoArrayWithType:(NTDevice *)device channelType:(NTChannelType)channelType error: (NSError **) error;

@end

NS_ASSUME_NONNULL_END
