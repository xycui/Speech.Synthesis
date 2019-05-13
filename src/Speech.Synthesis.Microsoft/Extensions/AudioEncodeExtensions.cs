namespace Speech.Synthesis.Microsoft.Extensions
{
    using System.Collections.Generic;
    using Microsoft;

    public static class AudioEncodeExtensions
    {
        public static readonly IReadOnlyDictionary<AudioEncode, string> EncodeMapping =
            new Dictionary<AudioEncode, string>
            {
                {AudioEncode.RawPcmMono16K16B, "raw-16khz-16bit-mono-pcm"},
                {AudioEncode.RawPcmMono8K8B, "raw-8khz-8bit-mono-mulaw"},
                {AudioEncode.Mp3Mono16K128Br, "audio-16khz-128kbitrate-mono-mp3"},
                {AudioEncode.Mp3Mono16K64Br, "audio-16khz-64kbitrate-mono-mp3"},
                {AudioEncode.Mp3Mono16K32Br, "audio-16khz-32kbitrate-mono-mp3"},
                {AudioEncode.RawSilkMono16K16B, "raw-16khz-16bit-mono-truesilk"},
                {AudioEncode.RiffPcmMono8K8B, "riff-8khz-8bit-mono-mulaw"},
                {AudioEncode.RiffPcmMono16K16B, "riff-16khz-16bit-mono-pcm"},
                {AudioEncode.RawPcmMono24K16B, "raw-24khz-16bit-mono-pcm"},
                {AudioEncode.RiffPcmMono24K16B, "riff-24khz-16bit-mono-pcm"},
                {AudioEncode.Mp3Mono24K160Br, "audio-24khz-160kbitrate-mono-mp3"},
                {AudioEncode.Mp3Mono24K96Br, "audio-24khz-96kbitrate-mono-mp3"},
                {AudioEncode.Mp3Mono24K48Br, "audio-24khz-48kbitrate-mono-mp3"},
            };

        public static string GetEncodeName(this AudioEncode encode)
        {
            EncodeMapping.TryGetValue(encode, out string encodeString);

            return encodeString;
        }
    }
}
