using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ABC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        static Random rnd = new Random();
        
        List<Kaynak> kaynakUret(int miktar)
        {

            int sayi =miktar;
            int maxLimit =(int)numericUpDown2.Value;
            List<Kaynak> kaynaklar = new List<Kaynak>();
            for (int i = 0; i < sayi; i++)
            {
                double s1 = rnd.NextDouble() * 20 - 10;
                double s2 = rnd.NextDouble() * 20 - 10;

                Kaynak k = new Kaynak(s1,s2, maxLimit);
                kaynaklar.Add(k);

            }

            return kaynaklar;
        }

        List<Arı> ArıUret(int miktar)
        {

            List<Arı> arılar = new List<Arı>();

            for (int  i= 0;  i< miktar; i++)
            {
                arılar.Add(new Arı());
            }
            return arılar;
        }
        private Series KaynakSeries()
        {

            //flowLayoutPanel1.Controls.Clear();
            //label11.Text = "Toplam:0";
            //pictureBox1.Image = Properties.Resources.matyas;
            this.chart1.Series.Clear();
            Series series = this.chart1.Series.Add("Sonuclar");
            chart1.IsSoftShadows = false;

            series.ChartType = SeriesChartType.Area;
            series.BorderWidth = 3;
            series.Color = Color.IndianRed;
            return series;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {

            int iterasyon = (int) numericUpDown3.Value;
            int populasyon = (int) numericUpDown1.Value;
            progressBar1.Maximum = iterasyon;
            progressBar1.Value = 0;


            List<Kaynak> kaynaklar = kaynakUret(populasyon);
            List<Arı> arılar = ArıUret(populasyon);
            List<Arı> gözcüArılar = ArıUret(populasyon);


            kaynaklar[0].ToString();
            Series series = KaynakSeries();
            Kaynak sonKaynak=null;
            chart1.SuspendLayout();
            for (int i = 0; i < iterasyon; i++)
            {
                

                ArıGezinti(arılar,kaynaklar);
                GözcüGezinti(gözcüArılar, kaynaklar);
                Kaynak k = kaynaklar.EnIyiKaynak();
                if (sonKaynak?.Uygunluk!=k?.Uygunluk)
                {
                    sonKaynak = k;
                }

                series.Points.AddXY(i, k.Uygunluk);
                progressBar1.Value++;

               // await Task.Delay(1);
            }
            chart1.ResumeLayout();
        }

        void ArıGezinti(List<Arı> arılar, List<Kaynak> kaynaklar)
        {
            for (int i = 0; i < arılar.Count; i++)
            {
                bool uygunlukArttımı=arılar[i].KaynagaGit(kaynaklar[i]);

                if (uygunlukArttımı)
                    kaynaklar[i] = arılar[i].SeciliKaynak;
                
            }
        }
        void GözcüGezinti(List<Arı> arılar, List<Kaynak> kaynaklar)
        {


            for (int i = 0; i < arılar.Count; i++)
            {
                Kaynak secilenKaynak = Greedy(kaynaklar);
                bool uygunlukArttımı = arılar[i].KaynagaGit(secilenKaynak);

                if (uygunlukArttımı)
                    kaynaklar[i] = secilenKaynak;

                if (arılar[i].SeciliKaynak.LimitDoldumu())
                {
                    arılar[i].SeciliKaynak = kaynakUret(1)[0];
                }
            }
        }

        Kaynak Greedy(List<Kaynak> kaynaklar)
        {
            double uygunlukSum = kaynaklar.Sum(a => 1 - a.Uygunluk);//Greedy

            List<int> sans= new List<int>();
            int i = 0;
            foreach (Kaynak k in kaynaklar)
            {
                var newD = ((1 - k.Uygunluk) * 1000).ToString().Substring(0, 3).Replace(",","");
                int loop = (int.Parse(newD));
                int[] sayi= new int[loop];
                for (int j = 0; j < loop; j++)
                {
                    sayi[j] = i;
                    
                }
                sans.AddRange(sayi);
                i++;
            }



            Kaynak selicenKaynak = kaynaklar[sans[rnd.Next(0, sans.Count)]];
            return selicenKaynak;
        }
    }

    public static class Extensions
    {
        public static Kaynak EnIyiKaynak(this List<Kaynak> kaynaklar)
        {
            return kaynaklar.OrderByDescending(a => a.Uygunluk).First();
        }
    }
}
