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
    public class Service : IService
    {
        public Igra Nova()
        {
            Random r = new Random();
            return new Igra(r.Next(1296));
        }

        public Pogodak Proba(Komb k, Igra i, out bool res)
        {
            bool t = false;
            Pogodak p = i.Provera(k, t);
            res = t;
            if (t) i.Zavrsi();
            return p;
        }

        public Komb Kraj(Igra i)
        {
            throw new NotImplementedException();
        }

        public Uspeh Jeee(Igra i, string ime)
        {
            if (i.Reseno)
            {
                Uspeh u = i.Skor();
                Baza.Dodaj(u, ime);
                return u;
            }
            return new Uspeh(TimeSpan.Zero, -1);

        }


        public DataSet DonesiSve()
        {
            return Baza.DonesiSve();
        }

        public DataSet DonesiVreme()
        {
            return Baza.DonesiVreme();
        }

        public DataSet DonesiPokusaji()
        {
            return Baza.DonesiPokusaji();
        }
    }

}
