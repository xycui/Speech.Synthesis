namespace Speech.Synthesis.Tests
{
    using Core.Params;
    using Microsoft;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class SynthesisTest
    {
        private const string SubscriptionKey = "";
        private const string SynthesisUri = "";
        private const string VoiceName = "";
        private readonly DirectoryInfo _outputDirInfo = new DirectoryInfo("output");

        private readonly ITestOutputHelper _output;

        public SynthesisTest(ITestOutputHelper output)
        {
            _output = output;
            if (_outputDirInfo.Exists)
            {
                return;
            }

            _outputDirInfo.Create();
            _outputDirInfo.Refresh();
        }

        [SkipFact(Params = new[] { SubscriptionKey, SynthesisUri, VoiceName })]
        public void TestCongnitiveSpeechSynthesis()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer(new Uri(SynthesisUri), SubscriptionKey);
            Stopwatch stopWatch = Stopwatch.StartNew();
            System.IO.Stream stream = synthesizer.GetSynthesizedData("ÄãºÃÄãºÃ¡£", new SynthesisParams { Language = "zh-CN", VoiceFont = VoiceName },
                AudioEncode.Mp3Mono16K128Br);
            _output.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);
#if DEBUG
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(new FileStream(Path.Combine(_outputDirInfo.FullName, "ttsresult.mp3"), FileMode.Create));
#endif
        }
    }

    internal class SkipFactAttribute : FactAttribute
    {
        public virtual string[] Params { get; set; }

        public override string Skip => Params.Any(string.IsNullOrEmpty) ? "Parameter not valid" : null;
    }
}
