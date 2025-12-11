namespace FileKEY
{
    public class ComparisonInfo
    {
        public bool IsCrc32Equal { get; set; } = false;
        public bool IsMd5Equal { get; set; } = false;
        public bool IsSha256Equal { get; set; } = false;

        public long InKeyColumn { get; set; } = 0;
        public long InKeyRow { get; set; } = 0;

        public string Message { get; set; } = "";

        public bool IsEqual => IsCrc32Equal || IsMd5Equal || IsSha256Equal;
    }
}