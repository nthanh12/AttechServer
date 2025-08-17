using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace AttechServer.Shared.ApplicationBase.Common
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Bước 1: Thường hóa và thay ký tự đặc biệt
            input = input.ToLowerInvariant()
                         .Replace('đ', 'd')
                         .Replace('Đ', 'd');

            // Bước 2: Loại bỏ dấu tiếng Việt
            var normalized = input.Normalize(NormalizationForm.FormD);
            var slug = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    slug.Append(c);
                }
            }

            // Bước 3: Loại bỏ ký tự không hợp lệ và chuẩn hóa dấu gạch ngang
            string result = Regex.Replace(slug.ToString(), @"[^a-z0-9]+", "-").Trim('-');

            return result;
        }
    }
}
