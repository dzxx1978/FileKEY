using FileKEY;

namespace TestProject
{
    public class DesktopUnitTests : IDisposable
    {
        private string imageCatFileTypeValue => "PNG(png);";
        private string imageCatFileCrcKey => "27E9D872";
        private string imageCatFileMd5Key => "d9ca43935c9663dccecf8c0951cf1ec2";
        private string imageCatFileSha256Key => "de566fbbf3033a30bb72cfd068b932d335d04b56212c78bd8fd87a7c8804819f";

        private string imageDogFileTypeValue => "JPEG(jpg);";
        private string imageDogFileSha256Key => "4a13733b1f6b9e0a16a58aef2fe54db59154eb335da590ef1894eac8c1e16628";

        private string testFileDir => "TestFile";
        private string imageDogPath => Path.Combine(testFileDir, "image-dog.jpg");
        private string imageCatPath => Path.Combine(testFileDir, "image-cat.png");

        StringWriter writer;
        public DesktopUnitTests()
        {
            writer = new StringWriter();
            Console.SetOut(writer);
        }

        public void Dispose()
        {
            Console.SetOut(Console.Out);
            writer.Dispose();
        }

        private async Task<string> TestRunAsync(string[] args)
        {
            var output = string.Empty;
            try
            {
                AppOption.SetOptions(args);
                await new Desktop().GanHuoer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                output = writer.ToString();
            }
            return output;
        }

        [Fact]
        public async Task HelpShownAndExit()
        {
            var args = new List<string>
            {
                AppOption.Command_v
            };

            var output = await TestRunAsync(args.ToArray());

            Assert.Equal(Language.GetHelpShown(), output);

        }

        [Fact]
        public void LoadTestLanguage()
        {
            var languageFile = ConfigFile.GetConfigFilePath(ConfigFile.ConfigType.Language, "test");
            var testLanguageFile = Path.GetFileName(languageFile);

            if (!File.Exists(languageFile) && File.Exists(testLanguageFile))
                File.Copy(testLanguageFile, languageFile);

            Assert.True(File.Exists(testLanguageFile));
            Assert.True(File.Exists(languageFile));

            File.Delete(testLanguageFile);
            Assert.False(File.Exists(testLanguageFile));

            var args = new List<string>
            {
                AppOption.Command_Language,
                "test"
            };
            AppOption.SetOptions(args.ToArray());

            Assert.Equal("*Test end*", Language.GetMessage(Language.MessageKey.End));
            Assert.False(string.IsNullOrEmpty(Language.GetMessage(Language.MessageKey.Set)));

            File.Delete(languageFile);
            Assert.False(File.Exists(languageFile));

        }

        [Fact]
        public async Task InFile_t()
        {

            var args = new List<string>
            {
                imageCatPath,
                AppOption.Command_t
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                AppOption.Command_c
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                AppOption.Command_m
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                AppOption.Command_s
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                "-0s"
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                "-0cms"
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath
            };

            AppOption.SetOptions(args.ToArray());

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
                imageCatPath,
                imageCatFileSha256Key
            };

            AppOption.SetOptions(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.Contains(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, Language.Sha256, imageCatFileSha256Key), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InFileAndKeyMatched_0s()
        {

            var args = new List<string>
            {
                imageCatPath,
                imageCatFileSha256Key,
                "-0s"
            };

            AppOption.SetOptions(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString().Trim();

            Assert.Equal(Language.GetMessage(Language.MessageKey.Matched, Language.Sha256, imageCatFileSha256Key), output);

        }

        [Fact]
        public async Task InFileAndFileKeysMatched_0s()
        {

            var args = new List<string>
            {
                imageCatPath,
                "imageKeys.txt",
                "-0s"
            };

            AppOption.SetOptions(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString().Trim();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Equal(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, Language.Sha256, imageCatFileSha256Key, 9, 8), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPath()
        {

            var args = new List<string>
            {
                testFileDir
            };

            AppOption.SetOptions(args.ToArray());

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
                testFileDir,
                 imageDogFileSha256Key,
                "-0s"
           };

            AppOption.SetOptions(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, Language.Sha256, imageDogFileSha256Key), output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Path.GetFullPath(imageDogPath), output);
            Assert.DoesNotContain(Path.GetFileName(imageCatPath), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPathAndCatKey_0s()
        {
            var args = new List<string>
            {
                testFileDir,
                 imageCatFileSha256Key,
                "-0s"
           };

            AppOption.SetOptions(args.ToArray());

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, Language.Sha256, imageCatFileSha256Key), output);
            Assert.DoesNotContain(imageDogFileSha256Key, output);
            Assert.Contains(Path.GetFullPath(imageCatPath), output);
            Assert.DoesNotContain(Path.GetFileName(imageDogPath), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPathAndFileKeys_0s()
        {

            var args = new List<string>
            {
                testFileDir,
                "imageKeys.txt",
                "-0s"
            };

            AppOption.SetOptions(args.ToArray());
            var outOptions= AppOption.GetOptions();
            Assert.True(outOptions.Count() == 6);

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, Language.Sha256, imageCatFileSha256Key, 9, 8), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.MatchedInKeysFile, Language.Sha256, imageDogFileSha256Key, 17, 8), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPathAndEqualsDogFile_0s()
        {

            var args = new List<string>
            {
                AppOption.Command_Directory,
                testFileDir,
                AppOption.Command_Equals,
                imageDogPath,
                "-0s"
            };

            AppOption.SetOptions(args.ToArray());
            var outOptions = AppOption.GetOptions();
            Assert.True(outOptions.Count() == 6);

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.Contains(Language.GetMessage(Language.MessageKey.Matched, Language.Sha256, imageDogFileSha256Key), output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.Contains(Path.GetFullPath(imageDogPath), output);
            Assert.DoesNotContain(Path.GetFileName(imageCatPath), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }

        [Fact]
        public async Task InPathAndGroupByType()
        {

            var args = new List<string>
            {
                AppOption.Command_Directory,
                testFileDir,
                AppOption.Command_SubDirectory,
                "1",
                AppOption.Command_GroupBy,
                "type",
                AppOption.Command_GroupMinCount,
                "3"
            };

            AppOption.SetOptions(args.ToArray());
            var outOptions = AppOption.GetOptions();
            Assert.True(outOptions.Count() == 9);

            await new Desktop().GanHuoer();

            var output = writer.ToString();

            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.ProcessCompleted), output);
            Assert.DoesNotContain(imageDogFileSha256Key, output);
            Assert.DoesNotContain(imageCatFileSha256Key, output);
            Assert.DoesNotContain("JPEG(jpg);  (2)", output);
            Assert.Contains("PNG(png);  (4)", output);
            Assert.DoesNotContain(Path.GetFullPath(imageDogPath), output);
            Assert.Contains(Path.GetFullPath(imageCatPath), output);
            Assert.DoesNotContain(Language.GetMessage(Language.MessageKey.End), output);

        }
    }
}