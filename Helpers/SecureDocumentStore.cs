using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// NEW: guest ID proofs and guest photos are personal data, so they are saved OUTSIDE wwwroot
    /// (ContentRoot/SecureUploads/{id|guestphoto}) and can only be read through the authorized
    /// GET api/documents/guest endpoint. Files saved earlier under wwwroot/uploads/{id|guestphoto}
    /// are still readable through the same endpoint, but no longer as public static files.
    /// Stored path format (DB): /secure/id/{guid}.jpg   (legacy: /uploads/id/{guid}.jpg)
    /// </summary>
    public static class SecureDocumentStore
    {
        /// <summary>Set once in Program.cs (ContentRoot/SecureUploads).</summary>
        public static string Root { get; set; } =
            Path.Combine(Directory.GetCurrentDirectory(), "SecureUploads");

        /// <summary>Legacy public folder (wwwroot/uploads), set in Program.cs.</summary>
        public static string LegacyRoot { get; set; } =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        private static readonly string[] Kinds = { "id", "guestphoto" };

        private static readonly Regex PathRule = new(
            @"^/(secure|uploads)/(id|guestphoto)/[0-9a-fA-F\-]{36}\.(jpg|jpeg|png|webp|pdf)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string Save(IFormFile file, string kind, bool allowPdf)
        {
            if (Array.IndexOf(Kinds, kind) < 0)
                throw new InvalidOperationException("Invalid document type.");

            string ext = UploadValidator.SafeExtension(file, allowPdf);
            string folder = Path.Combine(Root, kind);
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + ext;
            using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.CreateNew))
            {
                file.CopyTo(stream);
            }

            return "/secure/" + kind + "/" + fileName;
        }

        /// <summary>
        /// Maps a stored path to the physical file. Returns null for anything that is not exactly
        /// a stored guest document path (no "..", no other folders, no other extensions).
        /// </summary>
        public static string? Resolve(string? storedPath, out string contentType)
        {
            contentType = "application/octet-stream";
            if (string.IsNullOrWhiteSpace(storedPath)) return null;

            var m = PathRule.Match(storedPath.Trim());
            if (!m.Success) return null;

            string area = m.Groups[1].Value;
            string kind = m.Groups[2].Value;
            string ext = m.Groups[3].Value.ToLowerInvariant();
            string fileName = Path.GetFileName(storedPath.Trim());

            string baseDir = area == "secure" ? Root : LegacyRoot;
            string full = Path.GetFullPath(Path.Combine(baseDir, kind, fileName));
            string allowedDir = Path.GetFullPath(Path.Combine(baseDir, kind)) + Path.DirectorySeparatorChar;

            if (!full.StartsWith(allowedDir, StringComparison.Ordinal) || !File.Exists(full))
                return null;

            contentType = ext switch
            {
                "pdf" => "application/pdf",
                "png" => "image/png",
                "webp" => "image/webp",
                _ => "image/jpeg"
            };
            return full;
        }
    }
}
