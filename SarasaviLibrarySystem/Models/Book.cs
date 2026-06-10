using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SarasaviLibrarySystem.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string BookNumber { get; set; }
        public string Classification { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
    }
}
