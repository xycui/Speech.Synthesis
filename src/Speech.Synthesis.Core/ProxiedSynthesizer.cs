namespace Speech.Synthesis.Core
{
    using Params;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ProxiedSynthesizer<TParam, TEncode> : ISpeechSynthesizer<TParam, TEncode> where TParam : SynthesisParams
    {
        private readonly ISpeechSynthesizer<TParam, TEncode> _innerSynthesizer;

        protected ProxiedSynthesizer(ISpeechSynthesizer<TParam, TEncode> innerSynthesizer)
        {
            _innerSynthesizer = innerSynthesizer;
        }

        public virtual Stream GetSynthesizedData(string text, TParam synthesizeParams, TEncode outputEncode)
        {
            return _innerSynthesizer.GetSynthesizedData(text, synthesizeParams, outputEncode);
        }

        public virtual Stream GetSynthesizedData(string ssml, TEncode outputEncode)
        {
            return _innerSynthesizer.GetSynthesizedData(ssml, outputEncode);
        }

        public virtual Task<Stream> GetSynthesizedDataAsync(string text, TParam synthesizeParams, TEncode outputEncode, CancellationToken token)
        {
            return _innerSynthesizer.GetSynthesizedDataAsync(text, synthesizeParams, outputEncode, token);
        }

        public virtual Task<Stream> GetSynthesizedDataAsync(string ssml, TEncode outputEncode, CancellationToken token)
        {
            return _innerSynthesizer.GetSynthesizedDataAsync(ssml, outputEncode, token);
        }
    }
}
