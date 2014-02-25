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
        private static const string DBCStr = "Data Source=(LocalDB)/v11.0;AttachDbFilename='C:/Users/Master/Documents/Visual Studio 2012/Projects/MesterMeind/MesterMeind/WCFServis-Nikola/App_Data/Mastermind.mdf';Integrated Security=True";
        
        /// <summary>
        /// Dodaje prosledjeni uspeh u bazu
        /// </summary>
        /// <param name="u">Uspeh iz kog treba procitati podatke</param>
        /// <param name="ime">Ime takmicara</param>
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

        /// <summary>
        /// Cita sve upisane rezultate u bazi
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
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

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po vremenu
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
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

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po broju pokusaja
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
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
