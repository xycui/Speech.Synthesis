namespace Speech.Synthesis.Core
{
    using Params;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DynamicPathSynthesizer<TParam, TEncode> : ISpeechSynthesizer<TParam, TEncode> where TParam : SynthesisParams
    {
        private readonly ISsmlConverter<TParam> _ssmlConverter;

        protected DynamicPathSynthesizer() : this(SimpleSsmlConverter.Instance)
        {
        }

        protected DynamicPathSynthesizer(ISsmlConverter<TParam> ssmlConverter)
        {
            _ssmlConverter = ssmlConverter;
        }

        public Stream GetSynthesizedData(string text, TParam synthesizeParams, TEncode outputEncode)
        {
            return Task.Run(async () =>
                await GetSynthesizedDataAsync(text, synthesizeParams, outputEncode, CancellationToken.None)).Result;
        }

        public Stream GetSynthesizedData(string ssml, TEncode outputEncode)
        {
            return Task.Run(async () => await GetSynthesizedDataAsync(ssml, outputEncode, CancellationToken.None))
                .Result;
        }

        public Task<Stream> GetSynthesizedDataAsync(string text, TParam synthesizeParams, TEncode outputEncode, CancellationToken token)
        {
            Uri host = null;
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return InternalSynthesisAsync(ssml, host, outputEncode, token);
        }

        public Task<Stream> GetSynthesizedDataAsync(string ssml, TEncode outputEncode, CancellationToken token)
        {
            Uri host = null;
            return InternalSynthesisAsync(ssml, host, outputEncode, token);
        }

        protected abstract Task<Stream> InternalSynthesisAsync(string ssml, Uri targetEp, TEncode encode,
            CancellationToken token);
    }
}
