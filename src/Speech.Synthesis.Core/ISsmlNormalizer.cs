namespace Speech.Synthesis.Core
{
    /// <summary>
    /// It will normalize ssml and generate intermediate data
    /// </summary>
    /// <typeparam name="TInt">intermediate data type</typeparam>
    public interface ISsmlNormalizer<TInt>
    {
        /// <summary>
        /// Normalize method for the ssml text and generate the intermedia data.
        /// </summary>
        /// <param name="ssmlText">input ssml text</param>
        /// <param name="intData">intermediate data</param>
        /// <returns>processed ssml</returns>
        string Normalize(string ssmlText, out TInt intData);
    }
}
