using App.Application.Handlers.Helper.GetCompanyConsumption;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cmp;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Handlers.currencyConverter.CurrencyRateResponseDTO;

namespace App.Application.Handlers.currencyConverter
{
    public class currencyConverterHandler : IRequestHandler<currencyConverterRequest, CurrencyResponseDTO>
    {
        public async Task<CurrencyResponseDTO> Handle(currencyConverterRequest request, CancellationToken cancellationToken)
        {
            updateCurrency();
            var currency = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "wwwroot", "CurrencyRate"));
            var response = JsonConvert.DeserializeObject<currencyDTO>(currency);

            var rateOfCurrencyFrom = GetPropertyValue(response.rates, Enum.GetName(request.from));

            var amountAsUSD = request.amount / rateOfCurrencyFrom;
            var newAmount = GetPropertyValue(response.rates, Enum.GetName(request.to)) * amountAsUSD;

            CurrencyResponseDTO res = new CurrencyResponseDTO()
            {
                amount = request.amount,
                baseCarrency = Enum.GetName(request.from),
                baseArName = CurrencyEnum.CurrenciesList.Find(c=> c.Id == (int)request.from).arabicName,
                baseEnName = CurrencyEnum.CurrenciesList.Find(c=> c.Id == (int)request.from).latinName,
                rates = new RatesResponse
                {
                    arabicName = CurrencyEnum.CurrenciesList.Find(c => c.Id == (int)request.to).arabicName,
                    latinName = CurrencyEnum.CurrenciesList.Find(c => c.Id == (int)request.to).latinName,
                    Currency = Enum.GetName(request.to),
                    ConvertedAmount = newAmount
                }
            };
            return res;
        }
        private void updateCurrency()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "CurrencyRate");
            var CurrencyRate_lastUpdate_Path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "CurrencyRate_lastUpdate");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                File.WriteAllText(filePath, getCurrencyRate());
            }
            if (!File.Exists(CurrencyRate_lastUpdate_Path))
            {
                File.Create(CurrencyRate_lastUpdate_Path).Close();
                File.WriteAllText(CurrencyRate_lastUpdate_Path, DateTime.Now.ToString());
            }
            var timeString = File.ReadAllText(CurrencyRate_lastUpdate_Path);
            var lastUpdate = Convert.ToDateTime(timeString);
            if ((DateTime.Now - lastUpdate).TotalMinutes > 20)
            {
                File.WriteAllText(filePath, getCurrencyRate());
                File.WriteAllText(CurrencyRate_lastUpdate_Path, DateTime.Now.ToString());
            }
        }
        public string getCurrencyRate()
        {
            var client = new RestClient("https://openexchangerates.org");
            var RestRequest = new RestRequest("api/latest.json?app_id=ae7101a5f2a84ec3ad8d7a7c598cdf52", Method.Get);
            RestResponse restResponse = client.Execute(RestRequest);
            var statusCode = restResponse.StatusCode;
            
            return restResponse.Content;
        }
        public static double GetPropertyValue(object obj, string propertyName)
        {
            // Use reflection to get the property value
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                return Convert.ToDouble(propertyInfo.GetValue(obj));
            }

            // Handle the case where the property doesn't exist
            return 0;
        }
    }

    public class RatesResponse
    {
        public double ConvertedAmount { get; set; }
        public string Currency { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }

    }
    public class CurrencyResponseDTO
    {
        public double amount { get; set; }
        public string baseCarrency { get; set; }
        public string baseArName { get; set; }
        public string baseEnName { get; set; }
        public RatesResponse rates { get; set; }

    }
}
