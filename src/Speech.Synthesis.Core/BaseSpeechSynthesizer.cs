namespace Speech.Synthesis.Core
{
    using Params;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class BaseSpeechSynthesizer<TParam, TEncode> : ISpeechSynthesizer<TParam, TEncode>
        where TParam : SynthesisParams
    {
        private readonly ISsmlConverter<TParam> _ssmlConverter;

        protected BaseSpeechSynthesizer(ISsmlConverter<TParam> ssmlConverter)
        {
            _ssmlConverter = ssmlConverter;
        }

        public Stream GetSynthesizedData(string text, TParam synthesizeParams, TEncode outputEncode)
        {
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return GetSynthesizedData(ssml, outputEncode);
        }

        public abstract Stream GetSynthesizedData(string ssml, TEncode outputEncode);

        public Task<Stream> GetSynthesizedDataAsync(string text, TParam synthesizeParams, TEncode outputEncode, CancellationToken token)
        {
            string ssml = _ssmlConverter.Convert(text, synthesizeParams);
            return GetSynthesizedDataAsync(ssml, outputEncode, token);
        }

        public abstract Task<Stream> GetSynthesizedDataAsync(string ssml, TEncode outputEncode, CancellationToken token);
    }

    public abstract class BaseSpeechSynthesizer<TEncode> : BaseSpeechSynthesizer<SynthesisParams, TEncode>
    {
        protected BaseSpeechSynthesizer(ISsmlConverter<SynthesisParams> ssmlConverter) : base(ssmlConverter)
        {
        }
    }
}
