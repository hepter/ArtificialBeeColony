using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public class Arı
    {

        private Kaynak birOncekiKaynak;
        public Kaynak SeciliKaynak { get; set; }
      
        public Arı()
        {

        }

        public override string ToString()
        {
            return SeciliKaynak+"\r\n"+" BirOnceki - "+birOncekiKaynak;
        }

        public bool KaynagaGit(Kaynak k)
        { 
            if (SeciliKaynak==null)
                SeciliKaynak = k;

            int i = Form1.rnd.Next(2);//Seçili Parametre
    
            double yeniParametre = k[i] + Form1.rnd.NextDouble() * (SeciliKaynak[i] - k[i]);
            Kaynak yeniKaynak= new Kaynak(k.MaxLimit);

            int digerKaynakIndis = (i + 1) % 2;
            yeniKaynak[digerKaynakIndis] = SeciliKaynak[digerKaynakIndis];
            yeniKaynak[i] = yeniParametre;
            if (yeniKaynak.Uygunluk>SeciliKaynak.Uygunluk)
            {
                if (SeciliKaynak != null) birOncekiKaynak = SeciliKaynak;// Opsiyonel ( Gözlemlemek için )

                SeciliKaynak.LimitCounter = 0;
                SeciliKaynak = yeniKaynak;

                return true;
            }

            SeciliKaynak.LimitCounter++;
            return false;
        }
    }
}
