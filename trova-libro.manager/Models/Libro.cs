﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trova_libro.manager.Models
{
    public class Libro
    {

        public enum Categorie
        {
            Affitto = 1000000,
            Scambio = 4000000,
            Acquisto = 2000000,
            Vendita = 3000000
        }


        public enum SelectFileds
        {
            Lista,
            Full
        }



        public long annuncioId { get; set; }
        public DateTime dataInserimento { get; set; }

        public long customerId { get; set; }
        public long userId { get; set; }
        public string login { get; set; }


        public string titolo { get; set; }
        public string casaEditrice { get; set; }
        public string autore { get; set; }

        public string nota { get; set; }
        public string isbn { get; set; }


        public decimal prezzo { get; set; }


        public string regione { get; set; }
        public string provincia { get; set; }
        public string comune { get; set; }
        public int regioneId { get; set; }
        public string provinciaId { get; set; }
        public string comuneId { get; set; }




        // le imposto a null atlrimeti mi diventa un filtro obbligatorio nella RICERCA
        public Categorie? categoria { get; set; }

        public Libro()
        {

        }





        public Libro(System.Data.DataRow row, SelectFileds mode)
        {
            annuncioId = long.Parse(row["annuncio_id"].ToString());
            prezzo = (row["prezzo"] is DBNull) ? 0 : Decimal.Parse(row["prezzo"].ToString());

            dataInserimento = (row["date_added"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_added"].ToString());


            userId = (row["user_id"] is DBNull) ? 0 : long.Parse(row["user_id"].ToString());

            login = (row["my_login"] is DBNull) ? "" : row["my_login"].ToString();

            //            categoria = (Models.Immobile.Categorie)int.Parse(row["categoria_id"].ToString());

            // customerId = (row["customer_id"] is DBNull) ? -1 : long.Parse(row["customer_id"].ToString());
            titolo = (row["nome"] is DBNull) ? "" : row["nome"].ToString();


            if (mode == SelectFileds.Full)
            {
                regione = (row["regione"] is DBNull) ? "" : row["regione"].ToString();
                provincia = (row["provincia"] is DBNull) ? "" : row["provincia"].ToString();
                comune = (row["comune"] is DBNull) ? "" : row["comune"].ToString();


                regioneId = (row["regione_id"] is DBNull) ? -1 : int.Parse(row["regione_id"].ToString());
                provinciaId = (row["provincia_id"] is DBNull) ? "" : row["provincia_id"].ToString();
                comuneId = (row["comune_id"] is DBNull) ? "" : row["comune_id"].ToString();


                nota = (row["DESCRIZIONE"] is DBNull) ? "" : row["DESCRIZIONE"].ToString();


                /*   dateLastClick = (row["DATE_LAST_CLICK"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["DATE_LAST_CLICK"].ToString());
                   dateStartClickParziale = (row["date_start_click_parziale"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_start_click_parziale"].ToString());
                   countClick = long.Parse(row["COUNT_CLICK"].ToString());
                   countClickParziale = long.Parse(row["COUNT_CLICK_PARZIALE"].ToString());*/
            }

        }

    }
}
