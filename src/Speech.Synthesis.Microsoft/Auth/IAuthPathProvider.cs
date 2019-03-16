namespace Speech.Synthesis.Microsoft.Auth
{
    using Core.DynamicPath;
    using Core.Params;
    using System;

    public interface IAuthPathProvider<in TParam> : IPathProvider<TParam> where TParam : SynthesisParams
    {
        string GetSubscriptionId(Uri uri);
    }
}
