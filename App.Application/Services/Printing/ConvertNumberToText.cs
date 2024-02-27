using FastReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing
{
    public class ConvertNumberToText
    {
        public static string GetText(string Amount,bool isArabic)
        {
            if (Amount == "")
                Amount = "0";
            Amount = Math.Round(double.Parse(Amount), 2).ToString();
            if (Amount.Contains("E+"))
                return "";
            string Restult = "";
            string Currency, mini;
            if (Amount.ToString().Contains("."))
            {
                string[] intnumber = Amount.Split('.');
                Currency = intnumber[0];
                mini = intnumber[1];
            }
            else
            {
                Currency = Amount;
                mini = "";
            }
           
            string coin =  "";
            string currency =  "";
            if (isArabic)
            {
                coin = "  هللة ";
                currency = " ريال";
            }
            else
            {
                coin = "  halala ";
                currency = "riyal ";
            }

                if (mini.Trim() == "")
                mini = "0";
            if (Currency.Trim() == "")
                Currency = "0";
            if (mini.Length == 1)
                mini = mini + "0";
            if (mini.Length > 2)
                mini = Math.Round(double.Parse(mini), 2).ToString();
            if (int.Parse(mini) > 0 && int.Parse(Currency) > 0)
            {
                if (isArabic)
               
                    
                    
                    Restult = " فقط  " + convertNum_ar_en(Currency.ToString(),isArabic) + " " + currency + " و " + convertNum_ar_en(mini.ToString(), isArabic) + " " + coin + " لا غير ";

                
                else
                    Restult = " only  " + convertNum_ar_en(Currency.ToString(), isArabic) + " " + currency + " and " + convertNum_ar_en(mini.ToString(), isArabic) + " " + coin + " nothing but ";


            }
            else if (Currency != "0" && Currency != "")
            {
                if (isArabic)
                
                    Restult = " فقط  " + convertNum_ar_en(Currency.ToString(), isArabic) + "  " + currency + " لا غير ";
                else
                    Restult = " only  " + convertNum_ar_en(Currency.ToString(), isArabic) + "  " + currency + " nothing but ";


            }
            else if (int.Parse(mini) > 0)
            {
                if (isArabic)
                
                    Restult = " فقط  " + convertNum_ar_en(mini.ToString(), isArabic) + " " + coin + " لا غير ";
                else Restult = " only  " + convertNum_ar_en(mini.ToString(), isArabic) + " " + coin + " nothing but ";


            }
            return Restult;
        }
        public static string convertNum_ar_en(string Number,bool isArabic)
        {
            string word = "";
            if (Number == "")
                return "";
            long dblAmt = Convert.ToInt64(Number);
            //if ((dblAmt > 0) && number.StartsWith("0"))   
            if (dblAmt > 0)
            {
                int numDigits = Number.Length;
                switch (numDigits)
                {
                    
                    case 1:
                        if(isArabic)
                        word = ones_ar(Number);
                        else
                         word = ones_en(Number);

                        break;
                    case 2:
                        if(isArabic)
                        word = tens_ar(Number);
                        else
                            word = tens_en(Number);

                        break;
                    case 3:
                        if(isArabic)
                        word = hundreds_ar(Number);
                        else
                        word = hundreds_en(Number);


                        break;
                    case 4:
                    case 5:
                    case 6:
                        if(isArabic)    
                        word = thousands_ar(Number);
                        else word = thousands_en(Number);   
                        break;
                    case 7:
                    case 8:
                    case 9:
                        if(isArabic)
                        word = millions_ar(Number);
                        else
                            word = millions_en(Number);
                        break;


                }

            }
            return word;


        }

        //ar
        public static string millions_ar(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 1000000);
            switch (pos)
            {
                case 0:
                    break;

                default:
                    place = hundreds_ar(pos.ToString()) + " مليـــــون ";
                    break;

            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 1000000 == 0)
                    word = place;
                else if (place == "")
                    word = thousands_ar((_Number % 1000000).ToString());
                else
                    word = place + " و " + thousands_ar((_Number % 1000000).ToString());
            }
            return word;
        }

        public static string thousands_ar(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 1000);
            switch (pos)
            {
                case 0:
                    break;
                case 1:
                    place = "الف";
                    break;
                case 2:
                    place = " الفــان ";
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    place = ones_ar(pos.ToString()) + " الاف ";
                    break;
                default:
                    place = hundreds_ar(pos.ToString()) + " الف ";
                    break;

            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 1000 == 0)
                    word = place;
                else if (place == "")
                    word = hundreds_ar((_Number % 1000).ToString());
                else
                    word = place + " و " + hundreds_ar((_Number % 1000).ToString());
            }
            return word;
        }
        public static string hundreds_ar(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 100);

            switch (pos)
            {
                case 0:
                    break;
                case 1:
                    place = " مائة ";
                    break;
                case 2:
                    place = " مائتان ";
                    break;
                case 3:
                    place = " ثلاثمائة ";
                    break;
                case 4:
                    place = " اربعمائة ";
                    break;
                case 5:
                    place = " خمسمائة ";
                    break;
                case 6:
                    place = " ستمائة ";
                    break;
                case 7:
                    place = " سبعمائة ";
                    break;
                case 8:
                    place = " ثمانمائة ";
                    break;
                case 9:
                    place = " تسعمائة ";
                    break;



            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 100 == 0)
                    word = place;
                else if (place == "")
                    word = tens_ar((_Number % 100).ToString());
                else
                    word = place + " و " + tens_ar((_Number % 100).ToString());
            }

            return word;
        }
        private static string tens_ar(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {
                case 0:
                    break;
                case 10:
                    name = "عشرة";
                    break;
                case 11:
                    name = "احدى عشر";
                    break;
                case 12:
                    name = "اثنا عشر";
                    break;
                case 13:
                    name = "ثلاث عشر";
                    break;
                case 14:
                    name = "أربعة عشر";
                    break;
                case 15:
                    name = "خمسة عشر";
                    break;
                case 16:
                    name = "ستة عشر";
                    break;
                case 17:
                    name = "سبعة عشر";
                    break;
                case 18:
                    name = "ثمانية عشر";
                    break;
                case 19:
                    name = "تسعة عشر";
                    break;
                case 20:
                    name = "عشرون";
                    break;
                case 30:
                    name = "ثلاثون";
                    break;
                case 40:
                    name = "أربعون";
                    break;
                case 50:
                    name = "خمسون";
                    break;
                case 60:
                    name = "ستون";
                    break;
                case 70:
                    name = "سبعون";
                    break;
                case 80:
                    name = "ثمانون";
                    break;
                case 90:
                    name = "تسعون";
                    break;
                default:

                    if (_Number > 0)
                    {
                        if (_Number / 10 < 1)
                            name = ones_ar((_Number % 10).ToString());
                        else
                        //  if (language.isArabic)
                        { name = ones_ar(Number.Substring(1)) + " و " + tens_ar(Number.Substring(0, 1) + "0"); }
                        //else
                        //{ name = tens_ar(Number.Substring(0, 1) + "0") + ones_ar(Number.Substring(1)); }
                    }
                    break;
            }
            return name;
        }
        private static string ones_ar(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {
                case 0:
                    break;
                case 1:
                    name = "واحد";
                    break;
                case 2:
                    name = "اثنان";
                    break;
                case 3:
                    name = "ثلاثة";
                    break;
                case 4:
                    name = "أربعة";
                    break;
                case 5:
                    name = "خمسة";
                    break;
                case 6:
                    name = "ستة";
                    break;
                case 7:
                    name = "سبعة";
                    break;
                case 8:
                    name = "ثمانية";
                    break;
                case 9:
                    name = "تسعة";
                    break;
            }
            return name;
        }

        // en
        public static string millions_en(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 1000000);
            switch (pos)
            {
                case 0:
                    break;

                default:
                    place = hundreds_en(pos.ToString()) + " million ";
                    break;

            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 1000000 == 0)
                    word = place;
                else if (place == "")
                    word = thousands_en((_Number % 1000000).ToString());
                else
                    word = place + " and " + thousands_en((_Number % 1000000).ToString());
            }
            return word;
        }

        public static string thousands_en(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 1000);
            switch (pos)
            {
                case 0:
                    break;
                case 1:
                    place = "thousand";
                    break;
                case 2:
                    place = " Two thousand ";
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    place = ones_en(pos.ToString()) + " thousands ";
                    break;
                default:
                    place = hundreds_en(pos.ToString()) + " thousand ";
                    break;

            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 1000 == 0)
                    word = place;
                else if (place == "")
                    word = hundreds_en((_Number % 1000).ToString());
                else
                    word = place + " and " + hundreds_en((_Number % 1000).ToString());
            }
            return word;
        }
        public static string hundreds_en(string Number)
        {
            string place = "";

            decimal _Number = Convert.ToDecimal(Number);

            int pos = (int)Math.Truncate(_Number / 100);

            switch (pos)
            {
                case 0:
                    break;
                case 1:
                    place = " hundred ";
                    break;
                case 2:
                    place = " two hundred ";
                    break;
                case 3:
                    place = " three hundred ";
                    break;
                case 4:
                    place = " four hundred ";
                    break;
                case 5:
                    place = " five hundred ";
                    break;
                case 6:
                    place = " six hundred ";
                    break;
                case 7:
                    place = " seven hundred ";
                    break;
                case 8:
                    place = " eight hundred ";
                    break;
                case 9:
                    place = " nine hundred ";
                    break;



            }
            string word = "";
            if (_Number > 0)
            {
                if (_Number % 100 == 0)
                    word = place;
                else if (place == "")
                    word = tens_en((_Number % 100).ToString());
                else
                    word = place + " and " + tens_en((_Number % 100).ToString());
            }

            return word;
        }
        private static string tens_en(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {
                case 0:
                    break;
                case 10:
                    name = "ten";
                    break;
                case 11:
                    name = "eleven";
                    break;
                case 12:
                    name = "twelve";
                    break;
                case 13:
                    name = "thirteen";
                    break;
                case 14:
                    name = "fourteen";
                    break;
                case 15:
                    name = "fifteen";
                    break;
                case 16:
                    name = "sixteen";
                    break;
                case 17:
                    name = "seventeen";
                    break;
                case 18:
                    name = "eighteen";
                    break;
                case 19:
                    name = "nineteen";
                    break;
                case 20:
                    name = "twenty";
                    break;
                case 30:
                    name = "thirty";
                    break;
                case 40:
                    name = "forty";
                    break;
                case 50:
                    name = "fifty";
                    break;
                case 60:
                    name = "sixty";
                    break;
                case 70:
                    name = "seventy";
                    break;
                case 80:
                    name = "eighty";
                    break;
                case 90:
                    name = "ninety";
                    break;
                default:

                    if (_Number > 0)
                    {
                        if (_Number / 10 < 1)
                            name = ones_en((_Number % 10).ToString());
                        else
                        //  if (language.isArabic)
                        { name = ones_en(Number.Substring(1)) + " and " + tens_en(Number.Substring(0, 1) + "0"); }
                        //else
                        //{ name = tens_ar(Number.Substring(0, 1) + "0") + ones_ar(Number.Substring(1)); }
                    }
                    break;
            }
            return name;
        }
        private static string ones_en(string Number)
        {
            int _Number = Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {
                case 0:
                    break;
                case 1:
                    name = "one";
                    break;
                case 2:
                    name = "two";
                    break;
                case 3:
                    name = "three";
                    break;
                case 4:
                    name = "four";
                    break;
                case 5:
                    name = "five";
                    break;
                case 6:
                    name = "six";
                    break;
                case 7:
                    name = "seven";
                    break;
                case 8:
                    name = "eight";
                    break;
                case 9:
                    name = "nine";
                    break;
            }
            return name;
        }
    }
}
