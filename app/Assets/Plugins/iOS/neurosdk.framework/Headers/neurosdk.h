//
//  neurosdk.h
//  neurosdk
//
//  Created by Evgeny Samoylichenko on 04/01/2019.
//  Copyright © 2019 NeuroMD. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for neurosdk.
FOUNDATION_EXPORT double neurosdkVersionNumber;

//! Project version string for neurosdk.
FOUNDATION_EXPORT const unsigned char neurosdkVersionString[];

#import <neurosdk/NTDeviceEnumerator.h>
#import <neurosdk/NTDeviceInfo.h>
#import <neurosdk/NTDevice.h>
#import <neurosdk/NTDevice+Extension.h>

#import <neurosdk/NTBaseChannel.h>
#import <neurosdk/NTBatteryChannel.h>
#import <neurosdk/NTBipolarDoubleChannel.h>
#import <neurosdk/NTEcgChannel.h>
#import <neurosdk/NTEegArtifactChannel.h>
#import <neurosdk/NTEegChannel.h>
#import <neurosdk/NTEegIndexChannel.h>
#import <neurosdk/NTElectrodeStateChannel.h>
#import <neurosdk/NTEmotionalStateChannel.h>
#import <neurosdk/NTEmulationChannel.h>
#import <neurosdk/NTHeartRateChannel.h>
#import <neurosdk/NTMEMSChannel.h>
#import <neurosdk/NTOrientationChannel.h>
#import <neurosdk/NTResistanceChannel.h>
#import <neurosdk/NTRPeakChannel.h>
#import <neurosdk/NTSignalChannel.h>
#import <neurosdk/NTSpectrumChannel.h>
#import <neurosdk/NTSpectrumPowerDoubleChannel.h>
#import <neurosdk/NTStressIndexChannel.h>
#import <neurosdk/NTMeditationAnalyzerChannel.h>
#import <neurosdk/NTEmotionsAnalyzer.h>
#import <neurosdk/NTRespirationChannel.h>
#import <neurosdk/NTEnvelopeChannel.h>

