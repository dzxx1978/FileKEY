using FileKEY;
using System.Text;

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
            for(var i = 0; i < 10; i++)
            {
                if (File.Exists(Path.Combine("TestFile", "image-cat.png")))
                {
                    break;
                }

                Thread.Sleep(200);
            }

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
        public void LoadTestLanguage()
        {

            var args = new List<string>
            {
                "--Language",
                "test"
            };
            AppOption.parseCommandLineArgs(args.ToArray());

            Assert.Equal("*end*", Language.GetMessage(Language.MessageKey.End));

        }

        [Fact]
        public async Task InFile_t()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-t"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(imageCatFileTypeValue, output);
            Assert.DoesNotContain(imageCatFileCrcKey, output);
            Assert.DoesNotContain(imageCatFileMd5Key, output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFile_c()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-c"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageCatFileTypeValue, output);
            Assert.Contains(imageCatFileCrcKey, output);
            Assert.DoesNotContain(imageCatFileMd5Key, output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFile_m()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-m"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageCatFileTypeValue, output);
            Assert.DoesNotContain(imageCatFileCrcKey, output);
            Assert.Contains(imageCatFileMd5Key, output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFile_s()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageCatFileTypeValue, output);
            Assert.DoesNotContain(imageCatFileCrcKey, output);
            Assert.DoesNotContain(imageCatFileMd5Key, output);
            Assert.Contains(imageCatFileSha256Key, output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFile_0s()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-0s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageCatFileTypeValue, output);
            Assert.DoesNotContain(imageCatFileCrcKey, output);
            Assert.DoesNotContain(imageCatFileMd5Key, output);
            Assert.Contains(imageCatFileSha256Key, output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFile_0cms()
        {

            var args = new List<string>
            {
                Path.Combine("TestFile", "image-cat.png"),
                "-0cms"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageCatFileTypeValue, output);
            Assert.Contains(imageCatFileCrcKey, output);
            Assert.Contains(imageCatFileMd5Key, output);
            Assert.Contains(imageCatFileSha256Key, output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

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

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Equal(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, "sha256", imageCatFileSha256Key, 9, 8), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

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

        [Fact]
        public async Task InPathAndDogKey_0s()
        {

            var args = new List<string>
            {
                "TestFile",
                 imageDogFileSha256Key,
                "-0s"
           };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageDogFileSha256Key), output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Path.GetFullPath(Path.Combine("TestFile", "image-dog.jpg")), output);
            Assert.DoesNotContain("image-cat.png", output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPathAndCatKey_0s()
        {
            var args = new List<string>
            {
                "TestFile",
                 imageCatFileSha256Key,
                "-0s"
           };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, "sha256", imageCatFileSha256Key), output);
            Assert.DoesNotContain(imageDogFileSha256Key, output);
            Assert.Contains(Path.GetFullPath(Path.Combine("TestFile", "image-cat.png")), output);
            Assert.DoesNotContain("image-dog.jpg", output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);
        
        }

        [Fact]
        public async Task InPathAndFileKeys_0s()
        {

            var args = new List<string>
            {
                "TestFile",
                "imageKeys.txt",
                "-0s"
            };

            AppOption.parseCommandLineArgs(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, "sha256", imageCatFileSha256Key, 9, 8), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, "sha256", imageDogFileSha256Key, 17, 8), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }
    }
}