﻿namespace Webshop.Interfaces
{
    public interface IImage
    {
        public bool CategoryFolder(string categoryFolder);
        public string StoreImage(int productId);
        public string MoveImage();
        public bool ImageExist(string fileName);
        public bool RemoveImage(string imageInCategoryFolder);
    }
}
