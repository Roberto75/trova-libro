using Annunci;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trova_libro.manager
{
    public class LibriManager : AnnuncioManager
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
       
            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            string strWHERE = " WHERE ANNUNCIO.date_deleted Is Null";

            if (model.filter != null)
            {
                Debug.WriteLine("Days: " + model.days);
                Debug.WriteLine("Titolo: " + model.filter.titolo);
                Debug.WriteLine("Autore: " + model.filter.autore);
                Debug.WriteLine("Isbn: " + model.filter.isbn);

                if (model.filter.regioneId != null && model.filter.regioneId != -1 && model.filter.regioneId != 0)
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

            strWHERE += getWhereConditionByDate("ANNUNCIO.date_added", model.days);

            if (!String.IsNullOrEmpty(strWHERE))
            {
                mStrSQL += strWHERE;
            }



            mStrSQL += " ORDER BY " + model.Sort + " " + model.SortDir;


            command.CommandText = mStrSQL;

            mDt = mFillDataTable(command);

            model.TotalRows = mDt.Rows.Count;


            if (model.PageSize > 0 && model.PageNumber >= 0)
            {
                // apply paging
                IEnumerable<DataRow> rows = mDt.AsEnumerable().Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize);
                foreach (DataRow row in rows)
                {
                    risultato.Add(new Models.Libro(row, Models.Libro.SelectFileds.Lista));
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


            if (mDt.Rows.Count > 1)
            {
                throw new MyManagerCSharp.MyException("id > 0");
            }

            Models.Libro libro = new Models.Libro(mDt.Rows[0], Models.Libro.SelectFileds.Full);

            return libro;
        }



        public List<MyManagerCSharp.Models.MyItem> getComboCategoriaRoot()
        {
            //mStrSQL = "select A.nome, A.categoria_id ,  (select count (*) from  CATEGORIE where FK_PADRE_ID = a.CATEGORIA_ID  ) as childnodecount  " +
            //      "from CATEGORIE A WHERE FK_PADRE_ID is NULL  AND HIDE = False" +
            //    " ORDER BY nome";

            mStrSQL = "select A.nome, A.categoria_id " +
                    "from CATEGORIE A WHERE FK_PADRE_ID is NULL  AND HIDE = False" +
                    " ORDER BY nome";

            mDt = mFillDataTable(mStrSQL);

            List<MyManagerCSharp.Models.MyItem> risultato = new List<MyManagerCSharp.Models.MyItem>();

            MyManagerCSharp.Models.MyItem item;
            foreach (DataRow row in mDt.Rows)
            {
                item = new MyManagerCSharp.Models.MyItem(row["categoria_id"].ToString(), row["nome"].ToString());

                risultato.Add(item);
            }

            return risultato;

        }





        //elenco delle sotto-categorie
        public List<MyManagerCSharp.Models.MyItem> getComboCategoria(long categoria_id)
        {


            mStrSQL = "select A.nome, A.categoria_id,  (select count (*) from  CATEGORIE where FK_PADRE_ID = a.CATEGORIA_ID  ) as childnodecount  " +
                           " from CATEGORIE A WHERE FK_PADRE_ID = " + categoria_id +
                           " ORDER BY nome";

            mStrSQL = "select A.nome, A.categoria_id  " +
                           " from CATEGORIE A WHERE FK_PADRE_ID = " + categoria_id +
                           " ORDER BY nome";



            mDt = mFillDataTable(mStrSQL);

            List<MyManagerCSharp.Models.MyItem> risultato = new List<MyManagerCSharp.Models.MyItem>();

            MyManagerCSharp.Models.MyItem item;
            foreach (DataRow row in mDt.Rows)
            {
                item = new MyManagerCSharp.Models.MyItem(row["categoria_id"].ToString(), row["nome"].ToString());

                risultato.Add(item);
            }

            return risultato;

        }



        public long insertAnnuncio(Models.Libro libro, long userId)
        {
            return insertAnnuncio(libro, userId, false, 0, DateTime.MinValue);
        }

        public long insertAnnuncioInTestMode(Models.Libro libro, long userId)
        {
            return insertAnnuncio(libro, userId, true, 0, DateTime.MinValue);
        }

        public long insertAnnuncio(Models.Libro libro, long userId, bool test_mode, AnnuncioManager.StatoAnnuncio myStato, DateTime dateAdded)
        {

            if (libro.categoriaId == null || libro.categoriaId == 0)
            {
                throw new MyManagerCSharp.MyException("La Categoria deve essere obbiligatoria");
            }

            string strSQLParametri;

            mStrSQL = "INSERT INTO ANNUNCIO ( FK_CATEGORIA_ID , MY_STATO";
            strSQLParametri = " VALUES ( " + libro.categoriaId;

            if (myStato == 0)
            {
                strSQLParametri += ", '" + AnnuncioManager.StatoAnnuncio.Pubblicato.ToString() + "' ";
            }
            else
            {
                strSQLParametri += ", '" + myStato.ToString() + "' ";
            }


            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            //'27/01/2012 iposto data di modifica =  data inserimento 
            DateTime dataCorrente = DateTime.Now;

            mStrSQL += ",DATE_ADDED, DATE_MODIFIED";
            strSQLParametri += ", @DATE_ADDED , @DATE_MODIFIED ";

            if (dateAdded != DateTime.MinValue)
            {
                mAddParameter(command, "@DATE_ADDED", dateAdded);
                mAddParameter(command, "@DATE_MODIFIED", dateAdded);
            }
            else
            {
                mAddParameter(command, "@DATE_ADDED", dataCorrente);
                mAddParameter(command, "@DATE_MODIFIED", dataCorrente);
            }

            if (userId != 0)
            {
                mStrSQL += ",FK_USER_ID ";
                strSQLParametri += ", @FK_USER_ID ";
                mAddParameter(command, "@FK_USER_ID", userId);
            }

            
            mStrSQL += ",TIPO ";
            strSQLParametri += ", @TIPO ";
            mAddParameter(command, "@TIPO", (int)libro.tipo);
            
            if (!String.IsNullOrEmpty(libro.nota))
            {
                mStrSQL += ",DESCRIZIONE ";
                strSQLParametri += ", @DESCRIZIONE ";
                mAddParameter(command, "@DESCRIZIONE", libro.nota);
            }


            if (!String.IsNullOrEmpty(libro.titolo))
            {
                mStrSQL += ",nome ";
                strSQLParametri += ", @NOME ";
                mAddParameter(command, "@NOME", libro.titolo);
            }

            if (!String.IsNullOrEmpty(libro.autore))
            {
                mStrSQL += ",autore ";
                strSQLParametri += ", @AUTORE ";
                mAddParameter(command, "@AUTORE", libro.autore);
            }





            if (libro.prezzo > 0)
            {
                mStrSQL += ",PREZZO ";
                strSQLParametri += ", @PREZZO ";
                mAddParameter(command, "@PREZZO", libro.prezzo);
            }

            

            if (!String.IsNullOrEmpty(libro.regione))
            {
                mStrSQL += ",REGIONE ";
                strSQLParametri += ", @REGIONE ";
                mAddParameter(command, "@REGIONE", libro.regione);
            }

            if (!String.IsNullOrEmpty(libro.provincia))
            {
                mStrSQL += ",PROVINCIA ";
                strSQLParametri += ", @PROVINCIA ";
                mAddParameter(command, "@PROVINCIA", libro.provincia);
            }

            if (!String.IsNullOrEmpty(libro.comune))
            {
                mStrSQL += ", COMUNE ";
                strSQLParametri += ", @COMUNE ";
                mAddParameter(command, "@COMUNE", libro.comune);
            }


            if (libro.regioneId != null)
            {
                mStrSQL += ",REGIONE_ID ";
                strSQLParametri += ", @REGIONE_ID ";
                mAddParameter(command, "@REGIONE_ID", libro.regioneId);
            }

            //'if (immobile.provinciaId != -1 {
            if (!String.IsNullOrEmpty(libro.provinciaId))
            {
                mStrSQL += ",PROVINCIA_ID ";
                strSQLParametri += ", @PROVINCIA_ID ";
                mAddParameter(command, "@PROVINCIA_ID", libro.provinciaId);
            }

            //'if (immobile.comuneId != -1 {
            if (!String.IsNullOrEmpty(libro.comuneId))
            {
                mStrSQL += ", COMUNE_ID ";
                strSQLParametri += ", @COMUNE_ID ";
                mAddParameter(command, "@COMUNE_ID", libro.comuneId);
            }



            command.CommandText = mStrSQL + " ) " + strSQLParametri + " )";

            if (test_mode == true)
            {
                mTransactionBegin();
                mExecuteNoQuery(command);
                mTransactionRollback();
                return -1;
            }

            mExecuteNoQuery(command);

            return mGetIdentity();
        }





        public List<Models.Libro> getListAnnunci(long userId)
        {
            List<Models.Libro> risultato;
            risultato = new List<Models.Libro>();

            //Debug
            //userId = 567809036;

            //strSQL = "SELECT UTENTI.my_login, UTENTI.user_id, ANNUNCIO.annuncio_id, FORMAT(ANNUNCIO.date_added,""dd-MM-yyyy"") AS date_added, ANNUNCIO.tipo, ANNUNCIO.nome AS nome, ANNUNCIO.prezzo, categorie.nome AS categoria, Switch(tipo=1,'Vendo',tipo=2,'Compro',tipo=3,'Scambio') AS tipo_desc, categorie.categoria_id " & _
            //" FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id " & _
            //" WHERE ANNUNCIO.date_deleted Is Null And ANNUNCIO.fk_user_id= " & CType(Session("SessionData"), MyManager.SessionData).getUserId

            mStrSQL = _sqlElencoLibri;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null   And ANNUNCIO.fk_user_id= " + userId;
            mStrSQL += " ORDER BY ANNUNCIO.date_added DESC";
            mDt = mFillDataTable(mStrSQL);

            Models.Libro libro;

            foreach (DataRow row in mDt.Rows)
            {
                libro = new Models.Libro(row, Models.Libro.SelectFileds.Lista);
                risultato.Add(libro);
            }

            return risultato;

        }



      

    }
}
