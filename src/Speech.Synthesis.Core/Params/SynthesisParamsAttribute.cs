namespace Speech.Synthesis.Core.Params
{
    using System;

    public class SynthesisParamAttribute : Attribute
    {
        public SynthesisParamAttribute(int order, object defaultValue)
        {
            Order = order;
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; }
        public int Order { get; private set; }
    }
}
