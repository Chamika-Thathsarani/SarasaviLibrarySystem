using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarasaviLibrarySystem.Models
{
    public class Copy
    {
        public int CopyID { get; set; }
        public string CopyNumber { get; set; }
        public int BookID { get; set; }
        public string Status { get; set; }
        public bool IsReference { get; set; }
    }
}

