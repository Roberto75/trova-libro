using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trova_libro.manager.Models
{
   public  class SearchLibri : PagedLibri
    {

        public Libro filter = new Libro();
        public SearchLibri()
        {
            
            //Sort = "ANNUNCIO.date_added desc,  titolo";
            Sort = "ANNUNCIO.date_added";
            SortDir = "desc";
        }
        }
}
