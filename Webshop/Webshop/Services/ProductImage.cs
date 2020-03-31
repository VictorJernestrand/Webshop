using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Context;
using Webshop.Models;
using Webshop.Interfaces;

namespace Webshop.Services
{
    public class ProductImage
    {
        private readonly string _categoryFolderPath;
        private readonly string _folderName;
        private readonly string _oldFolderName;
        private readonly string _currentImage;
        private readonly IFormFile _file;

        public ProductImage(string pathToRoot)
        {
            _categoryFolderPath = pathToRoot + @"\image\";
            _folderName = null;
            _oldFolderName = null;
            _currentImage = null;
            _file = null;
        }

        public ProductImage(string pathToRoot, string newFolder, string currentImage)
        {
            _categoryFolderPath = pathToRoot + @"\image\";
            _folderName = newFolder;
            _oldFolderName = Path.GetDirectoryName(currentImage);
            _currentImage = Path.GetFileName(currentImage);
            _file = null;
        }

        public ProductImage(string pathToRoot, string folderName, IFormFile file)
        {
            _categoryFolderPath = pathToRoot + @"\image\" + folderName;
            _folderName = folderName;
            _currentImage = null;
            _file = file;
        }

        public bool CategoryFolder(string categoryFolder)
        {
            // Create folder if doesn't exist
            if (!Directory.Exists(categoryFolder))
                Directory.CreateDirectory(categoryFolder);

            // As an extra security measure, check again to see if the folder really exist after it was created above.
            return (!Directory.Exists(categoryFolder)) ? false : true;
        }

        public string StoreImage(int idproductId)
        {
            if (CategoryFolder(_categoryFolderPath))
            {

                // Set name of file using productId.!
                var fileName = idproductId + "_" + _file.FileName;

                // Set full path to new image
                var fullFilePath = Path.Combine(_categoryFolderPath, fileName);

                // Move file to fullFilePath
                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    _file.CopyTo(fileStream);
                }

                // Was the file stored successfully? Return path to image
                return ImageExist(fullFilePath) ? Path.Combine(_folderName, fileName) : null;
            }
            
            return null;
        }


        public string MoveImage()
        {
            var currentImageLocation = Path.Combine(_categoryFolderPath, _oldFolderName, _currentImage);
            var newImageLocation = Path.Combine(_categoryFolderPath, _folderName, _currentImage);

            // Check if image exist before attempting to move it to the new location
            if (ImageExist(currentImageLocation))
            {
                // Make sure the folder exist for the new location
                if (CategoryFolder(Path.Combine(_categoryFolderPath, _folderName)))
                {
                    // Move file and return the new location as string if successful!
                    File.Move(currentImageLocation, newImageLocation);
                    return ImageExist(newImageLocation) ? Path.Combine(_folderName, _currentImage) : null;
                }

                return null;
            }
            else
                return null;
        }

        public bool ImageExist(string pathTofile)
            => File.Exists(pathTofile);

        public void DeleteImage(string pathToImage)
        {
            if (string.IsNullOrEmpty(pathToImage) || !ImageExist(pathToImage))
                return;

            var fullPath = Path.Combine(_categoryFolderPath, pathToImage);

            File.Delete(fullPath);
        }
    }
}
