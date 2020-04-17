using System.Collections.Generic;

namespace Webshop.Models
{
    public class SpecificationModel
    {
        public Product Product { get; set; }
        public List<SpecificationInfo> SpecificationsList { get; set; }
    }


    public class SpecificationInfo
    {
        public string SpecTitle { get; set; }
        public string SpecInfo { get; set; }
    }
}
