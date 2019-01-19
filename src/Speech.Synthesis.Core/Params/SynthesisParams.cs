namespace Speech.Synthesis.Core.Params
{
    using System.Linq;
    using System.Reflection;

    public class SynthesisParams
    {
        private const float MinRate = 0.001f;
        private float _rate;

        public SynthesisParams()
        {
            foreach (PropertyInfo prop in typeof(SynthesisParams).GetTypeInfo().DeclaredProperties)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(SynthesisParamAttribute), false);
                MethodInfo setter = prop.SetMethod;
                if (!attrs.Any() || setter == null)
                {
                    continue;
                }

                if (attrs.First() is SynthesisParamAttribute attr)
                {
                    setter.Invoke(this, new[] { attr.DefaultValue });
                }
            }
        }

        public SynthesisParams(SynthesisParams param)
        {
            VoiceFont = param.VoiceFont;
            Rate = param.Rate;
            Emotion = param.Emotion;
            Gender = param.Gender;
            Language = param.Language;
        }

        [SynthesisParam(0, null)]
        public string VoiceFont { get; set; }

        [SynthesisParam(1, 1.0f)]
        public float Rate
        {
            get => _rate;
            set => _rate = value < MinRate ? MinRate : value;
        }

        public string RateString
        {
            get
            {
                float div = Rate - 1;
                return $"{(div >= 0 ? "+" : string.Empty)}{div.ToString("P").Replace(" ", "")}";
            }
        }

        [SynthesisParam(2, Gender.Female)]
        public Gender Gender { get; set; }

        [SynthesisParam(3, "zh-CN")]
        public string Language { get; set; }
        [SynthesisParam(4, Emotion.Neutral)]
        public Emotion Emotion { get; set; }
        [SynthesisParam(5, EchoScene.Normal)]
        public EchoScene EchoScene { get; set; }
    }
}
