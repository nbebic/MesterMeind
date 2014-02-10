using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MasterMind
{
    class Baza
    {
        private const string DBCStr = "Data Source=(LocalDB)/v11.0;AttachDbFilename='C:/Users/Master/Documents/Visual Studio 2012/Projects/MesterMeind/MesterMeind/WCFServis-Nikola/App_Data/Mastermind.mdf';Integrated Security=True";
        
        internal static void Dodaj(Uspeh u, string ime)
        {
            ime = ime.Replace("'", "");
            using(SqlConnection c = new SqlConnection(DBCStr))
            {
                using(SqlCommand q = new SqlCommand("INSERT INTO rezultat (ime,brojPokusaja,vreme) VALUES ('"+ime+"',"+u.Pokusaji.ToString()+","+u.Vreme.ToString()+")",c))
                {
                    q.ExecuteNonQuery();
                }
            }
        }

        internal static DataSet DonesiSve()
        {
            DataSet d = new DataSet();
            using(SqlConnection c = new SqlConnection(DBCStr))
            {
                using(SqlCommand q = new SqlCommand("SELECT ime, brojPokusaja, vreme FROM rezultat",c))
                {
                    using (SqlDataAdapter a = new SqlDataAdapter(q))
                    {
                        a.Fill(d);
                    }
                }
            }
            return d;
        }

        internal static DataSet DonesiVreme()
        {
            DataSet d = new DataSet();
            using (SqlConnection c = new SqlConnection(DBCStr))
            {
                using (SqlCommand q = new SqlCommand("SELECT ime, brojPokusaja, vreme FROM rezultat ORDER BY vreme ASC LIMIT 10", c))
                {
                    using (SqlDataAdapter a = new SqlDataAdapter(q))
                    {
                        a.Fill(d);
                    }
                }
            }
            return d;
        }

        internal static DataSet DonesiPokusaji()
        {
            DataSet d = new DataSet();
            using (SqlConnection c = new SqlConnection(DBCStr))
            {
                using (SqlCommand q = new SqlCommand("SELECT ime, brojPokusaja, vreme FROM rezultat ORDER BY brojPokusaja ASC LIMIT 10", c))
                {
                    using (SqlDataAdapter a = new SqlDataAdapter(q))
                    {
                        a.Fill(d);
                    }
                }
            }
            return d;
        }
    }
}
