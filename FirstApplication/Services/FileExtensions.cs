using BookShop.Enum;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SixLabors.ImageSharp.Formats.Webp;
using System.IO;

namespace BookShop.Services
{
    public static class FileExtensions
    {

        public async static Task<string> SharpSaveFile(
            string sourcePath ,
            string targetPath,
            string sourceFileName ,
            EnumFileExtension extension = EnumFileExtension.Webp) 
        {
            try
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(sourcePath + sourceFileName);
                var targetFileName = sourceFileName[..sourceFileName.LastIndexOf('.')];
                var ext = sourceFileName[sourceFileName.LastIndexOf('.')..];

                if (extension == EnumFileExtension.Webp)
                {
                    targetFileName += ".Webp";
                    await image.SaveAsWebpAsync(targetPath + targetFileName);
                }
                else if (extension == EnumFileExtension.Png)
                {
                    targetFileName += ".Png";
                    await image.SaveAsPngAsync(targetPath + targetFileName);
                }
                else
                {
                    targetFileName += ext;
                    await image.SaveAsPngAsync(targetPath + targetFileName);
                }

                image.Dispose();
                string path = sourcePath + sourceFileName;
                if(File.Exists(path)) File.Delete(path);

                return targetFileName;
            }catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }



        public async static Task<string?> SharpUploadFile (
            this IFormFile formFile,
            string path,
            EnumFileExtension extension = EnumFileExtension.Webp)
        {
            try
            {
                if(formFile == null || formFile.Length == 0) return null;

                var extensionFile = Path.GetExtension (formFile.FileName);

                string fileName = DateTime.Now.Ticks.ToString();

                //image
                if (formFile.ContentType.Contains("image"))
                {
                    var tempPath = path + "temp/";
                    var tempFileName = fileName + extensionFile;
                    var tempFilePath = tempPath + tempFileName;

                    CreateDirectory(tempPath);

                    using var stream = File.OpenWrite(tempFilePath);
                    await formFile.CopyToAsync(stream);
                    stream.Close();

                    fileName = await SharpSaveFile(tempPath, path, tempFileName, extension);
                }
                else
                {

                    CreateDirectory(path);

                    fileName += extensionFile;
                    using var stream = File.OpenWrite(path + fileName);
                    await formFile.CopyToAsync(stream);
                    stream.Close();
                }

                return fileName;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public static async Task<string> UploadFileBase64(
            string? fileBase64,
            string path,
            EnumFileExtension extension = EnumFileExtension.Webp)
        {
            try
            {
                if (fileBase64 == null) return null;

                if (!fileBase64.Contains(";base64"))
                    throw new OzelException(ErrorProvider.NotValid);

                var s = Math.Max(fileBase64.IndexOf(":"),0) + 1;

                var contentType = fileBase64[s..fileBase64.IndexOf(";base64")];
                MimeTypes.TryGetExtension(contentType, out string ext);

                var fileName = $"{ext}";

                var tempPath = path + "temp/";

                CreateDirectory(path);

                fileBase64 = fileBase64[(fileBase64.IndexOf(";base64") + 8)..];

                var bytes = Convert.FromBase64String(fileBase64);
                File.WriteAllBytes(tempPath + fileName, bytes);

                if(contentType.Contains("image"))
                    return await SharpSaveFile(tempPath ,path, fileName, extension);

                return fileName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public static async Task<SixLabors.ImageSharp.Image> SharpResizeImageAsync(
            string sourcePath,
            string destinationPath,
            int size,
            CancellationToken token = default)
        {
            var image = await SixLabors.ImageSharp.Image.LoadAsync(sourcePath, token);

            var ratioX = (double)size / image.Width;
            var ratioY = (double)size / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            using var stream = File.Create(destinationPath);

            if (ratio <= 1)
            {
                var width = (int)(image.Width * ratio);
                var height = (int)(image.Height * ratio);
                image.Mutate(x => x.Resize(width, height));
            }
            await image.SaveAsync(stream, WebpFormat.Instance, cancellationToken: token);
            await stream.DisposeAsync();

            return image;
        }



        public static bool Delete(string folderName , string fileName)
        {
            try
            {
                var path = folderName + "/" + fileName;

                bool result = false;

                if (File.Exists(path))
                {
                    File.Delete(path);
                    result = true;
                }

                return result;
            }
            catch { 
              return false;
            }
        }

        public static void CreateDirectory(string pathName)
        {
            if (!Directory.Exists(pathName))
                Directory.CreateDirectory(pathName);
        }

    }
}
