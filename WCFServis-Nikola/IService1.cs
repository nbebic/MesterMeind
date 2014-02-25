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
    /// <summary>
    /// Glavni interfejs servisa
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// Zapocinje novu igru
        /// </summary>
        /// <returns>Referenca na igru</returns>
        [OperationContract]
        Igra Nova();

        /// <summary>
        /// Vrsi probu date i konacne kombinacije
        /// </summary>
        /// <param name="k">Kombinacija za proveriti</param>
        /// <param name="i">Referenca na igru</param>
        /// <param name="res">OUT: Ako je tacno, igra je resena</param>
        /// <returns>Rezultat probe</returns>
        [OperationContract]
        Pogodak Proba(Komb k, Igra i, out bool res);

        /// <summary>
        /// Zavrsava igru
        /// </summary>
        /// <param name="i">Referenca na igru</param>
        /// <returns>Konacno resenje</returns>
        [OperationContract]
        Komb Kraj(Igra i);

        /// <summary>
        /// Upesno zavrsava igru
        /// </summary>
        /// <param name="i">Referenca na igru</param>
        /// <param name="ime">Ime takmicara</param>
        /// <returns>Postignut uspeh</returns>
        [OperationContract]
        Uspeh Pobeda(Igra i, string ime);

        /// <summary>
        /// Cita sve upisane rezultate u bazi
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiSve();

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po vremenu
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiVreme();

        /// <summary>
        /// Cita najboljih 10 rezultata sortiranih po broju pokusaja
        /// </summary>
        /// <returns>Rezultat pretrage</returns>
        [FaultContract(typeof(SqlError))]
        [OperationContract]
        DataSet DonesiPokusaji();
    }

    /// <summary>
    /// Klasa koja predstavlja referencu na igru
    /// </summary>
    [DataContract]
    public class Igra 
    {
        /// <summary>
        /// Konacna kombinacija, private i readonly
        /// </summary>
        [DataMember]
        private readonly Komb odg;

        /// <summary>
        /// Zastavica koja predstavlja zavrsenu igru. 
        /// Ako je true, igra je zavrsena
        /// </summary>
        [DataMember]
        private bool gotovo = false;

        /// <summary>
        /// Vreme pocetka i kraja igre
        /// </summary>
        [DataMember]
        private DateTime pocetak, kraj;

        /// <summary>
        /// Ukupan broj pokusaja
        /// </summary>
        [DataMember]
        private int pokusaji = 0;

        /// <summary>
        /// Zastavica koja predstavlja uspesno resavanje igre
        /// Ako je true, igra je uspesno resena
        /// </summary>
        [DataMember]
        private bool reseno = false;

        /// <summary>
        /// Zastavica koja predstavlja uspesno resavanje igre
        /// Ako je true, igra je uspesno resena
        /// </summary>
        /// <value>Getter za reseno</value>
        [DataMember]
        public bool Reseno
        {
            get { return reseno; }
        }

        /// <summary>
        /// Vraca konacno resenje, ili undef ako igra nije gotova
        /// </summary>
        [DataMember]
        public Komb Resenje
        {
            get
            {
                if (gotovo) return odg; 
                else return Komb.undef;
            }
        }

        /// <summary>
        /// Vraca trenutno vreme trajanja igre, ili ukupno vreme ako je igra zavrsena
        /// </summary>
        [DataMember]
        public TimeSpan tren
        {
            get
            {
                if (gotovo) return kraj - pocetak;
                else return DateTime.Now - pocetak;
            }
        }

        /// <summary>
        /// Proverava datu kombinaciju u odnosu na konacno resenje
        /// </summary>
        /// <param name="p">Kombinacija za proveriti</param>
        /// <param name="tacno">OUT: Ako je true, kombinacija je tacna</param>
        /// <returns>Rezultat provere</returns>
        public Pogodak Provera(Komb p, bool tacno)
        {
            tacno = false;
            if (pokusaji < 6)
            {
                pokusaji++;
                tacno = p == odg;//tacno je ako pu identicni pogodak i kombinacija
                reseno = reseno || tacno; //reseno je ako je vec reseno ili je sad tacno
                return odg - p;//vraca rezultat uporedjivanja
            }
            else return new Pogodak(0, 0);
        }

        /// <summary>
        /// Zavrsava igru
        /// </summary>
        /// <returns>Konacno resenje</returns>
        public Komb Zavrsi()
        {
            kraj = DateTime.Now; //sad se zavrsilo
            gotovo = true; //gotovo je
            return odg; //vraca konacno resenje
        }

        /// <summary>
        /// CONSTR: Pravi novu instancu igre
        /// </summary>
        /// <param name="seed">Random "seme" na osnovu kog se pravi kombinacija</param>
        public Igra(int seed)
        {
            odg = new Komb(
                (int)Math.Truncate((double)seed) % 6 + 1,
                (int)Math.Truncate((double)seed / 6) % 6 + 1,
                (int)Math.Truncate((double)seed / 36) % 6 + 1,
                (int)Math.Truncate((double)seed / 216) % 6 + 1);//pravi odgovor
            pocetak = DateTime.Now;//sad pocinje
        }

        /// <summary>
        /// Izracunava trenutni uspeh
        /// </summary>
        /// <returns>Trenutni uspeh, ili undef ako igra nije gotova</returns>
        public Uspeh Skor()
        {
            if (gotovo)
                return new Uspeh(kraj - pocetak, pokusaji);//vraca trenutni skor
            return Uspeh.undef;//ako nije gotovo vraca nedefinisan skor
        }
    }

    /// <summary>
    /// Predstavlja rezultat uporedjivanja 2 kombinacije
    /// </summary>
    [DataContract]
    public struct Pogodak 
    {
        public static readonly Pogodak tacno = new Pogodak(4, 0);
        /// <summary>
        /// Broj pogodjenih i na mestu
        /// </summary>
        public readonly int naMestu;

        /// <summary>
        /// Proj pogodjenih i nisu na mestu
        /// </summary>
        public readonly int tuNegde;

        /// <summary>
        /// Standardni konstruktor
        /// </summary>
        /// <param name="naMestu"><see cref="naMestu"/></param>
        /// <param name="tuNegde"><see cref="tuNegde"/></param>
        public Pogodak(int naMestu, int tuNegde)
        {
            this.naMestu = naMestu;
            this.tuNegde = tuNegde;
        }

        /// <summary>
        /// Uporedjuje 2 pogotka
        /// </summary>
        /// <remarks>znak je + zato sto za == ima problem sa Object.equals() i !=</remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator +(Pogodak a, Pogodak b)
        {
            return (a.naMestu == b.naMestu) && (a.tuNegde == b.tuNegde);
        }
    }

    /// <summary>
    /// Predstavlja jednu kombinaciju
    /// </summary>
    [DataContract]
    public class Komb 
    {
        /// <summary>
        /// Nedefinisana kombinacija
        /// </summary>
        public static Komb undef = new Komb(0, 0, 0, 0);

        /// <summary>
        /// Polja kombinacije
        /// </summary>
        private readonly int[] k = new int[4];

        /// <summary>
        /// Standardni konstruktor
        /// </summary>
        /// <param name="p1">Prvo polje kombinacije</param>
        /// <param name="p2">Drugo polje kombinacije</param>
        /// <param name="p3">Trece polje kombinacije</param>
        /// <param name="p4">Cetvrto polje kombinacije</param>
        public Komb(int p1,int p2, int p3,int p4)
        {
            k[0] = p1;
            k[1] = p2;
            k[2] = p3;
            k[3] = p4;
        }

        /// <summary>
        /// Prebraja znakove u kombinaciji
        /// </summary>
        /// <returns>Niz n, gde n[i] predstavlja koliko puta se znak i ponavlja u kombinaciji</returns>
        public int[] Broj()
        {
            int[] c = {0,0,0,0,0,0,0};
            for (int i = 0; i < 4; i++)
            {
                c[k[i]]++;//prebraja
            }
            return c;
        }

        /// <summary>
        /// OVERLOAD: binary operator -
        /// Uporedjuje dve kombinacije
        /// </summary>
        /// <param name="orig">Originalna kombinacija koju treba proveriti</param>
        /// <param name="pog">Pogodak koji treba proveriti</param>
        /// <returns>Rezultat uporedjivanja</returns>
        public static Pogodak operator -(Komb orig, Komb pog)
        {
            int t=0, d=0; //broj PANNM i PIJNM
            int[] c1 = orig.Broj(), c2 = pog.Broj(); //prebrojene verzije kombinacija

            for (int i = 1; i <= 6; i++) //uporedjuje koliko ima PANNM+PIJNM
                t += Math.Min(c1[i], c2[i]);

            for (int i = 0; i < 4; i++) //gleda koliko ima PIJNM
                d += (orig.k[i] == pog.k[i]) ? 1 : 0;

            return new Pogodak(d, t - d);
        }

        /// <summary>
        /// OVERLOAD: binary operator ==
        /// Uporedjuje 2 kombinacije za jednakost
        /// </summary>
        /// <param name="a">Prva kombinaacija</param>
        /// <param name="b">Druga kombinacija</param>
        /// <returns>True ako su kombinacije identicne; false inace</returns>
        public static bool operator ==(Komb a, Komb b)
        {
            bool p = true;
            for (int i = 0; (i < 3) && p; i++)
                p = p && (a.k[i] == b.k[i]);
            return p;
        }

        /// <summary>
        /// OVERLOAD: binary operator !=
        /// Uporedjuje 2 kombinacije za nejednakost
        /// </summary>
        /// <param name="a">Prva kombinaacija</param>
        /// <param name="b">Druga kombinacija</param>
        /// <returns>False ako su kombinacije identicne; true inace</returns>
        public static bool operator !=(Komb a, Komb b)
        {
            return !(a == b);//logika
        }


    }

    /// <summary>
    /// Predstavlja postignut uspeh (vreme i br. pokusaja)
    /// </summary>
    [DataContract]
    public struct Uspeh 
    {
        public readonly int Vreme;
        public readonly int Pokusaji;
        public static Uspeh undef = new Uspeh(TimeSpan.MaxValue, 10);

        public Uspeh(TimeSpan t, int p)
        {
            Vreme = (int)t.TotalSeconds*100;
            Pokusaji = p;
        }
    }
}
