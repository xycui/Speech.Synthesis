namespace Speech.Synthesis.Core
{
    using DynamicPath;
    using Params;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DynamicPathSynthesizer<TParam, TEncode> : ISpeechSynthesizer<TParam, TEncode> where TParam : SynthesisParams
    {
        private readonly ISsmlConverter<TParam> _ssmlConverter;
        private readonly IPathProvider<TParam> _pathProvider;

        protected DynamicPathSynthesizer(IPathProvider<TParam> pathProvider) : this(pathProvider, SimpleSsmlConverter.Instance)
        {
        }

        protected DynamicPathSynthesizer(IPathProvider<TParam> pathProvider, ISsmlConverter<TParam> ssmlConverter)
        {
            _pathProvider = pathProvider;
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
            Uri host = _pathProvider.GetSynthesisUri(text, synthesizeParams);
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return InternalSynthesisAsync(ssml, host, outputEncode, token);
        }

        public Task<Stream> GetSynthesizedDataAsync(string ssml, TEncode outputEncode, CancellationToken token)
        {
            Uri host = _pathProvider.GetSynthesisUri(ssml);
            return InternalSynthesisAsync(ssml, host, outputEncode, token);
        }

        protected abstract Task<Stream> InternalSynthesisAsync(string ssml, Uri targetEp, TEncode encode,
            CancellationToken token);
    }
}
