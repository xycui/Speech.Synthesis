namespace Speech.Synthesis.Microsoft.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class RegionExtensions
    {
        private static readonly IReadOnlyDictionary<Region, Uri> RegionUriMapping = new Dictionary<Region, Uri>
        {
            {
                Region.WestUS,
                new Uri("https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.WestUS2,
                new Uri("https://westus2.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.EastUS,
                new Uri("https://eastus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.EastUS2,
                new Uri("https://eastus2.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.EastAsia,
                new Uri("https://eastasia.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.SouthEastAsia,
                new Uri("https://southeastasia.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.NorthEurope,
                new Uri("https://northeurope.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            },
            {
                Region.WestEurope,
                new Uri("https://westeurope.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")
            }
        };

        public static Uri GetIssueTokenUri(this Region region)
        {
            return RegionUriMapping[region];
        }
    }
}
