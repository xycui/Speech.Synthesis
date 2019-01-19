namespace Speech.Synthesis.Core
{
    using Params;
    public interface ISsmlConverter<in TParam> where TParam : SynthesisParams
    {
        string Convert(string rawText, TParam param);
    }
}
