//
//  NTEmotionsAnalyzer.h
//  neurosdk
//
//  Created by admin on 28.07.2020.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN


typedef NS_ENUM (NSUInteger, NTSignalSource) {
    NTSignalSourceLeftSide,
    NTSignalSourceRightSide,
    NTSignalSourceArtifact
};

typedef NS_ENUM (NSUInteger, NTPrioritySide) {
    NTPrioritySideNone,
    NTPrioritySideLeft,
    NTPrioritySideRight
};

typedef NS_ENUM (NSUInteger, NTDataQuality) {
    NTDataQualityRaw,
    NTDataQualityCalibrated
};

typedef NS_ENUM (NSUInteger, NTEmotionsAnalyzerState) {
    NTEmotionsAnalyzerStateWorking,
    NTEmotionsAnalyzerStateCalibrating
};

@interface NTCalibrationStatus: NSObject
@property (nonatomic) float Progress;
@property (nonatomic) float ArtifactRate;
@end

@interface NTEmotionsSample: NSObject
@property (nonatomic) NTSignalSource Source;
@property (nonatomic) NTDataQuality Quality;
@property (nonatomic) float DeltaRate;
@property (nonatomic) float ThetaRate;
@property (nonatomic) float AlphaRate;
@property (nonatomic) float BetaRate;
@property (nonatomic) float RelaxationRate;
@property (nonatomic) float ConcentrationRate;
@property (nonatomic) float MeditationProgress;
@end


@interface NTEmotionsAnalyzer : NTBaseChannel

- (nullable instancetype)initWithDevice:(NTDevice *)device NS_DESIGNATED_INITIALIZER;

- (void) calibrate: (NSError **)error;
- (void) reset: (NSError **)error;

- (void) setWeights:(NTIndexValues * _Nonnull)weights error: (NSError**) error;
- (NTIndexValues * _Nullable) getWeights;

- (void) setPrioritySide:(NTPrioritySide) side error: (NSError**) error;
- (NTPrioritySide) getPrioritySide;

- (NTEmotionsAnalyzerState) getState;
- (NTCalibrationStatus*) getCalibrationStatus;

- (void)subscribeEmotionalStateChangedWithSubscribe:(void (^_Nullable)(NTEmotionsSample*))subscribe;
- (void)subscribeAnalyzerStateChangedWithSubscribe:(void (^_Nullable)(NTEmotionsAnalyzerState, NTCalibrationStatus*))subscribe;

@end

NS_ASSUME_NONNULL_END
