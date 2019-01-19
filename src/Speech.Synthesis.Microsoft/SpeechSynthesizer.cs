﻿namespace Speech.Synthesis.Microsoft
{
    using Core;
    using Core.Params;
    using Extensions;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class SpeechSynthesizer<TParam> : ISpeechSynthesizer<TParam, AudioEncode> where TParam : SynthesisParams
    {
        private readonly ISsmlConverter<TParam> _ssmlConverter;
        private readonly Uri _synthesizeUri;
        protected readonly HttpClient HttpClient;

        protected SpeechSynthesizer(Uri synthesizeUri, ISsmlConverter<TParam> ssmlConverter)
        {
            _synthesizeUri = synthesizeUri;
            _ssmlConverter = ssmlConverter;
            HttpClient = new HttpClient();
        }

        protected SpeechSynthesizer(Uri synthesizeUri, ISsmlConverter<TParam> ssmlConverter, string subscriptionKey, Region tokenIssueRegion = Region.EastAsia)
        {
            _synthesizeUri = synthesizeUri;
            _ssmlConverter = ssmlConverter;
            HttpClient =
                new HttpClient(
                    new BearerJwtTokenHandler(subscriptionKey, tokenIssueRegion)
                    {
                        InnerHandler = new HttpClientHandler()
                    });
        }


        public Stream GetSynthesizedData(string text, TParam synthesizeParams, AudioEncode outputEncode)
        {
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return GetSynthesizedData(ssml, outputEncode);
        }

        public Stream GetSynthesizedData(string ssml, AudioEncode outputEncode)
        {
            return Task.Run(() => GetSynthesizedDataAsync(ssml, outputEncode, CancellationToken.None)).Result;
        }

        public Task<Stream> GetSynthesizedDataAsync(string text, TParam synthesizeParams, AudioEncode outputEncode, CancellationToken token)
        {
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return GetSynthesizedDataAsync(ssml, outputEncode, token);
        }

        public async Task<Stream> GetSynthesizedDataAsync(string ssml, AudioEncode outputEncode, CancellationToken token)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _synthesizeUri)
            {
                Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml")
            };
            request.Headers.Add("Connection", "Keep-Alive");
            request.Headers.Add("X-Microsoft-OutputFormat", outputEncode.GetEncodeName());
            request.Headers.Add("X-FD-ClientID", "RadioStationService");
            request.Headers.Add("X-FD-ImpressionGUID", Guid.NewGuid().ToString());

            HttpResponseMessage response = await HttpClient.SendAsync(request, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception(
                    $"Synthesize fail with status code: {response.StatusCode.ToString()}, error message: [{errorMsg}]");
            }
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return stream;
        }
    }

    public class SpeechSynthesizer : SpeechSynthesizer<SynthesisParams>
    {
        public SpeechSynthesizer(Uri synthesizeUri) : base(synthesizeUri, SimpleSsmlConverter.Instance)
        {
        }

        public SpeechSynthesizer(Uri synthesizeUri, string subscriptionKey, Region region = Region.EastAsia) : base(
            synthesizeUri, SimpleSsmlConverter.Instance, subscriptionKey, region)
        {
        }

        public SpeechSynthesizer(Uri synthesizeUri, ISsmlConverter<SynthesisParams> customConverter) : base(
            synthesizeUri, customConverter)
        {
        }

        public SpeechSynthesizer(Uri synthesizeUri, ISsmlConverter<SynthesisParams> customConverter,
            string subscriptionKey, Region region = Region.EastAsia) : base(synthesizeUri, customConverter,
            subscriptionKey, region)
        {
        }
    }
}
