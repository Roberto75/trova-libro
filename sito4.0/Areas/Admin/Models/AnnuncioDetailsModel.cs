using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class AnnuncioDetailsModel
    {
       public long annuncioId { get; set; }

        public Annunci.Models.Annuncio annuncio;

        public List<Annunci.Models.MyPhoto> photos;
        public List<Annunci.Models.Trattativa> trattative;
        public AnnuncioDetailsModel()
        {
            trattative = new List<Annunci.Models.Trattativa>();
        }
    }
}