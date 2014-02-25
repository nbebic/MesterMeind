using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace MasterMind
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    /// <summary>
    /// Implementacija servisa
    /// </summary>
    public class Service : IService
    {
        /// <summary>
        /// Zapocinje novu igru
        /// </summary>
        /// <returns>Referenca na igru</returns>
        public Igra Nova()
        {
            Random r = new Random();
            return new Igra(r.Next(1296));
        }

        /// <summary>
        /// Vrsi probu date i konacne kombinacije
        /// </summary>
        /// <param name="k">Kombinacija za proveriti</param>
        /// <param name="i">Referenca na igru</param>
        /// <param name="res">OUT: Ako je tacno, igra je resena</param>
        /// <returns>Rezultat probe</returns>
        public Pogodak Proba(Komb k, Igra i, out bool res)
        {
            bool t = true;
            Pogodak p = i.Provera(k, t);
            res = p + Pogodak.tacno;
            if (t) i.Zavrsi();
            return p;
        }

        /// <summary>
        /// Zavrsava igru
        /// </summary>
        /// <param name="i">Referenca na igru</param>
        /// <returns>Konacno resenje</returns>
        public Komb Kraj(Igra i)
        {
            i.Zavrsi();
            return i.Resenje;
        }

        /// <summary>
        /// Upesno zavrsava igru
        /// </summary>
        /// <param name="i">Referenca na igru</param>
        /// <param name="ime">Ime takmicara</param>
        /// <returns>Postignut uspeh</returns>
        public Uspeh Pobeda(Igra i, string ime)
        {
            if (i.Reseno)
            {
                Uspeh u = i.Skor();
                Baza.Dodaj(u, ime);
                return u;
            }
            return new Uspeh(TimeSpan.Zero, -1);

        }

        /// <summary>
        /// Cita sve upisane rezultate u bazi
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        public DataSet DonesiSve()
        {
            return Baza.DonesiSve();
        }

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po vremenu
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        public DataSet DonesiVreme()
        {
            return Baza.DonesiVreme();
        }

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po broju pokusaja
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        public DataSet DonesiPokusaji()
        {
            return Baza.DonesiPokusaji();
        }
    }

}
