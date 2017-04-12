using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trova_libro.manager
{
    public class LibriManager : Annunci.AnnuncioManager
    {


        private const string _sqlElencoLibri = "SELECT UTENTI.my_login, UTENTI.user_id, ANNUNCIO.annuncio_id, ANNUNCIO.autore " +
        ", FORMAT (ANNUNCIO.date_added,\"dd-MM-yyyy\") as date_added " +
        ", ANNUNCIO.tipo, ANNUNCIO.nome AS nome, ANNUNCIO.prezzo, categorie.nome AS categoria, categorie.categoria_id, Switch(tipo=1,'Vendo',tipo=2,'Compro',tipo=3,'Scambio') AS tipo_desc " +
        " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id";
        


        public LibriManager(string connectionName)
            : base(connectionName)
        {

        }

        public LibriManager(System.Data.Common.DbConnection connection)
            : base(connection)
        {

        }


        public void getList(Models.SearchLibri model)
        {

            List<Models.Libro> risultato;
            risultato = new List<Models.Libro>();

            mStrSQL = _sqlElencoLibri;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            string strWHERE = "";

            if (model.filter != null)
            {
                Debug.WriteLine("Days: " + model.days);
                Debug.WriteLine("Titolo: " + model.filter.titolo);
                Debug.WriteLine("Autore: " + model.filter.autore);
                Debug.WriteLine("Isbn: " + model.filter.isbn);

                if (model.filter.regioneId != -1 && model.filter.regioneId != 0)
                {
                    strWHERE += " AND regione_id = " + model.filter.regioneId;
                }

                if (!String.IsNullOrEmpty(model.filter.provinciaId) && model.filter.provinciaId != "-1" && model.filter.provinciaId != "---")
                {
                    strWHERE += " AND provincia_id = @PROVINCIA_ID ";
                    mAddParameter(command, "@PROVINCIA_ID", model.filter.provinciaId);
                }

                if (!String.IsNullOrEmpty(model.filter.comuneId) && model.filter.comuneId != "-1" && model.filter.comuneId != "---")
                {
                    strWHERE += " AND comune_id =  @COMUNE_ID ";
                    mAddParameter(command, "@COMUNE_ID", model.filter.comuneId);
                }


                if (!String.IsNullOrEmpty(model.filter.titolo))
                {
                    strWHERE += " AND ANNUNCIO.nome like  @TITOLO ";
                    mAddParameter(command, "@TITOLO", "%" + model.filter.titolo.Trim() + "%");
                }
                /*
                if (parameters.filter.immobile != null && parameters.filter.immobile > 0)
                {
                    strWHERE += " AND tipo = '" + parameters.filter.immobile.ToString() + "'";
                }

                if (parameters.filter.categoria != null && parameters.filter.categoria > 0)
                {
                    strWHERE += " AND categoria_id = " + ((int)parameters.filter.categoria);
                }


                if (parameters.TipoAnnuncio != null && parameters.TipoAnnuncio.Count < 2)
                {

                    if (parameters.TipoAnnuncio.Contains(Models.SearchImmobili.EnumTipoAnnuncio.Agenzia))
                    {
                        strWHERE += " AND (UTENTI.customer_id IS NOT NULL ) ";
                    }

                    if (parameters.TipoAnnuncio.Contains(Models.SearchImmobili.EnumTipoAnnuncio.Privato))
                    {
                        strWHERE += " AND (UTENTI.customer_id IS NULL ) ";
                    }

                }

    */


            }

            if (!String.IsNullOrEmpty(strWHERE))
            {
                mStrSQL += strWHERE;
            }



            mStrSQL += " ORDER BY " + model.Sort + " " + model.SortDir;


            command.CommandText = mStrSQL;

            mDt = mFillDataTable(command);

            model.TotalRows = mDt.Rows.Count;


            if (model.PageSize  > 0 && model.PageNumber  >= 0)
            {
                // apply paging
                IEnumerable<DataRow> rows = mDt.AsEnumerable().Skip((model.PageNumber  - 1) * model.PageSize ).Take(model.PageSize);
                foreach (DataRow row in rows)
                {
                    risultato.Add(new Models.Libro (row, Models.Libro.SelectFileds.Lista));
                }
            }
            else
            {
                foreach (DataRow row in mDt.Rows)
                {
                    risultato.Add(new Models.Libro(row, Models.Libro.SelectFileds.Lista));
                }
            }
            model.Libri = risultato;
        }



        public Models.Libro getLibro(long id)
        {
            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();

            mStrSQL = "select * from ANNUNCIO where annuncio_id = @ID ";



            mStrSQL = " SELECT UTENTI.my_login AS my_login, UTENTI.user_id AS user_id, UTENTI.isModeratore AS isModeratore, ANNUNCIO.annuncio_id AS annuncio_id, ANNUNCIO.date_added AS date_added, ANNUNCIO.tipo AS tipo, ANNUNCIO.nome AS nome, ANNUNCIO.prezzo AS prezzo, ANNUNCIO.autore AS autore, ANNUNCIO.marca AS marca, ANNUNCIO.modello AS modello, ANNUNCIO.casa_editrice AS casa_editrice, ANNUNCIO.descrizione AS descrizione, ANNUNCIO.stato AS stato, Switch(tipo=1,'Vendo',tipo=2,'Compro',tipo=3,'Scambio') AS tipo_desc, categorie.nome AS categoria, categorie.categoria_id AS categoria_id, ANNUNCIO.isbn AS isbn " +
                ", ANNUNCIO.regione AS regione, ANNUNCIO.provincia AS provincia , ANNUNCIO.comune AS comune" +
                ", ANNUNCIO.regione_id AS regione_id, ANNUNCIO.provincia_id AS provincia_id , ANNUNCIO.comune_id AS comune_id" +
            " ,ANNUNCIO.count_click ,ANNUNCIO.date_start_click_parziale ,ANNUNCIO.date_last_click , ANNUNCIO.count_click_parziale " +
            " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id " +
            " WHERE (ANNUNCIO.date_deleted Is Null) And (ANNUNCIO.ANNUNCIO_ID= @ID )";

            mAddParameter(command, "@ID", id);
            command.CommandText = mStrSQL;

            command.CommandTimeout = 60;

            mDt = mFillDataTable(command);


            if (mDt.Rows.Count == 0)
            {
                return null;
            }

            Models.Libro libro = new Models.Libro(mDt.Rows[0], Models.Libro.SelectFileds.Full);

            return libro;
        }
    }
}
