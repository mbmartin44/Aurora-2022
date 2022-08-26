//
//  NTMeditationAnalyzer.h
//  neurosdk
//
//  Created by admin on 05.03.2020.
//  Copyright © 2020 NeuroMD. All rights reserved.
//

#import "NTEegIndexChannel.h"

NS_ASSUME_NONNULL_BEGIN

@interface NTMeditationAnalyzerChannel : NTBaseChannel

- (instancetype)init NS_UNAVAILABLE;
- (nullable instancetype)initWithIndexChannel:(NTEegIndexChannel *_Nullable)indexChannel NS_DESIGNATED_INITIALIZER;

- (void)subscribeLevelChangedWithSubscribe:(void (^_Nullable)(NSInteger))subscribe;
- (void)subscribeLevelProgressChangedWithSubscribe:(void (^_Nullable)(double))subscribe;


@property (NS_NONATOMIC_IOSONLY, getter = getLevel, readonly) NSInteger level;
@property (NS_NONATOMIC_IOSONLY, getter = getLevelProgress, readonly) double levelProgress;

@end

NS_ASSUME_NONNULL_END
