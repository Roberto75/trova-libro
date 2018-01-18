using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class DetailsModel
    {
        public Annunci.Libri.Models.Libro libro;
        public List<Annunci.Models.MyPhoto> photos;
    }
}