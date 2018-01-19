using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class CreateModel
    {
        public Annunci.Libri.Models.Libro libro { get; set; }

        public List<MyManagerCSharp.Models.MyItem> comboCategorie { get; set; }
        public List<MyManagerCSharp.Models.MyItem> comboSubCategorie { get; set; }

        
        public List<MyManagerCSharp.Models.MyItem> comboRegioni { get; set; }

        public CreateModel()
        {
            comboSubCategorie = new List<MyManagerCSharp.Models.MyItem>();
        }
    }
}