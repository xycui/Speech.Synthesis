namespace Speech.Synthesis.Core.DynamicPath
{
    using Params;
    using System;
    using System.Collections.Generic;

    public interface IPathProvider<in TParam> where TParam : SynthesisParams
    {
        Uri GetSynthesisUri(string text, TParam param);
        Uri GetSynthesisUri(string ssml);
    }
}
