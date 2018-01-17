﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Models
{
    public class ReplyModel
    {
        public long annuncioId { get; set; }
        public long trattativaId { get; set; }
        public long rispostaId { get; set; }

        public Annunci.Libri.Models.Libro annuncio { get; set; }
        [AllowHtml]
        public string testo { get; set; }
    }
}