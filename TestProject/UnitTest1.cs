using FileKEY;

namespace TestProject
{
    public class UnitTest1 : IDisposable
    {
        string imageFileTypeValue = "PNG(png);";
        string imageFileCrcKey = "27E9D872";
        string imageFileMd5Key = "d9ca43935c9663dccecf8c0951cf1ec2";
        string imageFileSha256Key = "de566fbbf3033a30bb72cfd068b932d335d04b56212c78bd8fd87a7c8804819f";

        StringWriter writer;
        public UnitTest1()
        {
            Language.Initialize();

            writer = new StringWriter();
            Console.SetOut(writer);
        }

        public void Dispose()
        {
            Console.SetOut(Console.Out);
            writer.Dispose();
        }

        [Fact]
        public async Task HelpShownAndExit()
        {
            var args = new List<string>
            {
                "-v"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = Language.GetHelpShown();

            Assert.Equal(writer.ToString(), output);

        }

        [Fact]
        public async Task InFile()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image.png")
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(imageFileTypeValue, output);
            Assert.Contains(imageFileCrcKey, output);
            Assert.Contains(imageFileMd5Key, output);
            Assert.Contains(imageFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFileAndKeyMatched()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image.png"),
                imageFileSha256Key
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageFileSha256Key), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFileAndKeyMatched_0s()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image.png"),
                imageFileSha256Key,
                "-0s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString().Trim();

            Assert.Equal(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageFileSha256Key), output);

        }

        [Fact]
        public async Task InPath()
        {

            var args = new List<string>
            {
                "TestFile"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(imageFileTypeValue, output);
            Assert.Contains(imageFileCrcKey, output);
            Assert.Contains(imageFileMd5Key, output);
            Assert.Contains(imageFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

    }
}