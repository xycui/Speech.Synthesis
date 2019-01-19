namespace Speech.Synthesis.Microsoft.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class RegionExtensions
    {
        // https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/rest-apis
        private static readonly IReadOnlyDictionary<Region, Uri> RegionUriMapping = new Dictionary<Region, Uri>
        {
            {
                Region.WestUS,
                new Uri("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.WestUS2,
                new Uri("https://westus2.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.EastUS,
                new Uri("https://eastus.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.EastUS2,
                new Uri("https://eastus2.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.EastAsia,
                new Uri("https://eastasia.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.SouthEastAsia,
                new Uri("	https://southeastasia.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.NorthEurope,
                new Uri("https://northeurope.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            },
            {
                Region.WestEurope,
                new Uri("https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issueToken")
            }
        };

        public static Uri GetIssueTokenUri(this Region region)
        {
            return RegionUriMapping[region];
        }
    }
}
