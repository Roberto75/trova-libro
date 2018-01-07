using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class CreateModel
    {
        public trova_libro.manager.Models.Libro libro { get; set; }

        public List<MyManagerCSharp.Models.MyItem> comboCategorie { get; set; }
        public List<MyManagerCSharp.Models.MyItem> comboRegioni { get; set; }


    }
}