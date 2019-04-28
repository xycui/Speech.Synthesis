namespace Speech.Synthesis.Core
{
    using System;

    public interface ISsmlNormalizer<TInt>
    {
        string Normalize(string ssmlText, out TInt intData);
    }
}
