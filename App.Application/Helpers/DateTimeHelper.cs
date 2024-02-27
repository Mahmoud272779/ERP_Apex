using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace App.Application.Helpers
{
    public static class DateTimeHelper
    {
        public static string DateFormat(DateTime date)
        {
            string _date = date.ToString("yyyy-MM-dd");
            return ConvertDateToDate(_date);
        }
        public static DateTime ConvertDateStringtoDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return default;

            DateTime Date = DateTime.ParseExact(date.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Date;
        }
        public static string ConvertToHourMinuts(string _number)
        {
            try
            {
                double hours = 0;
                double mintus = 0;
                if (Convert.ToDouble(_number) > 0)
                {
                    hours = Math.Floor(Convert.ToDouble(_number) / 60);
                    mintus = Convert.ToDouble(_number) % 60;
                    return hours.ToString() + "H:" + mintus.ToString() + "M";
                }
                else if (Convert.ToDouble(_number) < 0)
                {
                    _number = (Convert.ToDouble(_number) * -1).ToString();
                    hours = Math.Floor(Convert.ToDouble(_number) / 60);
                    mintus = Convert.ToDouble(_number) % 60;

                    return "-" + hours.ToString() + "H:" + mintus.ToString() + "M";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        /// <summary>
        /// Convert Minutes to HH:MM
        /// </summary>
        /// <param name="mins"></param>
        /// <returns></returns>
        public static String convertMinutesToHours(int mins)
        {
            int hours = (mins - mins % 60) / 60;
            return "" + hours + "H : " + (mins - hours * 60) + "M";
        }
        //Added By Abdelaziz Ahmed 02/12/2018 To Add day Formate
        /// <summary>
        /// Convert DateTime (yyyy-MM-dd hh:mm tt)
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string ConvertDateToDateAddDay(string Date, double days)
        {
            try
            {
                string CleanDate = Date.TrimEnd().TrimStart();
                string _Date = DateTime.ParseExact(CleanDate, "yyyy-MM-dd", null).AddDays(days).ToString("yyyy-MM-dd");
                return String.Format("{0:yyyy-MM-dd}", _Date);

            }
            catch
            {
                throw new Exception("InCorrect Date Format");
            }
        }
        //Added By John 06/11/2018 To ChangeDate Formate
        /// <summary>
        /// Convert DateTime (yyyy-MM-dd hh:mm tt) to (yyyy-MM-dd)
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string ConvertDateToDate(string Date)
        {
            try
            {
                string CleanDate = Date.TrimEnd().TrimStart();
                string _Date = DateTime.ParseExact(CleanDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                return String.Format("{0:yyyy-MM-dd}", _Date);

            }
            catch
            {
                throw new Exception("InCorrect Date Format");
            }
        }
        /// <summary>
        /// Convert DateTime (yyyy-MM-dd)
        /// </summary>
        /// <param name="_DateTime"></param>
        /// <returns></returns>
        public static string ConvertDateToDateTime(string _DateTime)
        {
            try
            {
                /* To Merge Spaces to one space
                 * For Ex. 25/10/2018 08:00      AM
                 */
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                _DateTime = regex.Replace(_DateTime, " ");

                string CleanDateTime = _DateTime.TrimEnd().TrimStart();
                string _CleanDateTime = DateTime.ParseExact(CleanDateTime, "yyyy-MM-dd hh:mm tt", null).ToString("yyyy-MM-dd hh:mm tt");
                return String.Format("{0:yyyy-MM-dd hh:mm tt}", _CleanDateTime);

            }
            catch
            {
                throw new Exception("InCorrect Date Format");
            }
        }
        /// <summary>
        /// Convert DateTime to yyyy-MM-dd hh:mm:ss tt (yyyy-MM-dd)
        /// </summary>
        /// <param name="_DateTime"></param>
        /// <returns></returns>
        public static string ConvertDateToDateTimeSec(string _DateTime)
        {
            try
            {
                /* To Merge Spaces to one space
                 * For Ex. 25/10/2018 08:00      AM
                 */
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                _DateTime = regex.Replace(_DateTime, " ");

                string CleanDateTime = _DateTime.TrimEnd().TrimStart();
                string _CleanDateTime = DateTime.ParseExact(CleanDateTime, "yyyy-MM-dd hh:mm:ss tt", null).ToString("yyyy-MM-dd hh:mm:ss tt");
                return String.Format("{0:yyyy-MM-dd hh:mm:ss tt}", _CleanDateTime);

            }
            catch
            {
                throw new Exception("InCorrect Date Format");
            }
        }
    }
}
