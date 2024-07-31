
namespace BankingControlPanel.Domain.Heplers
{
    public static class MediaFileService
    {

        public static async Task<UploadedMediaFileInfoDto> UploadMediaFileAsync(IFormFile file, string folderName)
        {
            UploadedMediaFileInfoDto uploadedMediaFile = new();
            try
            {
                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = Path.GetRandomFileName().Replace(".", "") + "" + extension;
                    var originalFileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);
                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                    uploadedMediaFile.OriginalFileName = originalFileName;
                    uploadedMediaFile.FileName = fileName;

                }
            }
            catch (Exception ex)
            {

            }
            return uploadedMediaFile;
        }

        public static async Task<UploadedMediaFileInfoDto> UploadMediaFileBase64Async(string base64, string extension, string folderName)
        {
            UploadedMediaFileInfoDto uploadedMediaFile = new();
            try
            {
                if (!string.IsNullOrEmpty(base64))
                {
                    var fileName = Path.GetRandomFileName().Replace(".", "") + "" + extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);
                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        using (var msrteam = new MemoryStream(Convert.FromBase64String(base64)))
                        {
                            await msrteam.CopyToAsync(fileSrteam);
                        }
                    }
                    uploadedMediaFile.OriginalFileName = fileName;
                    uploadedMediaFile.FileName = fileName;

                }
            }
            catch (Exception ex)
            {

            }
            return uploadedMediaFile;
        }

        public static void DeleteMediaFile(string fileName, string folderName)
        {
            var filePath = Path.GetFullPath(folderName + fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
    public class UploadedMediaFileInfoDto
    {
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
    }
}
