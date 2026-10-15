using Microsoft.AspNetCore.Http;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// Uploaded files are saved under wwwroot/uploads and served as public static files,
    /// so the file type must never be taken on trust from the client's file name
    /// (a ".html" / ".svg" / ".aspx" upload would run in the browser or on the server).
    /// </summary>
    public static class UploadValidator
    {
        private const long MaxBytes = 10 * 1024 * 1024;   // 10 MB

        private static readonly HashSet<string> ImageExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

        private static readonly HashSet<string> ImageOrPdfExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

        /// <summary>Returns the safe lower-case extension, or throws a clear message.</summary>
        public static string SafeExtension(IFormFile file, bool allowPdf = false)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("The uploaded file is empty.");

            if (file.Length > MaxBytes)
                throw new InvalidOperationException("File is too large. Maximum allowed size is 10 MB.");

            var ext = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
            var allowed = allowPdf ? ImageOrPdfExt : ImageExt;

            if (!allowed.Contains(ext))
                throw new InvalidOperationException(
                    allowPdf
                        ? "Only JPG, PNG, WEBP or PDF files are allowed."
                        : "Only JPG, PNG or WEBP images are allowed.");

            return ext;
        }
    }
}
