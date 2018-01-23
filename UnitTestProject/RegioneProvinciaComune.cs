using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

namespace UnitTestProject
{
    [TestClass]
    public class RegioneProvinciaComune
    {

        MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("mercatino");

        MyManagerCSharp.RegioniProvinceComuniManager regioniProvinceComuniManager = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");

        [TestMethod]
        public void Regione()
        {
            //cerco gli annunci con regioneId == null
            string strSQL = "SELECT * from ANNUNCIO where regione_id is null";

            try
            {
                manager.mOpenConnection();
                regioniProvinceComuniManager.mOpenConnection();

                DataTable dt = manager.mFillDataTable(strSQL);
                Debug.WriteLine(String.Format("Sono stati trovati {0} annunci", dt.Rows.Count));
                int conta = 0;
                long regioneId;
                foreach (DataRow row in dt.Rows)
                {
                    conta++;
                    Debug.WriteLine(String.Format("{0:N0}/{1:N0}  id: {2} - regione: {3} ", conta, dt.Rows.Count, row["annuncio_id"].ToString(), row["regione"].ToString()));

                    if (!(row["regione"] is DBNull))
                    {
                        regioneId = regioniProvinceComuniManager.getRegioneIdByLabel(row["regione"].ToString());
                        Debug.WriteLine(String.Format("\t regioneId: {0}", regioneId));

                        strSQL = "UPDATE ANNUNCIO SET REGIONE_ID = " + regioneId + " WHERE ANNUNCIO_ID = " + row["annuncio_id"].ToString();
                        manager.mExecuteNoQuery(strSQL);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }
            finally
            {
                manager.mCloseConnection();
                regioniProvinceComuniManager.mCloseConnection();
            }
        }

        [TestMethod]
        public void Provincia()
        {
            //cerco gli annunci con regioneId == null
            string strSQL = "SELECT * from ANNUNCIO where provincia_id is null";

            try
            {
                manager.mOpenConnection();
                regioniProvinceComuniManager.mOpenConnection();

                DataTable dt = manager.mFillDataTable(strSQL);
                Debug.WriteLine(String.Format("Sono stati trovati {0} annunci", dt.Rows.Count));
                int conta = 0;
                string provinciaId;
                foreach (DataRow row in dt.Rows)
                {
                    conta++;
                    Debug.WriteLine(String.Format("{0:N0}/{1:N0}  id: {2} - provincia: {3} ", conta, dt.Rows.Count, row["annuncio_id"].ToString(), row["provincia"].ToString()));

                    if (!(row["provincia"] is DBNull))
                    {
                        provinciaId = regioniProvinceComuniManager.getProvinciaIdByLabel(row["provincia"].ToString());
                        Debug.WriteLine(String.Format("\t provinciaId: {0}", provinciaId));

                        strSQL = "UPDATE ANNUNCIO SET PROVINCIA_ID = '" + provinciaId + "' WHERE ANNUNCIO_ID = " + row["annuncio_id"].ToString();
                        manager.mExecuteNoQuery(strSQL);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }
            finally
            {
                manager.mCloseConnection();
                regioniProvinceComuniManager.mCloseConnection();
            }
        }






    }
}
