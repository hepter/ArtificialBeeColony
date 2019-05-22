using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ABC
{
    public class Kaynak
    {
        public double x1 { get; set; }
        public double x2 { get; set; }
        private int _maxLimit;
        private int limitCounter;
        public Kaynak(double x1, double x2, int maxLimit)
        {
            this.x1 = x1;
            this.x2 = x2;
            MaxLimit = maxLimit;
        }

        public override string ToString()
        {
            return "Uygunluk:" + Uygunluk.ToString().Substring(0, 6)
                               + $" x1: {doubleToString(x1)}"
                               + $" x2: {doubleToString(x2)}"
                               + " Limit:" + _maxLimit.ToString();
        }

        private string doubleToString(double d)
        {
            int bonus = d > 0?1:0;
            int bonus2 = bonus == 0 ? 7 : 6;
           
            string str = d.ToString().Substring(0, bonus2);

            int len = bonus2 - str.Length + bonus;
            string newStr = new string(' ', len)+ str ;
            return newStr;

        }

        public bool LimitDoldumu()
        {
            if (MaxLimit==limitCounter)
            {

                return true;
            }
            return false;
        }

        public void ResetLimitCounter()
        {
            limitCounter = 0;
        }
        public Kaynak(int maxLimit)
        {
            MaxLimit = maxLimit;
        }
        public double Uygunluk
        {
            get
            {
                double result = (0.26 * (Math.Pow(x1, 2) + Math.Pow(x2, 2))) - (0.48 * x1 * x2);

                return EkUygunluk(result);
            }
            
            
        }

        public int LimitCounter
        {
            get { return limitCounter; }
            set { limitCounter = value; }
        }

        public int MaxLimit
        {
            get { return _maxLimit; }
            set { _maxLimit = value; }
        }

        public double this[int index]
        {
            get
            {
                if (index==0)
                {
                    return x1;
                }
                return x2;
            }
            set
            {
                if (index == 0)
                {
                    x1 = value;
                }
                else
                {
                    x2 = value;
                }
              
            }
        }

        private double EkUygunluk(double d)
        {
            if (d>=0)
            {
                return (double) 1 / (1 + d);
            }
            else
            {
               return (double)1 + Math.Abs(d);
            }
        }


        public void BesinSeçim()
        {



        }
    }
}
