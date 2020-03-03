using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class CreateCategoryViewModel
    {
        public string name { get; set; }

        internal Task CopyToAsync(FileStream fileStream)
        {
            throw new NotImplementedException();
        }
    }

}
