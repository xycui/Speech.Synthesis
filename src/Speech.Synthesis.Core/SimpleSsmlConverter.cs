namespace Speech.Synthesis.Core
{
    using Params;
    using System;
    using System.Linq;
    using System.Security;
    using System.Xml;

    public class SimpleSsmlConverter : ISsmlConverter<SynthesisParams>
    {
        private const string SsmlTemplate = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xml:lang=\"{3}\"><voice name=\"{0}\"><prosody rate=\"{1}\">{2}</prosody></voice></speak>";
        private static readonly Lazy<ISsmlConverter<SynthesisParams>> LazyInstace = new Lazy<ISsmlConverter<SynthesisParams>>(() => new SimpleSsmlConverter());
        public static ISsmlConverter<SynthesisParams> Instance => LazyInstace.Value;

        public string Convert(string source, SynthesisParams param)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            string validText = SecurityElement.Escape(source);
            string voiceFont = param.VoiceFont;
            string rateString = string.IsNullOrEmpty(param.RateString) ? "+00.00%" : param.RateString;

            return string.Format(SsmlTemplate, voiceFont, rateString, validText, param.Language);
        }
    }
}
