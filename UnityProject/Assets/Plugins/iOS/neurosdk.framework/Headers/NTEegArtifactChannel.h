//
//  NTEegArtifactChannel.h
//  neurosdk
//
//  Created by admin on 24.12.2019.
//

#import "NTBaseChannel.h"

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSUInteger, NTArtifactType) {
    NTArtifactTypeTypeNoise = 0,
    NTArtifactTypeTypeBlink = 1,
    NTArtifactTypeTypeBrux  = 2,
    NTArtifactTypeTypeNone  = 3,
};
typedef NS_ENUM (NSUInteger, NTSourceChannel) {
    NTSourceChannelT3 = 0b00000001,
    NTSourceChannelT4 = 0b00000010,
    NTSourceChannelO1 = 0b00000100,
    NTSourceChannelO2 = 0b00001000
};

@interface NTArtifactZone : NSObject
@property (nonatomic, readonly) double time;
@property (nonatomic, readonly) double duration;
@property (nonatomic, readonly) enum NTArtifactType type;
@property (nonatomic, readonly) enum NTSourceChannel source_channels_flags;
@end

@interface NTEegArtifactChannel : NTBaseChannel

- (nullable instancetype)initWithT3:(NTBaseChannel *_Nullable)t3 t4:(NTBaseChannel *_Nullable)t3 o1:(NTBaseChannel *_Nullable)o1 o2:(NTBaseChannel *_Nullable)o2 NS_DESIGNATED_INITIALIZER;

/// Read array of NTArtifactZone from eeg artifact channel
/// @param offset Offset from first received value
/// @param length Size of chunk that you will read
- (NSArray<NTArtifactZone *> *)readDataWithOffset:(NSUInteger)offset length:(NSUInteger) length  NS_SWIFT_NAME(readData(offset:length:));
@end

NS_ASSUME_NONNULL_END
