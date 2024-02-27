using DocumentFormat.OpenXml.ExtendedProperties;
using FastReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Printing.OfferPriceLanding
{
    public class OfferPriceLanding : IOfferPriceLanding
    {


        public async Task<WebReport> OfferPriceLandingPrint()
        {
            // var offerPriceData = new OfferPriceLandingModel();
            // if (offerPriceData.OfferPrice_Detalies == null)
            // {
            //     offerPriceData.OfferPrice_Detalies = new List<OfferPriceLandingDetials>()
            //     {
            //         new OfferPriceLandingDetials(),
            //         new OfferPriceLandingDetials() , 
            //         new OfferPriceLandingDetials(),
            //         new OfferPriceLandingDetials() , new OfferPriceLandingDetials() ,
            //             new OfferPriceLandingDetials() ,
            //             new OfferPriceLandingDetials() ,
            //                 new OfferPriceLandingDetials()
            //     };
            // }
            // Type offerPriceType = offerPriceData.GetType();
            // Type offerPriceDetailsType = offerPriceData.OfferPrice_Detalies[0].GetType();
            // IList<PropertyInfo> propsOfferPrice = new List<PropertyInfo>(offerPriceType.GetProperties());
            // IList<PropertyInfo> propsOfferPriceDetails = new List<PropertyInfo>(offerPriceDetailsType.GetProperties());
            // DataTable offerPriceTable = new DataTable();
            // DataTable offerPriceDetailsTable = new DataTable();
            // DataRow drOfferPrice = offerPriceTable.NewRow();

            // DataRow drOfferPriceDetails = offerPriceDetailsTable.NewRow();
            // foreach (var Property in propsOfferPrice)
            // {

            //     offerPriceTable.Columns.Add(Property.Name);
            //     drOfferPrice[Property.Name] = Property.GetValue(offerPriceData);

            // }
            // offerPriceTable.Rows.Add(drOfferPrice);
            // for (int i = 0; i < offerPriceData.OfferPrice_Detalies.Count(); i++)
            // {

            //     foreach (var Property in propsOfferPriceDetails)
            //     {
            //         if (i == 0)
            //         {
            //             offerPriceDetailsTable.Columns.Add(Property.Name);
            //         }

            //         drOfferPriceDetails[Property.Name] = Property.GetValue(offerPriceData.OfferPrice_Detalies[i]);
            //     }
            //     offerPriceDetailsTable.Rows.Add(drOfferPriceDetails);
            //     drOfferPriceDetails = offerPriceDetailsTable.NewRow();
            // }
            // offerPriceTable.TableName = "OfferPriceLandingModel";
            // offerPriceDetailsTable.TableName = "OfferPriceLandingDetials";
            // WebReport WebReport = new WebReport();
            // var path = Path.Combine("wwwroot", "Reports\\ar", "Offerofprice" + ".frx");
            // WebReport.Report.Load(path);

            // WebReport.Toolbar.Show = false;
            // var currencyWord = ConvertNumberToText.GetText(offerPriceData.net.ToString(), true);
            // WebReport.Report.RegisterData(offerPriceTable, "Ref"+ offerPriceTable.TableName);
            // WebReport.Report.RegisterData(offerPriceDetailsTable, "Ref"+ offerPriceDetailsTable.TableName);
            // WebReport.Report.SetParameterValue("CurrencyWord", currencyWord);

            // WebReport.Report.Prepare();
            //// var file = await _iPrintFileService.PrintFile(WebReport, "OfferPriceLanding", exportType.Print);

            // return WebReport;



            // Create Services 
            // Get Data
            var offerPriceData = new OfferPriceLandingModel();

            //Handling Data
            if (offerPriceData.OfferPrice_Detalies == null)
            {
                new OfferPriceLandingDetials();
                new OfferPriceLandingDetials();
                new OfferPriceLandingDetials();
                new OfferPriceLandingDetials();
                new OfferPriceLandingDetials();
                new OfferPriceLandingDetials();

            }

            //Get Type of data (object of list)
            Type offerPriceType = offerPriceData.GetType();
            Type offerPriceDetailsType = offerPriceData.OfferPrice_Detalies[0].GetType();

            //Get Properities of Types
            IList<PropertyInfo> propOfferPrice = new List<PropertyInfo>(offerPriceType.GetProperties());
            IList<PropertyInfo> propOfferPriceDetails = new List<PropertyInfo>(offerPriceDetailsType.GetProperties());

            //Create DataTable
            DataTable OfferPriceTable = new DataTable();
            DataTable offerPriceDetailsTable = new DataTable();

            //Create Initial Row
            DataRow drOfferPrice = OfferPriceTable.NewRow();
            DataRow drofferPriceDetails = offerPriceDetailsTable.NewRow();

            //Fill Data Table ( Coloumns and Rows)
            // For Table of Object
            foreach (var property in propOfferPrice)
            {
                OfferPriceTable.Columns.Add(property.Name);
                drOfferPrice[property.Name] = property.GetValue(offerPriceData);
            }
            OfferPriceTable.Rows.Add(drOfferPrice);
            // For Table of List
            for (int i = 0; i < offerPriceData.OfferPrice_Detalies.Count(); i++)
            {
                foreach (var property in propOfferPriceDetails)
                {
                    if (i == 0)
                    {
                        offerPriceDetailsTable.Columns.Add(property.Name);
                    }
                    drofferPriceDetails[property.Name] = property.GetValue(offerPriceData.OfferPrice_Detalies[i]);
                }
                offerPriceDetailsTable.Rows.Add(drofferPriceDetails);
                drofferPriceDetails = offerPriceDetailsTable.NewRow();
            }

            //Set Name of Tables
            OfferPriceTable.TableName = "OfferPriceLandingModelN";
            offerPriceDetailsTable.TableName = "OfferPriceDetailsLandingModelN";

            //Get File(Path) and load it in webreport
            WebReport webReport = new WebReport();
            var path = Path.Combine("wwwroot", "Reports\\ar", "OfferPrice" + ".frx");
            webReport.Report.Load(path);

            //Register DataTable (dataTable , Ref) ==> inject data from dataTable To the Refrence Dectionary in FastReport
            webReport.Report.RegisterData(OfferPriceTable, "Ref" + OfferPriceTable.TableName);
            webReport.Report.RegisterData(offerPriceDetailsTable, "Ref" + offerPriceDetailsTable.TableName);

            //Prepare Report
            webReport.Report.Prepare();

            //return WebReport

            return webReport;
        }
    }

    public class OfferPriceLandingModel
    {
        public int Id { get; set; } = 1;
        public int code { get; set; } = 1;
        public string companyName { get; set; } = "CompanyName";
        public string? companyActive { get; set; } = "CompantActive";
        public string personName { get; set; } = "PrsonName";
        public string Email { get; set; } = "Email";
        public string phone { get; set; } = "Phone";
        public string? city { get; set; } = "City";
        public int bundleId { get; set; } = 1;
        public double total { get; set; } = 1;
        public double vat { get; set; }= 1;
        public double net { get; set; } = 1;
        public double discount { get; set; } 
        public string date { get; set; }
        public List<OfferPriceLandingDetials> OfferPrice_Detalies { get; set; }
    }
    public class OfferPriceLandingDetials
    {
        public int item_Id { get; set; } = 1;
        public string item_NameAr { get; set; } = "ItemNameAr";
        public string item_NameEN { get; set; } = "ItemNameEn";
        public double item_Price { get; set; }= 1;
        public int item_Count { get; set; } = 1;
        public double item_VAT { get; set; } = 1;
        public double item_Total { get; set; } = 1;
        public string note { get; set; } = "note";
    }

}
