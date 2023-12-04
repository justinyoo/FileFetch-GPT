namespace FileFetch.Helpers
{
    public class FileHelper
    {
        public static byte[] GetFileData(IFormFile file)
        {
            try
            {
                byte[] fileData = null;
                var length = file.Length;
                using (var fileStream = file.OpenReadStream())
                {
                    fileData = new byte[length];
                    fileStream.Read(fileData, 0, (int)file.Length);
                }
                return fileData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsValidFile(IFormFile file)
        {
            bool isValid = false;
            List<string> validFiles = new List<string>() { ".pdf", ".png", ".jpg", ".jpeg"};
            string fileExtension = System.IO.Path.GetExtension(file.FileName);
            isValid = validFiles.Contains(fileExtension);
            return isValid;
        }
    }
}
