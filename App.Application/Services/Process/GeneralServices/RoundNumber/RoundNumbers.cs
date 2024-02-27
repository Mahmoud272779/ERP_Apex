using App.Application.Services.HelperService;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GeneralServices.RoundNumber
{
    public class RoundNumbers : IRoundNumbers
    {
        private readonly IHelperService generalSetteing;
        private InvGeneralSettings _setting;
        public static int defaultDecimal = 6;

        //public InvGeneralSettings GLsetting
        //{

        //    get { return (InvGeneralSettings)generalSettingsRepositoyQuery.GetAll().ToList().FirstOrDefault(); ; }

        //}
        public RoundNumbers(IHelperService generalSetteing)
        {
            this.generalSetteing = generalSetteing;
            this._setting = generalSetteing.GetAllGeneralSettings().Result;
        }
        //will use when activate decimal system
        public async Task<double> GetRoundNumberD(double roundNumber)
        {
            //var set = _setting.Other_Decimals;
            //var setting = await generalSetteing.GetAllGeneralSettings();
            int decemalNum = _setting.Other_Decimals;
            if (_setting.Other_UseRoundNumber)
            {
                return Numbers.RoundedUp(roundNumber, decemalNum);
            }
            else 
            {
             return   Numbers.Roundedvalues(roundNumber, decemalNum);
            }
        
        }

        //defult for any number insert to database by defult
        public double GetDefultRoundNumber(double roundNumber)
        {
         
         return   Numbers.RoundedUp(roundNumber, defaultDecimal);

        }

        // seting 

        public  double GetRoundNumber(double value)
        {
             // var set = _setting.Other_Decimals;
            //  var setting = await generalSetteing.GetAllGeneralSettings();
           //   int decemalNum = _setting.Other_Decimals;
          //    return Numbers.RoundedUp(roundNumber, decemalNum);
        
            return GetRoundNumber(value, _setting.Other_Decimals);


        }


        public double GetRoundNumber(double value, int nfp, int dataType )
        {
            if(_setting.Other_Decimals>nfp)
                nfp = _setting.Other_Decimals;

            double result = Math.Round(value,nfp);
            //
            //result = SetZerosOfDeicimal(result, nfp);
            //if (result == 0)
            //    return value;
      
            return result;
        }

        public double GetRoundNumber(double value, int nfp)
        {
            return GetRoundNumber(value, nfp, (int)dataTypeOfRound.other);
        }

        public double SetZerosOfDeicimal(double value ,int nfp)
        {
            string result="";
            int count = BitConverter.GetBytes(decimal.GetBits((decimal)value)[3])[2];
            var zero = "";
            if(nfp>count)
            {
                //count = nfp - count;
                for (int i = 0; i < nfp; i++)
                {
                    zero = String.Concat("0", zero);
                }
                result = String.Format("{0:0.0000}", value);
            }
            return Convert.ToDouble( result);

        }
    }
}
