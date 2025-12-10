using FileKEY;

namespace TestProject
{
    public class DesktopUnitTests : IDisposable
    {
        string imageCatFileTypeValue = "PNG(png);";
        string imageCatFileCrcKey = "27E9D872";
        string imageCatFileMd5Key = "d9ca43935c9663dccecf8c0951cf1ec2";
        string imageCatFileSha256Key = "de566fbbf3033a30bb72cfd068b932d335d04b56212c78bd8fd87a7c8804819f";

        string imageDogFileTypeValue = "JPEG(jpg);";
        string imageDogFileSha256Key = "4a13733b1f6b9e0a16a58aef2fe54db59154eb335da590ef1894eac8c1e16628";

        StringWriter writer;
        public DesktopUnitTests()
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
                Path.Combine("TestFile", "image-cat.png")
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(imageCatFileTypeValue, output);
            Assert.Contains(imageCatFileCrcKey, output);
            Assert.Contains(imageCatFileMd5Key, output);
            Assert.Contains(imageCatFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFileAndKeyMatched()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                imageCatFileSha256Key
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageCatFileSha256Key), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFileAndKeyMatched_0s()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                imageCatFileSha256Key,
                "-0s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString().Trim();

            Assert.Equal(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageCatFileSha256Key), output);

        }

        [Fact]
        public async Task InFileAndFileKeysMatched_0s()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "imageKeys.txt",
                "-0s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString().Trim();

            Assert.Equal(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, "sha256", imageCatFileSha256Key, 9, 8), output);

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
            Assert.Contains(imageCatFileTypeValue, output);
            Assert.Contains(imageDogFileTypeValue, output);
            Assert.Contains(imageCatFileCrcKey, output);
            Assert.Contains(imageCatFileMd5Key, output);
            Assert.Contains(imageCatFileSha256Key, output);
            Assert.Contains(imageDogFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);
            
        }

    }
}