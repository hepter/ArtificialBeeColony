using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ABC.Properties;

namespace ABC
{
    public partial class Form1 : Form
    {
        public static Random rnd = new Random();

        private int imgX, imgY;
        private bool isRunning;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |ControlStyles.UserPaint |ControlStyles.DoubleBuffer,true);
        }
        

        private List<Kaynak> kaynakUret(int miktar)
        {
            int sayi = miktar;
            int maxLimit = (int) numericUpDown2.Value;
            List<Kaynak> kaynaklar = new List<Kaynak>();
            for (int i = 0; i < sayi; i++)
            {
                double s1 = rnd.NextDouble() * 20 - 10;
                double s2 = rnd.NextDouble() * 20 - 10;

                Kaynak k = new Kaynak(s1, s2, maxLimit);
                kaynaklar.Add(k);
            }

            return kaynaklar;
        }

        private List<Arı> ArıUret(int miktar)
        {
            List<Arı> arılar = new List<Arı>();

            for (int i = 0; i < miktar; i++)
                arılar.Add(new Arı());
            return arılar;
        }

        private Series CreateKaynakSeries()
        {
            chart1.Series.Clear();
            Series series = chart1.Series.Add("Sonuclar");
            chart1.IsSoftShadows = false;

            series.ChartType = SeriesChartType.Area;
            series.BorderWidth = 3;
            series.Color = Color.IndianRed;
            return series;
        }

        private bool ToggleKontrol()
        {
            if (isRunning)
            {
                isRunning = false;
                button1.Text = "HESAPLA";
                groupBox1.Enabled = true;
            }
            else
            {
                button1.Text = "Durdur";
                groupBox1.Enabled = false;
                isRunning = true;
            }

            return isRunning;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            if (!ToggleKontrol()) return;
            rnd = new Random(); //Random yenile

            Series series = CreateKaynakSeries();

            int iterasyon = (int) numericUpDown3.Value;
            int populasyon = (int) numericUpDown1.Value;
            int cap = (int) numericUpDown4.Value;

            List<Kaynak> kaynaklar = kaynakUret(populasyon + 1);
            List<Arı> isciArılar = ArıUret(populasyon);
            List<Arı> gözcü_Arılar = ArıUret(populasyon);


            Image img = Resources.matyas;
            Kaynak bestKaynak = kaynaklar.EnIyiKaynak();
            label7.Text = bestKaynak.Uygunluk.ToString();
            for (int i = 0; i < iterasyon; i++)
            {

                kaynaklar = (await IsciArıGezintiAsync(isciArılar, kaynaklar.Haric(bestKaynak))).ToList();
                TabloRender(kaynaklar, cap, colorDialog1.Color, img);
                kaynaklar = (await GözcüGezintiAsync(gözcü_Arılar, kaynaklar.Haric(bestKaynak))).ToList();
                TabloRender(kaynaklar, cap, colorDialog2.Color, img);
                kaynaklar.Add(bestKaynak);
                bestKaynak = kaynaklar.EnIyiKaynak();
                label8.Text = bestKaynak.Uygunluk.ToString();


                pictureBox1.Invoke((Action) delegate { pictureBox1.Image = img; });
                DataPoint dataPoint = new DataPoint(i + 1, 1 - bestKaynak.Uygunluk);
                chart1.Invoke((Action) delegate { series.Points.Add(dataPoint); });
                await Task.Delay(1);

                if (!isRunning) break;
                if (i == iterasyon - 1) ToggleKontrol();
            }
        }

        private Task<Kaynak[]> IsciArıGezintiAsync(List<Arı> arılar, List<Kaynak> kaynaklar)
        {
            List<Task<Kaynak>> taskPool = new List<Task<Kaynak>>(kaynaklar.Count);
            List<Kaynak> yeniKaynaklar = new List<Kaynak>();
            for (int i = 0; i < kaynaklar.Count; i++)
            {
                if (i >= arılar.Count) break;


                Task<Kaynak> t = new Task<Kaynak>(() =>
                {
                    bool uygunlukArttımı = arılar[i].KaynagaGit(kaynaklar[i]);
                    Kaynak guncelKaynak = null;
                    if (uygunlukArttımı)
                        guncelKaynak = arılar[i].SeciliKaynak;
                    else
                        guncelKaynak = kaynaklar[i];

                    if (arılar[i].SeciliKaynak.LimitDoldumu())
                        arılar[i].SeciliKaynak = kaynakUret(1)[0];
                    return guncelKaynak;
                });
                taskPool.Add(t);
                if (i == kaynaklar.Count - 1)
                {
                    break;
                } // i değeri referans hatalı artış hatası
            }

            taskPool.ForEach(a => { a.Start(); });
            return Task.WhenAll(taskPool);
        }

        private Task<Kaynak[]> GözcüGezintiAsync(List<Arı> arılar, List<Kaynak> kaynaklar)
        {
            List<Task<Kaynak>> taskPool = new List<Task<Kaynak>>(kaynaklar.Count);

            List<Kaynak> yeniKaynaklar = new List<Kaynak>();
            for (int i = 0; i < kaynaklar.Count; i++)
            {
                if (i >= arılar.Count) break;

                Task<Kaynak> t = new Task<Kaynak>(() =>
                {
                    Kaynak secilenKaynak = Greedy(kaynaklar);
                    bool uygunlukArttımı = arılar[i].KaynagaGit(secilenKaynak);
                    Kaynak guncelKaynak = null;
                    if (uygunlukArttımı)
                        guncelKaynak = arılar[i].SeciliKaynak;
                    else
                        guncelKaynak = kaynaklar[i];

                    if (arılar[i].SeciliKaynak.LimitDoldumu())
                        arılar[i].SeciliKaynak = kaynakUret(1)[0];
                    return guncelKaynak;
                });
                taskPool.Add(t);
                if (i == kaynaklar.Count - 1)
                {
                    break;
                } // i değeri referans hatalı artış hatası
            }

            taskPool.ForEach(a => { a.Start(); });
            return Task.WhenAll(taskPool);
        }

        private Kaynak Greedy(List<Kaynak> kaynaklar)
        {
            double uygunlukSum = kaynaklar.Sum(a => 1 - a.Uygunluk); //Greedy
            double rand = rnd.NextDouble() * uygunlukSum;
            Kaynak selicenKaynak = null;
            double toplam = 0;
            foreach (var item in kaynaklar)
            {
                toplam += 1 - item.Uygunluk;
                if (toplam > rand)
                {
                    selicenKaynak = item;
                    break;
                }
            }

            return selicenKaynak;
        }

        public void TabloRender(List<Kaynak> k, int cap, Color color, Image img)
        {
            if (imgX == 0)
            {
                imgX = img.Width - 50;
                imgY = img.Height - 50;
            }

            foreach (Kaynak kaynak in k)
            {
                int x = (int) ((kaynak.x1 + 10) / 20 * imgX);
                int y = (int) ((kaynak.x2 + 10) / 20 * imgY);
                drawPoint(x + 25, y + 30, img, cap, color);
            }
        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void ButtonClr1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btn.BackColor = colorDialog1.Color;
            }
        }

        private void ButtonClr2_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                btn.BackColor = colorDialog2.Color;
            }
        }

        public void drawPoint(int x, int y, Image img, int radius, Color color)
        {
            Graphics g = Graphics.FromImage(img);
            int alpha = (int) numericUpDown7.Value;
            Color colorTemp = Color.Empty;

            Point dPoint = new Point(x, img.Height - y);
            dPoint.X = dPoint.X - 2;
            dPoint.Y = dPoint.Y - 2;
         
            colorTemp = Color.FromArgb(alpha, color.R, color.G, color.B);

            g.FillCircle(new SolidBrush(Color.FromArgb(alpha, color.R, color.G, color.B)),
                dPoint.X, dPoint.Y, 2);
            g.DrawCircle(new Pen(colorTemp), dPoint.X, dPoint.Y, radius); //çember
      
            g.Dispose();
        }
    }

    public static class Extensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
            float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
            float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static Kaynak EnIyiKaynak(this List<Kaynak> kaynaklar)
        {
            return kaynaklar.OrderByDescending(a => a.Uygunluk).First();
        }

        public static List<Kaynak> ExcludeEnIyiKaynak(this List<Kaynak> k)
        {
            return k.OrderBy(a => a.Uygunluk).Take(k.Count - 1).ToList();
        }

        public static List<Kaynak> Haric(this List<Kaynak> k, Kaynak haric)
        {
            return k.Where(a => a != haric).ToList();
        }
    }
}