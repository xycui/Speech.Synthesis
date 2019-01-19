namespace Speech.Synthesis.Core
{
    using Params;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISpeechSynthesizer<in TParam, in TEncode> where TParam : SynthesisParams
    {
        Stream GetSynthesizedData(string text, TParam synthesizeParams, TEncode outputEncode);
        Stream GetSynthesizedData(string ssml, TEncode outputEncode);
        Task<Stream> GetSynthesizedDataAsync(string text, TParam synthesizeParams, TEncode outputEncode, CancellationToken token);
        Task<Stream> GetSynthesizedDataAsync(string ssml, TEncode outputEncode, CancellationToken token);
    }
}
