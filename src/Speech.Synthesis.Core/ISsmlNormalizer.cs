namespace Speech.Synthesis.Core
{
    using System;

    public interface ISsmlNormalizer<TArg>
    {
        event EventHandler<TArg> OnNormalized;
        string Normalize(string ssmlText);
    }
}
