using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GeneralServices.RoundNumber
{
    public interface IRoundNumbers
    {
        //public Task<double> GetRoundNumber(double roundNumber);
        public double GetRoundNumber(double value);
        public double GetDefultRoundNumber(double roundNumber);
        //public double GetRoundNumber(double value, int nfp, int dataType);
        public double GetRoundNumber(double value, int nfp);
       
    }
}
