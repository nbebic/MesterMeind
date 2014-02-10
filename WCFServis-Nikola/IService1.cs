using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MasterMind
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        Igra Nova();

        [OperationContract]
        Pogodak Proba(Komb k, Igra i, out bool res);

        [OperationContract]
        Komb Kraj(Igra i);

        [OperationContract]
        Uspeh Jeee(Igra i, string ime);

        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiSve();

        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiVreme();

        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiPokusaji();
    }

    [DataContract]
    public class Igra 
    {
        [DataMember]
        private readonly Komb odg;
        [DataMember]
        private bool gotovo = false;
        [DataMember]
        private DateTime pocetak, kraj;
        [DataMember]
        private int pokusaji = 0;
        [DataMember]
        private bool reseno = false;
        [DataMember]
        public bool Reseno
        {
            get { return reseno; }
        }

        [DataMember]
        public Komb Resenje
        {
            get
            {
                if (gotovo) return odg; else return Komb.undef;
            }
        }

        [DataMember]
        public TimeSpan tren
        {
            get
            {
                if (gotovo) return kraj - pocetak;
                else return DateTime.Now - pocetak;
            }
        }

        public Pogodak Provera(Komb p, bool tacno)
        {
            tacno = false;
            if (pokusaji < 6)
            {
                pokusaji++;
                tacno = p == odg;
                reseno = reseno || tacno;
                return odg - p;
            }
            else return new Pogodak(0, 0);
        }

        public Komb Zavrsi()
        {
            kraj = DateTime.Now;
            gotovo = true;
            return odg;
        }

        public Igra(int seed)
        {
            odg = new Komb(
                (int)Math.Truncate((double)seed) % 6 + 1,
                (int)Math.Truncate((double)seed / 6) % 6 + 1,
                (int)Math.Truncate((double)seed / 36) % 6 + 1,
                (int)Math.Truncate((double)seed / 216) % 6 + 1);
            pocetak = DateTime.Now;
        }

        public Uspeh Skor()
        {
            return new Uspeh(kraj - pocetak, pokusaji);
        }
    }

    [DataContract]
    public struct Pogodak 
    {
        public readonly int naMestu, tuNegde;

        public Pogodak(int a, int b)
        {
            naMestu = a;
            tuNegde = b;
        }
    }

    [DataContract]
    public class Komb 
    {
        public static Komb undef = new Komb(0, 0, 0, 0);
        private readonly int[] k = new int[4];

        public Komb(int p1,int p2, int p3,int p4)
        {
            k[0] = p1;
            k[1] = p2;
            k[2] = p3;
            k[3] = p4;
        }

        public int[] Broj()
        {
            int[] c = {0,0,0,0,0,0,0};
            for (int i = 0; i < 4; i++)
            {
                c[k[i]]++;
            }
            return c;
        }

        public static Pogodak operator -(Komb orig, Komb pog)
        {
            int t=0, d=0;
            int[] c1 = orig.Broj(), c2 = pog.Broj();

            for (int i = 1; i <= 6; i++)
                t += Math.Min(c1[i], c2[i]);

            for (int i = 0; i < 4; i++)
                d += (orig.k[i] == pog.k[i]) ? 1 : 0;

            return new Pogodak(d, t - d);
        }

        public static bool operator ==(Komb a, Komb b)
        {
            bool p = true;
            for (int i = 0; i < 3; i++)
                p = p && (a.k[i] == b.k[i]);
            return p;
        }

        public static bool operator !=(Komb a, Komb b)
        {
            return !(a == b);
        }


    }

    [DataContract]
    public struct Uspeh 
    {
        public readonly int Vreme;
        public readonly int Pokusaji;

        public Uspeh(TimeSpan t, int p)
        {
            Vreme = (int)t.TotalSeconds*100;
            Pokusaji = p;
        }
    }
}
