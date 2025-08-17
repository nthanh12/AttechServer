using System.ComponentModel.DataAnnotations;

namespace AttechServer.Shared.ApplicationBase.Common.Validations
{
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;
        private readonly string _relationType;

        public FileValidationAttribute(string relationType = "any", long maxFileSizeMB = 10, params string[] allowedExtensions)
        {
            _relationType = relationType.ToLower();
            _maxFileSize = maxFileSizeMB * 1024 * 1024; // Convert MB to bytes
            _allowedExtensions = allowedExtensions?.Select(ext => ext.ToLower()).ToArray() ?? Array.Empty<string>();
        }

        public override bool IsValid(object? value)
        {
            if (value is IFormFile file)
            {
                return ValidateFile(file);
            }
            else if (value is List<IFormFile> files)
            {
                return files.All(ValidateFile);
            }
            else if (value is IFormFile[] fileArray)
            {
                return fileArray.All(ValidateFile);
            }

            return true; // If no files, validation passes
        }

        private bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return true; // Allow empty files (will be handled elsewhere)

            // Check file size
            if (file.Length > _maxFileSize)
            {
                ErrorMessage = $"Kích thước file '{file.FileName}' vượt quá {_maxFileSize / (1024 * 1024)}MB";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLower();

            // Check allowed extensions if specified
            if (_allowedExtensions.Length > 0 && !_allowedExtensions.Contains(extension))
            {
                ErrorMessage = $"File '{file.FileName}' không được hỗ trợ. Chỉ chấp nhận: {string.Join(", ", _allowedExtensions)}";
                return false;
            }

            // Check relation type specific rules
            if (_relationType == "image")
            {
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
                if (!imageExtensions.Contains(extension))
                {
                    ErrorMessage = $"File '{file.FileName}' không phải là ảnh hợp lệ";
                    return false;
                }
            }
            else if (_relationType == "document")
            {
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
                if (imageExtensions.Contains(extension))
                {
                    ErrorMessage = $"File '{file.FileName}' là ảnh, không thể upload như document";
                    return false;
                }
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"File validation failed for {name}";
        }
    }

    // Convenient preset attributes
    public class ImageFileValidationAttribute : FileValidationAttribute
    {
        public ImageFileValidationAttribute(long maxFileSizeMB = 5) 
            : base("image", maxFileSizeMB, ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp")
        {
        }
    }

    public class DocumentFileValidationAttribute : FileValidationAttribute
    {
        public DocumentFileValidationAttribute(long maxFileSizeMB = 20) 
            : base("document", maxFileSizeMB, ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar")
        {
        }
    }

    public class AttachmentFileValidationAttribute : FileValidationAttribute
    {
        public AttachmentFileValidationAttribute(long maxFileSizeMB = 20) 
            : base("any", maxFileSizeMB)
        {
        }
    }
}
