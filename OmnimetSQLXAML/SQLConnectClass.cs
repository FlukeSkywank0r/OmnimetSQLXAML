using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System;

namespace OmnimetSQLXAML
{
    public class SqlDbConnect
    {
        private SqlConnection con;
        public SqlCommand Cmd;
        private SqlDataAdapter sda;
        public DataTable dta;

        public SqlDbConnect()
        {
            con = new SqlConnection("user id=itwbuehler;" +
                                       "password=05370537;server=localhost;" +
                                       "Trusted_Connection=yes;" +
                                       "database=Omnimet; " +
                                       "connection timeout=30");
            con.Open();
        }

        public void SqlQuery(string queryText)
        {
            Cmd = new SqlCommand(queryText, con);
        }

        public DataTable QueryEx()
        {
            sda = new SqlDataAdapter(Cmd);
            dta = new DataTable();
            sda.Fill(dta);
            return dta;
        }

        public void NonQueryEx()
        {
            try
            {
                Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}