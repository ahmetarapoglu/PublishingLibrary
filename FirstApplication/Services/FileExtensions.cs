using Microsoft.EntityFrameworkCore;

namespace BookShop.Services
{
    public static class FileExtensions
    {
        public static bool Delete(string folderName , string fileName)
        {
            try
            {
                var path = folderName +"/"+ fileName;
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
    }
}
