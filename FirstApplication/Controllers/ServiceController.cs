using BookShop.Abstract;
using BookShop.Enum;
using BookShop.Models.EmailSender;
using BookShop.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ServiceController(IEmailService emailService)
        {
            _emailService = emailService;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendMessage(EmailRequest model)
        {
            try
            {
                await _emailService.SendEmailAsync(model);
                return Ok("Email sent successfully");
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            try
            {
                string filename = "";

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = DateTime.Now.Ticks.ToString() + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Files");

                if (!Directory.Exists(filepath))
                    Directory.CreateDirectory(filepath);

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Files", filename);

                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(filename);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SharpUploadFile(
            IFormFile file,
            int thumbSize,
            string imageSize,
            EnumFileExtension extension = EnumFileExtension.Webp)
        {
            try
            {
                int[] imageSizeInt = JsonConvert.DeserializeObject<int[]>(imageSize ?? "[]")!;

                var path = "wwwroot/Upload/Images/";

                if (!file.ContentType.Contains("image")) {
                    throw new OzelException(ErrorProvider.NotValid);
                }

                var fileName = await FileExtensions.SharpUploadFile(file,path,extension);

                var sizePath = path + "resize/";

                if(!Directory.Exists(sizePath))
                    Directory.CreateDirectory(sizePath);

                if(thumbSize > 0)
                {
                    var newFileName ="thumb_" + fileName;

                    _ = await FileExtensions.SharpResizeImageAsync(
                        path + fileName ,sizePath + newFileName , thumbSize);
                }

                if(imageSizeInt?.Length > 0)
                {
                    foreach (var size in imageSizeInt)
                    {
                        var newFileName = size.ToString() + "_" + fileName;
                        _ = await FileExtensions.SharpResizeImageAsync(
                            path + fileName , sizePath + newFileName ,size);
                    }
                }

                return Ok(fileName);
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteFile(string fileName , EnumFileType fileType = EnumFileType.Image)
        {
            try
            {
                var path = "wwwroot/Upload";
                path = fileType == EnumFileType.Image ? path + "/Images" : path + "/Files";

                var res = FileExtensions.Delete(path, fileName);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
