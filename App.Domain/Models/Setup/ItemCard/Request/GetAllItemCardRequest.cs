using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetAllItemCardRequest : PaginationVM, IRequest<ResponseResult>
    {
#nullable enable
        public string? Name { get; set; }                 //Here Name represents ItemName (Arabic or Latin) and Itemcode either
        public int Statues { get; set; }         = 0;    //int as 1 to active 2 to not active 0 for all
        public string categories { get; set; }   = "0";  //string like "1,2,3" if 0 get all
        public string itemTypes { get; set; }    = "0";  //string like "1,2,3" if 0 get all
        public string storesPlaces { get; set; } = "0";  //sting like "1,2,3" if 0 get all 

        // Used only For Print
        public bool isPrint { get; set; } = false;
        public bool IsSearchData { get; set; } = true;
        public string? Ids { get; set; }
        //public GetAllItemCardRequest(int PageNumber,int PageSize,string Name,int Statues,string categories,string itemTypes,string storesPlaces)
        //{
        //    this.PageNumber = PageNumber;
        //    this.PageSize = PageSize;
        //    this.Name = Name;
        //    this.Statues = Statues;
        //    this.categories = categories;
        //    this.itemTypes = itemTypes;
        //    this.storesPlaces = storesPlaces;
        //}
    }
    //For Print
    public class InvItemCardDTO
    {
        public int Id { get; set; }
        public int TypeId { get; set; }

        public string TypeNameAr
        {
            get
            {
                if (TypeId == 0)
                {
                    return "الكل";

                }
                else if (TypeId == 1)
                {
                    return "مخزنى";

                }
                else if (TypeId == 2)
                {
                    return "سيريال";
                }
                else if (TypeId == 3)
                {
                    return "تاريخ صلاحية";


                }
                else if (TypeId == 4)
                {
                    return "خدمة";

                }
                else if (TypeId == 6)
                {
                    return "ملاحظة";
                }
                else if (TypeId == 5)
                {
                    return "مركب";


                }
                else if (TypeId == 7)
                {
                    return "عرض";
                }
                else if (TypeId == 8)
                {
                    return "مطعم";



                }
                else if (TypeId == 9)
                {
                    return "ألوان و مقاسات";



                }
                else if (TypeId == 10)
                {
                    return "إضافى";
                }
                else return "";

            }
            #region ItemType
            
            #endregion 
        }
        public string TypeNameEn {
            get
            {
                if (TypeId == 0)
                {

                    return "All";


                }
                else if (TypeId == 1)
                {

                    return "Inventory";


                }
                else if (TypeId == 2)
                {

                    return "Serial";


                }
                else if (TypeId == 3)
                {

                    return "Validity Date";

                }
                else if (TypeId == 4)
                {

                    return "Service";


                }
                else if (TypeId == 6)
                {

                    return "Note";


                }
                else if (TypeId == 5)
                {

                    return "Composit";


                }
                else if (TypeId == 7)
                {
                    return "Offer";

                }
                else if (TypeId == 8)
                {

                    return "Resturant";


                }
                else if (TypeId == 9)
                {

                    return "SizeAndColor";


                }
                else if (TypeId == 10)
                {

                    return "Extras";


                }
                else return "";
            }


        }
        public string NationalBarcode { get; set; }

        public double VAT { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }//Represent the status of the ItemCard 1 if active 2 if inactive
        public string ItemStatusEn
        {
            get
            {
                if (Status == 1)
                    return "Active";
                else if (Status == 2)
                    return "Inactive";
                else return "";
            }
        }
        public string ItemStatusAr
        {
            get
            {
                if (Status == 1)
                    return "نشط";
                else if (Status == 2)
                    return "غير نشط";
                else return "";
            }
        }

        

        public string? CategoryNameAr { get; set; }
        public string? CatogeryNameEn { get; set; }
        // public virtual StorePlacesParameter StorePlace { get; set; }
        public string StorePlaceAr { get; set; }
        public string StorePlaceEn { get; set; }
        public int StorePlaceStatus { get; set; }
        public string StorePlaceStatusEN
        {
            get
            {
                if (StorePlaceStatus == 1) return "Active";
                else if (StorePlaceStatus == 2) return "Inactive";
                else return "";

            }
        }
        public string StorePlaceStatusAr
        {
            get
            {
                if (StorePlaceStatus == 1) return "نشط";
                else if (StorePlaceStatus == 2) return "غير نشط";
                else return "";
            }
        }
        public string StoreNameAr { get; set; }
        public string StoreNameEn { get; set; }
       

    }

    public class ItemCardMainData:InvItemCardDTO
    {
        public string? ItemCode { get; set; }

        public string UnitNameAr { get; set; }
        public string UnitNameEn { get; set; }
        public string? LatinName { get; set; }
        public string? ArabicName { get; set; }
    }
}
