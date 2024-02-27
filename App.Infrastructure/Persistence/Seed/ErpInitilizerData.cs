using App.Domain;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Process.Store.Barcode;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using App.Infrastructure.settings;
using System.Data;
using static App.Domain.Enums.BarcodeEnums;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Shared.accountantTree;

namespace App.Infrastructure.Persistence.Seed
{
    // code #011144477 for database script
    public class ErpInitilizerData : IErpInitilizerData
    {
        public GLBranch[] ReturnBranchTypeList()
        {
            var dayTypeCountList = new List<GLBranch>();
            dayTypeCountList.AddRange(new[] {
                //"الفرع الرئيسي" && e.NameEn == "Main branch")
               new GLBranch {
                   Id = 1,
                   LatinName = "Main branch",
                   ArabicName = "الفرع الرئيسي",
                   Code=1,
                   Status=(int)Status.Active,
                   ManagerId = 1,
                   UTime = DateTime.Now,
                   SalesPriceId=(int)SalePricesList.SalePrice1

               },

            });

            return dayTypeCountList.ToArray();
        }
        
        public GLCurrency[] ReturnWithCurrencyList()
        {
            var CurrencyList = new List<GLCurrency>();
            CurrencyList.AddRange(new[] {
                new GLCurrency {
                    Id =1,
                    Code=1,
                    AbbrEn ="SAR" ,
                    AbbrAr ="ر س" ,
                    ArabicName= "ريال سعودي" ,
                    LatinName="Saudi Riyal",
                    CoinsAr="هللة",
                    CoinsEn="halala",
                    Status=(int)Status.Active,
                    Factor=1,
                    IsDefault=true,
                    CurrancySymbol="",
                   Notes=""
                }
               });
            return CurrencyList.ToArray();
        }
        public GLFinancialBranch[] ReturnFinancialAccountBranches()
        {
            var branches = new List<GLFinancialBranch>();
            //var FA_list = DefultGLFinancialAccountList(); 
            var FA_list = accountantTree.DefultGLFinancialAccountList();//uncomment this for creating database script #011144477
            foreach (var item in FA_list)
            {
                branches.Add(new GLFinancialBranch { BranchId = 1, FinancialId = item.Id });
            }
            return branches.ToArray();
        }
        public GLFinancialAccount[] ReturnWithFinancialAccountList()
        {
            #region old 
            //var CurrencyList = new List<GLFinancialAccount>();
            //CurrencyList.AddRange(new[] {
            //    //الاصول
            //    new GLFinancialAccount { Id =1,IsMain=true, ArabicName= "الاصول" ,LatinName="Assets", Status=(int)Status.Active,ParentId=null,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01",autoCoding ="1",MainCode=1,SubCode=0},
            //    new GLFinancialAccount { Id =2,IsMain=true, ArabicName= "الاصول الثابتة" ,LatinName="Fixed Assets", Status=(int)Status.Active,ParentId=1,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001",autoCoding ="1.1",MainCode=1,SubCode=1},
            //    new GLFinancialAccount { Id =3,IsMain=true, ArabicName= "اجهزة كمبيوتر" ,LatinName="Computers", Status=(int)Status.Active,ParentId=2,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.001",autoCoding ="1.1.1",MainCode=1,SubCode=2},
            //    new GLFinancialAccount { Id =4,IsMain=false , ArabicName= "اجهزه ديسك توب" ,LatinName="Desktop devices", Status=(int)Status.Active,ParentId=3,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.001.001",autoCoding ="1.1.1.1",MainCode=1,SubCode=3},

            //    new GLFinancialAccount { Id =5,IsMain=false, ArabicName= "اجهزة كمبيوتر محمولة (لاب توب)" ,LatinName="Laptops", Status=(int)Status.Active,ParentId=3,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.001.002",autoCoding ="1.1.1.2",MainCode=1,SubCode=4},

            //    new GLFinancialAccount { Id =6,IsMain=true, ArabicName= "أثاث ومفروشات" ,LatinName="Furniture",Status= (int)Status.Active,ParentId=2,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002",autoCoding ="1.1.2",MainCode=1,SubCode=3},





            //    new GLFinancialAccount { Id =7,IsMain=true, ArabicName= "مكاتب" ,LatinName="Offices", Status=(int)Status.Active,ParentId=6,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.001",autoCoding ="1.1.2.1",MainCode=1,SubCode=4},
            //    new GLFinancialAccount { Id =8,IsMain=false, ArabicName= "مكتب زجاج" ,LatinName="Glass Office", Status=(int)Status.Active,ParentId=7,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.001.001",autoCoding ="1.1.2.1.1",MainCode=1,SubCode=5},





            //    new GLFinancialAccount { Id =9,IsMain=false, ArabicName= "مكتب خشب" ,LatinName="Wood Office", Status=(int)Status.Active,ParentId=8,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.001.002",autoCoding ="1.1.2.1.2",MainCode=1,SubCode=6},
            //    new GLFinancialAccount { Id =10,IsMain=true, ArabicName= "الكراسي" ,LatinName="Chairs", Status=(int)Status.Active,ParentId=6,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.002",autoCoding ="1.1.2.2",MainCode=1,SubCode=5},

            //    new GLFinancialAccount { Id =11,IsMain=false, ArabicName= "كرسي ثابت" ,LatinName="Fixed Chair", Status=(int)Status.Active,ParentId=10,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.002.001",autoCoding ="1.1.2.2.1",MainCode=1,SubCode=6},
            //    new GLFinancialAccount { Id =12,IsMain=false, ArabicName= "كرسي دوار كبير" ,LatinName="Large Swivel Chair", Status=(int)Status.Active,ParentId=10,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.002.002",autoCoding ="1.1.2.2.2",MainCode=1,SubCode=7},



            //    new GLFinancialAccount { Id =13,IsMain=false, ArabicName= "كرسي دوار صغير" ,LatinName="Small Swivel Chair", Status=(int)Status.Active,ParentId=10,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.002.002.003",autoCoding ="1.1.2.2.3",MainCode=1,SubCode=8},


            //    new GLFinancialAccount { Id =14,IsMain=true, ArabicName= "سيارات" ,LatinName="Cars", Status=(int)Status.Active,ParentId=2,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.003",autoCoding ="1.1.3",MainCode=1,SubCode=4},
            //    new GLFinancialAccount { Id =15,IsMain=true, ArabicName= "سيارات نقل" ,LatinName="Transportation Cars", Status=(int)Status.Active,ParentId=14,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.003.001",autoCoding ="1.1.3.1",MainCode=1,SubCode=4},




            //    new GLFinancialAccount { Id =16,IsMain=false, ArabicName= "سيارة نقل 1" ,LatinName="Transportation Cars-1", Status=(int)Status.Active,ParentId=15,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.003.001.001",autoCoding ="1.1.3.1.1",MainCode=1,SubCode=6},
            //    new GLFinancialAccount { Id =17,IsMain=true, ArabicName= "سيارات خاصة" ,LatinName="Private Cars", Status=(int)Status.Active,ParentId=14,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.003.002",autoCoding ="1.1.3.2",MainCode=1,SubCode=6},
            //    new GLFinancialAccount { Id =18,IsMain=false, ArabicName= "1سيارة خاصة" ,LatinName="Private Cars-1", Status=(int)Status.Active,ParentId=17,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.001.003.002.001",autoCoding ="1.1.3.2.1",MainCode=1,SubCode=7},


            //    new GLFinancialAccount { Id =19,IsMain=true, ArabicName= "الأصول المتداولة" ,LatinName="Current Assets", Status=(int)Status.Active,ParentId=1,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002",autoCoding ="1.2",MainCode=1,SubCode=2},

            //    new GLFinancialAccount { Id =20,IsMain=true, ArabicName= "البنوك" ,LatinName="Banks", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.001",autoCoding ="1.2.1",MainCode=1,SubCode=3},




            //    new GLFinancialAccount { Id =21,IsMain=false, ArabicName= "بنك إفتراضى" ,LatinName="Default bank", Status=(int)Status.Active,ParentId=20,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.001.001",autoCoding ="1.2.1.1",MainCode=1,SubCode=4},

            //    new GLFinancialAccount { Id =22,IsMain=true, ArabicName= "الخزائن" ,LatinName="", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.002",autoCoding ="1.2.2",MainCode=1,SubCode=4},
            //    new GLFinancialAccount { Id =23,IsMain=false, ArabicName= "خزينة رئيسية" ,LatinName="", Status=(int)Status.Active,ParentId=22,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.002.001",autoCoding ="1.2.2.1",MainCode=1,SubCode=5},

            //    new GLFinancialAccount { Id =24,IsMain=true, ArabicName= "العملاء" ,LatinName="Customers", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.003",autoCoding ="1.2.3",MainCode=1,SubCode=5},




            //    new GLFinancialAccount { Id =25,IsMain=false, ArabicName= "عميل نقدي" ,LatinName="Cash Customer", Status=(int)Status.Active,ParentId=24,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.003.001",autoCoding ="1.2.3.1",MainCode=1,SubCode=6},

            //    new GLFinancialAccount { Id =26,IsMain=false, ArabicName= "ذمم الموظفين والسلف" ,LatinName="Staff receivables and advances", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.004",autoCoding ="1.2.4",MainCode=1,SubCode=6},

            //    new GLFinancialAccount { Id =27,IsMain=true, ArabicName= "أرصده مدينة اخري" ,LatinName="Other Debit Balances", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.005",autoCoding ="1.2.5",MainCode=1,SubCode=7},
            //    new GLFinancialAccount { Id =28,IsMain=false, ArabicName= "المخزون" ,LatinName="Stock", Status=(int)Status.Active,ParentId=19,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.006",autoCoding ="1.2.6",MainCode=1,SubCode=6},


            //    new GLFinancialAccount { Id =29,IsMain=false, ArabicName= "مصروف مقدم" ,LatinName="", Status=(int)Status.Active,ParentId=27,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.005.001",autoCoding ="1.2.5.1",MainCode=1,SubCode=8},
            //    new GLFinancialAccount { Id =30,IsMain=false, ArabicName= "ايراد مستحق" ,LatinName="Accrued Revenue", Status=(int)Status.Active,ParentId=27,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="01.002.005.002",autoCoding ="1.2.5.2",MainCode=1,SubCode=9},




            //    //الخصوم
            //    new GLFinancialAccount { Id =31,IsMain=true, ArabicName= "الخصوم" ,LatinName="", Status=(int)Status.Active,ParentId=null,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02",autoCoding ="2",MainCode=2,SubCode=0},

            //    new GLFinancialAccount { Id =32,IsMain=true, ArabicName= "الخصوم طويلة اللاجل" ,LatinName="", Status=(int)Status.Active,ParentId=31,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.001",autoCoding ="2.1",MainCode=2,SubCode=1},
            //    new GLFinancialAccount { Id =33,IsMain=false, ArabicName= "قروض" ,LatinName="", Status=(int)Status.Active,ParentId=32,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.001.001" ,autoCoding ="2.1.1",MainCode=2,SubCode=2},

            //    new GLFinancialAccount { Id =34,IsMain=true, ArabicName= "الخصوم المتداولة" ,LatinName="", Status=(int)Status.Active,ParentId=31,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002",autoCoding ="2.2",MainCode=2,SubCode=2},
            //    new GLFinancialAccount { Id =35,IsMain=true, ArabicName= "الموردين" ,LatinName="", Status=(int)Status.Active,ParentId=34,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002.001",autoCoding ="2.2.1",MainCode=2,SubCode=3},
            //    new GLFinancialAccount { Id =36,IsMain=false, ArabicName= "مورد نقدى" ,LatinName="", Status=(int)Status.Active,ParentId=35,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002.001.001",autoCoding ="2.2.1.1",MainCode=2,SubCode=4},
            //    new GLFinancialAccount { Id =37,IsMain=false, ArabicName= "ضريبة القيمة المضافة" ,LatinName="", Status=(int)Status.Active,ParentId=34,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=1,AccountCode="02.002.002",autoCoding ="2.2.2",MainCode=2,SubCode=4},
            //    new GLFinancialAccount { Id =38,IsMain=false, ArabicName= "ارصدة دائنة اخري" ,LatinName="", Status=(int)Status.Active,ParentId=34,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002.003",autoCoding ="2.2.3",MainCode=2,SubCode=5},

            //    new GLFinancialAccount { Id =39,IsMain=false, ArabicName= "مصروف مستحق" ,LatinName="", Status=(int)Status.Active,ParentId=37,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002.003.001",autoCoding ="2.2.3.2",MainCode=2,SubCode=6},
            //    new GLFinancialAccount { Id =40,IsMain=false, ArabicName= "ايراد مقدم" ,LatinName="", Status=(int)Status.Active,ParentId=37,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="02.002.003.002",autoCoding ="2.2.3.2",MainCode=2,SubCode=7},




            //    //حقوق الملكية
            //    new GLFinancialAccount { Id =41,IsMain=true, ArabicName= "حقوق الملكية" ,LatinName="", Status=(int)Status.Active,ParentId=null,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03",autoCoding ="3",MainCode=3,SubCode=0},
            //    new GLFinancialAccount { Id =42,IsMain=true, ArabicName= "راس المال" ,LatinName="", Status=(int)Status.Active,ParentId=41,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.001",autoCoding ="3.1",MainCode=3,SubCode=1},
            //    new GLFinancialAccount { Id =43,IsMain=false, ArabicName= "راس المال الشريك ا" ,LatinName="", Status=(int)Status.Active,ParentId=42,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.001.001",autoCoding ="3.1.1",MainCode=3,SubCode=2},
            //    new GLFinancialAccount { Id =44,IsMain=false, ArabicName= "راس المال الشريك ب" ,LatinName="", Status=(int)Status.Active,ParentId=42,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.001.002",autoCoding ="3.1.2",MainCode=3,SubCode=3},
            //    new GLFinancialAccount { Id =45,IsMain=true, ArabicName= "ارباح وخسائر مرحلة" ,LatinName="", Status=(int)Status.Active,ParentId=41,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.002",autoCoding ="3.2",MainCode=3,SubCode=2},
            //    new GLFinancialAccount { Id =46,IsMain=false, ArabicName= "ارباح محتجزة" ,LatinName="", Status=(int)Status.Active,ParentId=45,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.002.001",autoCoding ="3.2.1",MainCode=3,SubCode=3},
            //    new GLFinancialAccount { Id =47,IsMain=false, ArabicName= "خسائر مرحلة" ,LatinName="", Status=(int)Status.Active,ParentId=45,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=1,AccountCode="03.002.002",autoCoding ="3.2.2",MainCode=3,SubCode=4},



            //    //الايرادات
            //    new GLFinancialAccount { Id =48,IsMain=true, ArabicName= "الايرادات" ,LatinName="", Status=(int)Status.Active,ParentId=null,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="04",autoCoding ="4",MainCode=4,SubCode=0},
            //    new GLFinancialAccount { Id =49,IsMain=true, ArabicName= "ايرادات النشاط التجاري" ,LatinName="", Status=(int)Status.Active,ParentId=48,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="04.001",autoCoding ="4.1",MainCode=4,SubCode=1},
            //    new GLFinancialAccount { Id =50,IsMain=false, ArabicName= "المبيعات" ,LatinName="", Status=(int)Status.Active,ParentId=49,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="04.001.001",autoCoding ="4.1.1",MainCode=4,SubCode=2},
            //    new GLFinancialAccount { Id =51,IsMain=false, ArabicName= "مردودات المبيعات" ,LatinName="", Status=(int)Status.Active,ParentId=49,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="04.001.002",autoCoding ="4.1.2",MainCode=4,SubCode=3},
            //    new GLFinancialAccount { Id =52,IsMain=false, ArabicName= "مسموحات المبيعات" ,LatinName="", Status=(int)Status.Active,ParentId=49,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="04.001.003",autoCoding ="4.1.3",MainCode=4,SubCode=4},
            //    new GLFinancialAccount { Id =53,IsMain=false, ArabicName= "خصم مسموح به" ,LatinName="", Status=(int)Status.Active,ParentId=49,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="04.001.004",autoCoding ="4.1.4",MainCode=4,SubCode=5},

            //    new GLFinancialAccount { Id =54,IsMain=true, ArabicName= "ايرادات اخري" ,LatinName="", Status=(int)Status.Active,ParentId=48,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="04.002",autoCoding ="4.2",MainCode=4,SubCode=2},
            //    new GLFinancialAccount { Id =55,IsMain=false, ArabicName= "فوائد بنكية" ,LatinName="", Status=(int)Status.Active,ParentId=52,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="04.002.001",autoCoding ="4.2.1",MainCode=4,SubCode=3},




            //    //المصروفات والتكاليف
            //    new GLFinancialAccount { Id =56,IsMain=true, ArabicName= "المصروفات والتكاليف" ,LatinName="", Status=(int)Status.Active,ParentId=null,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05",autoCoding ="5",MainCode=5,SubCode=0},
            //    new GLFinancialAccount { Id =57,IsMain=true, ArabicName= "تكلفة النشاط التجاري" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.001",autoCoding ="5.1",MainCode=5,SubCode=1},


            //    new GLFinancialAccount { Id =58,IsMain=false, ArabicName= "تكلفة المبيعات" ,LatinName="", Status=(int)Status.Active,ParentId=57,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.001.001",autoCoding ="5.1.1",MainCode=5,SubCode=2},



            //    new GLFinancialAccount { Id =59,IsMain=false, ArabicName= "خصم مكتسب" ,LatinName="", Status=(int)Status.Active,ParentId=57,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="05.001.002",autoCoding ="5.1.2",MainCode=5,SubCode=3},
            //    //new GLFinancialAccount { Id =59,IsMain=false, ArabicName= "مردودات المشتريات" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="05.001.003",autoCoding ="5.1.3",MainCode=5,SubCode=4},
            //    //new GLFinancialAccount { Id =60,IsMain=false, ArabicName= "مسموحات المشتريات" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="05.001.004",autoCoding ="5.1.4",MainCode=5,SubCode=5},
            //    //new GLFinancialAccount { Id =61,IsMain=false, ArabicName= "مصروف شراء" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="05.001.005",autoCoding ="5.1.5",MainCode=5,SubCode=6},
            //    //new GLFinancialAccount { Id =62,IsMain=false, ArabicName= "خصم مكتسب" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Credit,FinalAccount=2,AccountCode="05.001.006",autoCoding ="5.1.6",MainCode=5,SubCode=7},

            //    new GLFinancialAccount { Id =60,IsMain=true, ArabicName= "المصروفات" ,LatinName="", Status=(int)Status.Active,ParentId=56,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002",autoCoding ="5.2",MainCode=5,SubCode=2},
            //    new GLFinancialAccount { Id =61,IsMain=true, ArabicName= "مصاريف ادارية" ,LatinName="", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.001",autoCoding ="5.2.1",MainCode=5,SubCode=3},
            //    new GLFinancialAccount { Id =62,IsMain=false, ArabicName= "رواتب" ,LatinName="", Status=(int)Status.Active,ParentId=61,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.001.001",autoCoding ="5.2.1.1",MainCode=5,SubCode=4},
            //    new GLFinancialAccount { Id =63,IsMain=false, ArabicName= "كهرباء" ,LatinName="", Status=(int)Status.Active,ParentId=61,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.001.002",autoCoding ="5.2.1.2",MainCode=5,SubCode=5},
            //    new GLFinancialAccount { Id =64,IsMain=false, ArabicName= "مياه" ,LatinName="", Status=(int)Status.Active,ParentId=61,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.001.003",autoCoding ="5.2.1.3",MainCode=5,SubCode=6},



            //    new GLFinancialAccount { Id =65,IsMain=false, ArabicName= "الموظفين" ,LatinName="Employees", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.005",autoCoding ="5.2.5",MainCode=5,SubCode=6},
            //    new GLFinancialAccount { Id =66,IsMain=false, ArabicName= "موظف افتراضي" ,LatinName="Defult Employee", Status=(int)Status.Active,ParentId=65,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.005.001",autoCoding ="5.2.5.1",MainCode=5,SubCode=4},
            //    new GLFinancialAccount { Id =67,IsMain=false, ArabicName= "كاشير افتراضي" ,LatinName="Casher Employee", Status=(int)Status.Active,ParentId=65,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.005.002",autoCoding ="5.2.5.2",MainCode=5,SubCode=4},

            //    new GLFinancialAccount { Id =68,IsMain=true, ArabicName= "مصاريف بيعية وتسويقية" ,LatinName="", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.002",autoCoding ="5.2.2",MainCode=5,SubCode=4},
            //    new GLFinancialAccount { Id =69,IsMain=false, ArabicName= "دعاية واعلان" ,LatinName="", Status=(int)Status.Active,ParentId=68,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.002.001",autoCoding ="5.2.2.1",MainCode=5,SubCode=5},
            //    new GLFinancialAccount { Id =70,IsMain=true, ArabicName= "عمولات المناديب" ,LatinName="", Status=(int)Status.Active,ParentId=68,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.002.002",autoCoding ="5.2.2.2",MainCode=5,SubCode=6},

            //    new GLFinancialAccount { Id =71,IsMain=true, ArabicName= "مصاريف الاهلاك" ,LatinName="", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.003",autoCoding ="5.2.3",MainCode=5,SubCode=5},



            //    new GLFinancialAccount { Id =72,IsMain=false, ArabicName= "مصروف اهلاك اجهزة كمبيوتر" ,LatinName="", Status=(int)Status.Active,ParentId=71,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.003.001",autoCoding ="5.2.3.1",MainCode=5,SubCode=6},
            //    new GLFinancialAccount { Id =73,IsMain=false, ArabicName= "مصروف اهلاك اثاث ومفروشات" ,LatinName="", Status=(int)Status.Active,ParentId=71,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.003.002",autoCoding ="5.2.3.2",MainCode=5,SubCode=7},
            //    new GLFinancialAccount { Id =74,IsMain=false, ArabicName= "مصروف اهلاك سيارات" ,LatinName="", Status=(int)Status.Active,ParentId=71,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.003.003",autoCoding ="5.2.3.3",MainCode=5,SubCode=8},

            //    new GLFinancialAccount { Id =75,IsMain=true, ArabicName= "مصاريف تشغيليه" ,LatinName="", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.004",autoCoding ="5.2.4",MainCode=5,SubCode=6},
            //    new GLFinancialAccount { Id =76,IsMain=true, ArabicName= "صيانة سيارات الفنيين" ,LatinName="", Status=(int)Status.Active,ParentId=75,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.004.001",autoCoding ="5.2.4.1",MainCode=5,SubCode=7},


            //    new GLFinancialAccount { Id =77,IsMain=true, ArabicName= "جهات صرف اخري" ,LatinName="Other Authorities", Status=(int)Status.Active,ParentId=60,CurrencyId=1,BranchId=1,FA_Nature=(int)FA_Nature.Debit,FinalAccount=2,AccountCode="05.002.005",autoCoding ="5.2.5",MainCode=5,SubCode=7}

            //});
            #endregion
            //var list = DefultGLFinancialAccountList();
            var list = accountantTree.DefultGLFinancialAccountList(); //uncomment this for creating database script #011144477
            return list.ToArray();
        }
        public GLSafe[] ReturnTreasuryTypeList()
        {
            var dayTypeCountList = new List<GLSafe>();
            dayTypeCountList.AddRange(new[] {
               new GLSafe {
                   Id=1,
                   Code=1,
                   BranchId = 1,
                   LatinName = "Main Safe",
                   ArabicName = "خزينة رئيسية",
                   FinancialAccountId= (int)FinanicalAccountDefultIds.Mainbranchtreasury,
                   Status =(int)Status.Active
               },

            });

            return dayTypeCountList.ToArray();
        }
        public GLGeneralSetting[] SetGlGeneralSettings()
        {
            var settings = new List<GLGeneralSetting>();
            var subCodes = new List<SubCodeLevels>();
            subCodes.AddRange(new[]
            {
                new SubCodeLevels{value = 2},
                new SubCodeLevels{value = 3},
                new SubCodeLevels{value = 3},
                new SubCodeLevels{value = 3},
            });
            settings.AddRange(new[] {
             new GLGeneralSetting
             {
                 Id=1,
                 AutomaticCoding=true,
                 MainCode=2,
                 SubCode=3,
                 BranchId=1,

                 DefultAccSafe =(int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdSafe = (int)FinanicalAccountDefultIds.Cashinthesafe,

                 DefultAccBank = (int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdBank = (int)FinanicalAccountDefultIds.cashinthebank,

                 DefultAccSupplier = (int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdSupplier = (int)FinanicalAccountDefultIds.Suppliers,

                 DefultAccCustomer = (int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdCustomer = (int)FinanicalAccountDefultIds.Customers,

                 DefultAccEmployee = (int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdEmployee = (int)FinanicalAccountDefultIds.StaffSalariesAndWages,

                 DefultAccSalesMan = (int)GLGeneralSetting_RelationTypes.OneAccountForAll,
                 FinancialAccountIdSalesMan = (int)FinanicalAccountDefultIds.salesMan,

                 DefultAccOtherAuthorities =(int)GLGeneralSetting_RelationTypes.generateUnderOneAccount,
                 FinancialAccountIdOtherAuthorities = (int)FinanicalAccountDefultIds.OtherAuthorities,


                 evaluationMethodOfEndOfPeriodStockType = 1
             }
            });
            return settings.ToArray();
        }
        public GLBank[] ReturnBanksTypeList()
        {
            var dayTypeCountList = new List<GLBank>();
            dayTypeCountList.AddRange(new[] {
               new GLBank {
                   Id=1,
                   Code=1,
                   //BankBranches=new List<GLBankBranch> (){new GLBankBranch (){BankId=1,BranchId=1}},
                   LatinName = "Main Banks",
                   ArabicAddress="",
                   LatinAddress="",
                   ArabicName = "بنك افتراضي",
                   FinancialAccountId=(int)FinanicalAccountDefultIds.AlRajhiBank,
                   Status=(int)Status.Active,
                   AccountNumber="",
                   Email="",
                   Phone="",
                   Website="",
                   BrowserName="",
                   Notes=""
               },

            });

            return dayTypeCountList.ToArray();
        }
        public GLJournalEntry[] SetGLJournalEntry()
        {
            var glJournalEntry = new List<GLJournalEntry>();
            glJournalEntry.AddRange(new[]
            {
                new GLJournalEntry {
                    Id = -1,
                    Code = -1,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                },
                //Customers
                new GLJournalEntry {
                    Id = -2,
                    Code = -2,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                },
                //Suppliers
                new GLJournalEntry {
                    Id = -3,
                    Code = -3,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                },
                //Safes
                new GLJournalEntry {
                    Id = -4,
                    Code = -4,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                },
                //Banks
                new GLJournalEntry {
                    Id = -5,
                    Code = -5,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                },
                //Items
                new GLJournalEntry {
                    Id = -6,
                    Code = -6,
                    CurrencyId = 1,
                    IsDeleted = false,
                    CreditTotal = 0,
                    DebitTotal = 0,
                    Notes = "",
                    IsBlock = false,
                    IsTransfer = false,
                    BrowserName = "",
                    Auto = true,
                    ReceiptsId = null,
                    InvoiceId = null,
                    BranchId = 1,
                    //LastTransactionAction = "u",
                    //LastTransactionUser = "System",
                    //AddTransactionDate = DateTime.Now.ToString(),
                    //AddTransactionUser = "System",
                    //LastTransactionDate =DateTime.Now.ToString(),
                    //DeleteTransactionDate = DateTime.Now.ToString(),
                    //DeleteTransactionUser = "System",

                }
            });
            return glJournalEntry.ToArray();
        }
        public userAccount[] SetUserAccount()
        {
            var userAccount = new List<userAccount>();
            userAccount.AddRange(new[]
            {
                new userAccount
                {
                    id  = 1,
                    employeesId = 1,
                    isActive = true,
                    password = "admin",
                    username = "admin",
                    permissionListId = 1

                },
                new userAccount
                {
                    id  = 2,
                    employeesId = 2,
                    isActive = true,
                    password = "Casher",
                    username = "1234",
                    permissionListId = 2

                }
            });
            return userAccount.ToArray();
        }
        public otherSettings[] setOtherSettings()
        {
            var otherSettings = new List<otherSettings>();
            otherSettings.AddRange(new[]
            {
                new otherSettings
                {
                    Id = 1,
                    accredditForAllUsers = true,
                    posAddDiscount = true,
                    posAllowCreditSales = true,
                    posCashPayment = true,
                    posEditOtherPersonsInv = true,
                    posNetPayment = true,
                    posOtherPayment = true,
                    posShowOtherPersonsInv = true,
                    posShowReportsOfOtherPersons = true,
                    salesAddDiscount = true,
                    salesAllowCreditSales = true,
                    salesCashPayment = true,
                    salesEditOtherPersonsInv = true,
                    salesNetPayment = true,
                    salesOtherPayment = true,
                    salesShowOtherPersonsInv = true,
                    salesShowReportsOfOtherPersons = true,
                    salesShowBalanceOfPerson=true ,
                    purchasesAllowCreditSales = true,
                    purchasesAddDiscount = true,
                    purchasesEditOtherPersonsInv = true,
                    purchasesShowOtherPersonsInv = true,
                    purchasesShowReportsOfOtherPersons = true,
                    PurchasesCashPayment = true,
                    PurchasesNetPayment =  true,
                    PurchasesOtherPayment = true,
                    purchaseShowBalanceOfPerson=true,
                    showAllBranchesInCustomerInfo = true,
                    showAllBranchesInSuppliersInfo = true,
                    showCustomersOfOtherUsers = true,
                    showHistory = true,
                    showOfferPricesOfOtherUser = true,
                    userAccountId = 1,
                    showDashboardForAllUsers = true,
                    canShowAllPOSSessions = true,
                    allowCloseCloudPOSSession = true,
                    CollectionReceipts=true
                },
                new otherSettings
                {
                    Id = 2,
                    accredditForAllUsers = true,
                    posAddDiscount = true,
                    posAllowCreditSales = true,
                    posCashPayment = true,
                    posEditOtherPersonsInv = true,
                    posNetPayment = true,
                    posOtherPayment = true,
                    posShowOtherPersonsInv = true,
                    posShowReportsOfOtherPersons = true,
                    salesAddDiscount = true,
                    salesAllowCreditSales = true,
                    salesCashPayment = true,
                    salesEditOtherPersonsInv = true,
                    salesNetPayment = true,
                    salesOtherPayment = true,
                    salesShowOtherPersonsInv = true,
                    salesShowReportsOfOtherPersons = true,
                    salesShowBalanceOfPerson=true,
                    purchasesAllowCreditSales = true,
                    purchasesAddDiscount = true,
                    purchasesEditOtherPersonsInv = true,
                    purchasesShowOtherPersonsInv = true,
                    purchasesShowReportsOfOtherPersons = true,
                    PurchasesCashPayment = true,
                    PurchasesNetPayment =  true,
                    PurchasesOtherPayment = true,
                    purchaseShowBalanceOfPerson = true,
                    showAllBranchesInCustomerInfo = true,
                    showAllBranchesInSuppliersInfo = true,
                    showCustomersOfOtherUsers = true,
                    showHistory = true,
                    showOfferPricesOfOtherUser = false,
                    userAccountId = 2,
                    canShowAllPOSSessions = true,
                    allowCloseCloudPOSSession = true,
                   
                }
            });
            return otherSettings.ToArray();
        }
        public OtherSettingsBanks[] setOtherSettingBanks()
        {
            var otherSettingBanks = new List<OtherSettingsBanks>();
            otherSettingBanks.AddRange(new[]
            {
                new OtherSettingsBanks
                {
                    Id = 1,
                    gLBankId =1,
                    otherSettingsId = 1,
                },
                new OtherSettingsBanks
                {
                    Id = 2,
                    gLBankId =1,
                    otherSettingsId = 2,
                }
            });
            return otherSettingBanks.ToArray();
        }
        public OtherSettingsSafes[] setOtherSettingSafes()
        {
            var otherSettingsSafes = new List<OtherSettingsSafes>();
            otherSettingsSafes.AddRange(new[]
            {
                new OtherSettingsSafes
                {
                    Id = 1,
                    gLSafeId = 1,
                    otherSettingsId = 1
                },
                new OtherSettingsSafes
                {
                    Id = 2,
                    gLSafeId = 1,
                    otherSettingsId = 2
                }
            });
            return otherSettingsSafes.ToArray();
        }
        public OtherSettingsStores[] setOtherSettingStores()
        {
            var otherSettingsStores = new List<OtherSettingsStores>();
            otherSettingsStores.AddRange(new[]
            {
                new OtherSettingsStores
                {
                    Id = 1,
                    InvStpStoresId = 1,
                    otherSettingsId = 1
                },
                new OtherSettingsStores
                {
                    Id = 2,
                    InvStpStoresId = 1,
                    otherSettingsId = 2
                }
            });
            return otherSettingsStores.ToArray();
        }
        public permissionList[] setPermissionLists()
        {
            var permissionLists = new List<permissionList>();
            permissionLists.AddRange(new[]
            {
                new permissionList
                {
                    Id = 1,
                    arabicName = "مدير النظام",
                    latinName = "Administrator"
                },
                new permissionList
                {
                    Id = 2,
                    arabicName = "كاشير",
                    latinName = "Casher"
                }
            });
            return permissionLists.ToArray();
        }
        public UserAndPermission[] setUserAndPermissions()
        {
            var usersAndPermissions = new List<UserAndPermission>();
            usersAndPermissions.AddRange(new[]
            {
               new UserAndPermission
               {
                   Id = 1,
                   permissionListId = 1,
                   userAccountId = 1
               },
               new UserAndPermission
               {
                   Id = 2,
                   permissionListId = 2,
                   userAccountId = 2
               }
            });
            return usersAndPermissions.ToArray();
        }
        public rules[] setRules()
        {
            var listOfRulesAdministrator = returnSubForms.returnRules(true, 1);
            int id = 0;
            for (int i = 0; i < listOfRulesAdministrator.Count(); i++)
            {
                id += 1 * -1;
                listOfRulesAdministrator[i].Id = id;
            }
            var listOfRulesCasher = returnSubForms.returnRules(false, 2);
            for (int i = 0; i < listOfRulesCasher.Count(); i++)
            {

                id += 1 * -1;
                listOfRulesCasher[i].Id = id;
                if (listOfRulesCasher[i].mainFormCode == (int)MainFormsIds.pos)
                {
                    listOfRulesCasher[i].isAdd = true;
                    listOfRulesCasher[i].isDelete = true;
                    listOfRulesCasher[i].isEdit = true;
                    listOfRulesCasher[i].isPrint = true;
                    listOfRulesCasher[i].isShow = true;
                }

            }
            //POS
            //set pos defulat value for casher here
            //POS
            var rules = new List<rules>();
            rules.AddRange(listOfRulesAdministrator);
            rules.AddRange(listOfRulesCasher);
            return rules.ToArray();
        }
        public InvEmployees[] setDefultEmployees()
        {
            var emplyees = new List<InvEmployees>();
            emplyees.AddRange(new[]
            {
                new InvEmployees
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "موظف افتراضي",
                    LatinName = "Default employee",
                    Status = (int)Status.Active,
                    JobId = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.DefultEmployee,
                    SalesPriceId=(int)SalePricesList.SalePrice1
                },
                new InvEmployees
                {
                    Id = 2,
                    Code = 2,
                    ArabicName = "كاشير افتراضي",
                    LatinName = "Default Casher",
                    Status = (int)Status.Active,
                    JobId = 2,
                     FinancialAccountId = (int)FinanicalAccountDefultIds.DefultCasher,
                     SalesPriceId=(int)SalePricesList.SalePrice1
                }
            });
            return emplyees.ToArray();
        }
        public InvStorePlaces[] setDefultStoresPlaces()
        {
            var storesPlaces = new List<InvStorePlaces>();
            storesPlaces.AddRange(new[]
            {
                new InvStorePlaces
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "رف1",
                    LatinName = "Rack 1",
                    Status = (int)Status.Active,
                }
            });
            return storesPlaces.ToArray();
        }
        public InvStpStores[] setDefultStores()
        {
            var stores = new List<InvStpStores>();
            stores.AddRange(new[]
            {
                new InvStpStores
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "مستودع رئيسي",
                    LatinName = "Main store",
                    Status = (int)Status.Active
                }
            });
            return stores.ToArray();
        }
        public InvEmployeeBranch[] setEmployeesBranches()
        {
            var listEmployeeBranch = new List<InvEmployeeBranch>();
            listEmployeeBranch.AddRange(new[]
            {
                new InvEmployeeBranch
                {
                    EmployeeId = 1,
                    BranchId = 1
                },
                new InvEmployeeBranch
                {
                    EmployeeId = 2,
                    BranchId = 1
                }
            });
            return listEmployeeBranch.ToArray();
        }
        public InvStoreBranch[] setStoreBranch()
        {
            var listInvStoreBranch = new List<InvStoreBranch>();
            listInvStoreBranch.AddRange(new[]
            {
                new InvStoreBranch
                {
                    StoreId = 1,
                    BranchId = 1
                }
            });
            return listInvStoreBranch.ToArray();
        }
        public GLBankBranch[] GLBankBranch()
        {
            var list = new List<GLBankBranch>();
            list.AddRange(new[]
            {
                new GLBankBranch
                {
                    BankId = 1,
                    BranchId = 1
                }
            });
            return list.ToArray();
        }
        public InvColors[] setColors()
        {
            var list = new List<InvColors>();
            list.AddRange(new[]
            {
                new InvColors
                {
                    Id =1,
                    Code = 1,
                    ArabicName = "أحمر",
                    LatinName = "Red",
                    Color = "#FF0000",
                    Status = (int)Status.Active
                }
            });
            return list.ToArray();
        }
        public InvSizes[] setInvSizes()
        {
            var list = new List<InvSizes>();
            list.AddRange(new[]
            {
                new InvSizes
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "صغير",
                    LatinName = "Small",
                    Status = (int)Status.Active,
                    CanDelete = false
                }
            });
            return list.ToArray();
        }
        public InvStpUnits[] setInvStpUnits()
        {
            var list = new List<InvStpUnits>();
            list.AddRange(new[]
            {
                new InvStpUnits
                {
                    Id =1,
                    Code = 1,
                    ArabicName = "حبة",
                    LatinName = "Piece",
                    Status = (int)Status.Active,
                }
            });
            return list.ToArray();
        }
        public InvCategories[] setInvCategories()
        {
            var list = new List<InvCategories>();
            list.AddRange(new[]
            {
                new InvCategories
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "مجموعة افتراضية",
                    LatinName = "Default category",
                    Status = (int)Status.Active,
                    Color = "#040404",
                    UsedInSales = 1
                }
            });
            return list.ToArray();
        }
        public InvGeneralSettings[] setInvGeneralSettings()
        {
            var list = new List<InvGeneralSettings>();
            list.AddRange(new[]
            {
                new InvGeneralSettings
                {
                    Id = 1,
                    Purchases_ModifyPrices = true,//السماح بتعديل الأسعار
                    Purchases_PayTotalNet = false,//يجب أن تسدد قيمة المستند كاملة
                    Purchases_UseLastPrice = false,//استخدام آخر سعر شراء للمورد
                    Purchases_PriceIncludeVat = false,//السعر يشمل الضريبة
                    Purchases_PrintWithSave = false,//الطباعة عند الحفظ
                    Purchases_ReturnWithoutQuantity = false,//استرجاع بدون رصيد
                    Purchases_ActiveDiscount = true,//تفعيل الخصم
                    Purchase_UpdateItemsPricesAfterInvoice=false, // تحديث أسعار المنتجات بعد فاتورة المشتريات
                    //نقاط البيع
                    Pos_ModifyPrices = true,//السماح بتعديل أسعار البيع
                    //اسم غير واضح عايز يتراجع
                    Pos_ModifyPricesType = (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4,//السماح بتجاوز نسبة البيع

                    Pos_ExceedDiscountRatio = true,//السماح بتجاوز نسبة الخصم
                    //Pos_UseLastPrice = false,//استخدام أخر سعر بيع للعميل
                    Pos_ActivePricesList = false,//تنشيط قوائم الأسعار
                    Pos_ExtractWithoutQuantity = false,//الصرف بدون رصيد
                    Pos_PriceIncludeVat = false,//السعر يشمل الضريبة
                    Pos_ActiveDiscount = true,//تفعيل الخصم
                    Pos_DeferredSale = false,//البيع بالآجل
                    Pos_IndividualCoding = true,//ترقيم مستقل
                    Pos_PreventEditingRecieptFlag = false,//منع تعديل الفاتورة
                    Pos_PreventEditingRecieptValue = 2,//عدد الدقائق اللازمة لمنع تعديل الفاتورة
                    Pos_ActiveCashierCustody = false,//تفعيل عهدة الكاشير
                    Pos_PrintPreview = false,//معاينة قبل الطباعة
                    Pos_PrintWithEnding = false,//الطباعة عند إنهاء الطلب
                    Pos_EditingOnDate = false,//التعديل على التاريخ

                    // المبيعات
                    Sales_ModifyPrices = true,//السماح بتعديل أسعار البيع
                    Sales_ModifyPricesType = (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4,//السماح بتجاوز أسعار البيع
                    Sales_ExceedDiscountRatio = true,// السماح بتجاوز نسبة الخصم
                    Sales_PayTotalNet = false,// يجب أن تسدد قيمة المستند كاملة
                  //  Sales_UseLastPrice = false,//استخدام آخر سعر بيع للعميل
                    Sales_ExtractWithoutQuantity = false,//الصرف بدون رصيد
                    Sales_PriceIncludeVat = false,//السعر يشمل الضريبة
                    Sales_PrintWithSave = false,// الطباعة عند الحفظ
                    Sales_ActiveDiscount = true,//تفعيل الخصم
                    Sales_LinkRepresentCustomer = true,//ربط المندوب بالعميل
                    Sales_ActivePricesList = false,//تنشيط قوائم الأسعار


                    //أخرى
                    Other_MergeItems = true,//دمج الأصناف
                    otherMergeItemMethod = "withSave",//دمج مع الحفظ
                                                      //دمج مع الادخال
                    Other_ItemsAutoCoding = false,//تكويد آلي للأصناف
                    Other_ZeroPricesInItems = false,//السماح بالأسعار الصفرية بكارت الصنف
                    Other_PrintSerials = true, //طباعة سيريالات الأصناف
                    Other_AutoExtractExpireDate = false,//الصرف الآلي لتاريخ الصلاحية
                    Other_ViewStorePlace = false,//إظهار مكان التخزين في المبيعات ونقاط البيع
                    Other_ConfirmeSupplierPhone = false,//التأكيد من تسجيل رقم الهاتف للموردين
                    Other_ConfirmeCustomerPhone = false,//التأكد من تسجيل رقم الهاتف للعملاء
                    Other_DemandLimitNotification = true,//التنبيه بحد الطلب
                    Other_ExpireNotificationFlag = true,//تنبيه انتهاء الصلاحية
                    Other_ExpireNotificationValue = 30,//عدد أيام التنبيه على انتهاء الصلاحية
                    Other_Decimals = 2,//العلامات العشرية
                    Other_ShowBalanceOfPerson =false,// اظهار رصيد الموردين والعملاء فى الفواتير
                    other_ActivatePriceLists=false,
                    PriceListType = (int)PriceListsType.PriceLists,
                    // الأرصدة
                    Funds_Items = false,//إغلاق أرصدة أول مدة أصناف
                    Funds_Supplires = false,//إغلاق أرصدة أول المدة موردين
                    Funds_Customers = false,//إغلاق أرصدة أول المدة عملاء
                    Funds_Safes = false,//إغلاق أرصدة أول المدة خزائن
                    Funds_Banks = false,//إغلاق أرصدة أول المدة بنوك
                    Funds_Customers_Date = null,
                    Funds_Supplires_Date = null,

                    //الباركود
                    barcodeType =ScalbarcodeType.Weight ,//نوع الميران وزن
                                           // نوع الميزان تكلفة
                    Barcode_ItemCodestart = false,//عدم وجود رقم مبدئي

                    //القيمة المضافة
                    Vat_Active = true,//تفعيل الضريبة
                    Vat_DefaultValue = 15,//القيمة الافتراضية للضريبة

                    // الاعتماد
                    Accredite_StartPeriod = Convert.ToDateTime(DateTime.Now.Year + "/1/1"),//بداية الفترة المالية
                    Accredite_EndPeriod = Convert.ToDateTime(DateTime.Now.Year + "/12/31"),//نهاية الفترة المالية

                    // واجهة العملاء 
                    CustomerDisplay_Active = true,//تنشيط واجهة العملاء
                    CustomerDisplay_PortNumber = "1",//رقم البورت
                    CustomerDisplay_Code = 9600,//الكود
                    CustomerDisplay_LinesNumber = 2,//عدد السطور
                    CustomerDisplay_CharNumber = 20,//عدد الحروف
                    CustomerDisplay_DefaultWord = "welcome",//الكلمة الافتتاحية
                    CustomerDisplay_ScreenType = (int)DocumentType.POS,//نوع الشاشة
                    isExpenses = false,// مصروفات

                }
            });
            return list.ToArray();
        }
        public InvSalesMan[] setInvSalesMan()
        {
            var list = new List<InvSalesMan>();
            list.AddRange(new[]
            {
                new InvSalesMan
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "مندوب مبيعات افتراضي",
                    LatinName = "Default salesman",
                    Phone = "",
                    Email = "",
                    ApplyToCommissions = false,
                    CommissionListId = null,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.salesMan,
                    SalesPriceId=(int)SalePricesList.SalePrice1
                }
            });
            return list.ToArray();
        }
        public InvPersons[] SetInvPersons()
        {
            var list = new List<InvPersons>();
            list.AddRange(new[]
            {
                new InvPersons
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "مورد نقدي",
                    LatinName = "Cash Supplier",
                    Type = (int)PersonType.Normal,
                    Status = (int)Status.Active,
                    SalesManId = null,
                    ResponsibleAr = "",
                    ResponsibleEn = "",
                    Phone = "",
                    Fax = "",
                    Email = "",
                    TaxNumber = "",
                    AddressAr = "",
                    AddressEn = "",
                    //CreditLimit = 0,
                    //CreditPeriod = 0,
                    //DiscountRatio = 0,
                    //SalesPriceId = 0,
                    //LessSalesPriceId = 0,
                    IsCustomer = false,
                    IsSupplier = true,
                    CanDelete = false,
                    AddToAnotherList = false,
                    CodeT = "S",
                    FinancialAccountId = (int)FinanicalAccountDefultIds.cashSuppliers,
                    InvEmployeesId = 1
                },
                new InvPersons
                {
                    Id = 2,
                    Code = 1,
                    ArabicName = "عميل نقدي",
                    LatinName = "Cash Customer",
                    Type = (int)PersonType.Normal,
                    Status = (int)Status.Active,
                    SalesManId = 1,
                    ResponsibleAr = "",
                    ResponsibleEn = "",
                    Phone = "",
                    Fax = "",
                    Email = "",
                    TaxNumber = "",
                    AddressAr = "",
                    AddressEn = "",
                    //CreditLimit = 0,
                    //CreditPeriod = 0,
                    //DiscountRatio = 0,
                    //SalesPriceId = (int)SalePricesList.SalePrice1,
                    //LessSalesPriceId = (int)SalePricesList.SalePrice1,
                    IsCustomer = true,
                    IsSupplier = false,
                    CanDelete = false,
                    AddToAnotherList = false,
                    CodeT = "C",
                    FinancialAccountId = (int)FinanicalAccountDefultIds.cashCustomer,
                    InvEmployeesId = 1,
                    SalesPriceId=(int)SalePricesList.SalePrice1
                }
            });
            return list.ToArray();
        }
        public InvPersons_Branches[] setInvPersons_Branches()
        {
            var list = new List<InvPersons_Branches>();
            list.AddRange(new[]
            {
                new InvPersons_Branches
                {
                    BranchId = 1,
                    PersonId = 1
                },
                new InvPersons_Branches
                {
                    BranchId = 1,
                    PersonId = 2
                }
            });
            return list.ToArray();
        }
        public InvFundsCustomerSupplier[] setInvFundsCustomerSupplier()
        {
            var list = new List<InvFundsCustomerSupplier>();
            list.AddRange(new[]
            {
                new InvFundsCustomerSupplier
                {
                    Id = 1,
                    PersonId = 1,
                    Credit = 0,
                    Debit = 0,
                },
                new InvFundsCustomerSupplier
                {
                    Id = 2,
                    PersonId = 2,
                    Credit = 0,
                    Debit = 0,
                }
            });
            return list.ToArray();
        }
        public InvPaymentMethods[] setInvPaymentMethods()
        {
            var list = new List<InvPaymentMethods>();
            list.AddRange(new[]
            {
                new InvPaymentMethods
                {
                    PaymentMethodId = (int)PaymentMethod.Transfer,
                    Code = (int)PaymentMethod.Transfer,
                    ArabicName = "تحويل",
                    LatinName = "Transfer",
                    SafeId = null,
                    BankId = null,
                    Status = (int)Status.Inactive,
                },new InvPaymentMethods
                {
                    PaymentMethodId = (int)PaymentMethod.PersonBalance,
                    Code = (int)PaymentMethod.PersonBalance,
                    ArabicName = "السحب من رصيد المستفيد",
                    LatinName = "Withdrawal from the beneficiarys balance",
                    SafeId = null,
                    BankId = null,
                    Status = (int)Status.Active,
                },
                new InvPaymentMethods
                {
                    PaymentMethodId = 1,
                    Code = 1,
                    ArabicName = "نقدي",
                    LatinName = "Cash",
                    SafeId = 1,
                    BankId = null,
                    Status = (int)Status.Active,
                },
                new InvPaymentMethods
                {
                    PaymentMethodId = 2,
                    Code = 2,
                    ArabicName = "شبكة",
                    LatinName = "Net",
                    SafeId = null,
                    BankId = 1,
                    Status = (int)Status.Active,
                },
                new InvPaymentMethods
                {
                    PaymentMethodId = 3,
                    Code = 3,
                    ArabicName = "شيك",
                    LatinName = "Chique",
                    SafeId = 1,
                    BankId = null,
                    Status = (int)Status.Active,
                }
            });
            return list.ToArray();
        }
   
        public InvStpItemCardMaster[] setInvStpItemCardMaster()
        {
            var list = new List<InvStpItemCardMaster>();
            list.AddRange(new[]
            {
               new InvStpItemCardMaster
               {
                   Id = 1,
                   ItemCode = "1",
                   LatinName = "كارت الصنف الافتراضى",
                   ArabicName = "كارت الصنف الافتراضى",
                   GroupId = 1,
                   Status = 1,
                   UsedInSales = true,
                   TypeId = 1,
                   ApplyVAT = true,
                   VAT = 15,
                   BranchId = 1,
                   ReportUnit = 1,
                   WithdrawUnit = 1,
                   DepositeUnit = 1
               }
            });
            return list.ToArray();
        }
        public InvStpItemCardUnit[] setInvStpItemCardUnit()
        {
            var list = new List<InvStpItemCardUnit>();
            list.AddRange(new[]
            {
                new InvStpItemCardUnit
                {
                    Barcode = "",
                    ConversionFactor = 1,
                    ItemId = 1,
                    PurchasePrice = 10,
                    SalePrice1 = 10,
                    SalePrice2 = 10,
                    SalePrice3 = 10,
                    SalePrice4 = 10,
                    UnitId = 1,
                }
            });
            return list.ToArray();
        }
        public InvCompanyData[] setInvCompanyData()
        {
            var list = new List<InvCompanyData>();
            list.AddRange(new[]
            {
                new InvCompanyData
                {
                    Id =1,
                    ArabicName = "اسم الشركة الافتراضي",
                    LatinName = "Default company name",
                    ArabicAddress = "",
                    CommercialRegister = "",
                    Email ="",
                    Fax ="",
                    FieldAr = "",
                    FieldEn = "",
                    Image = "",
                    imageFile = null,
                    LatinAddress = "",
                    Notes = "",
                    Phone1 = "",
                    Phone2 = "",
                    TaxNumber = "",
                    Website = ""
                }
            });
            return list.ToArray();
        }
        public InvBarcodeTemplate[] setInvBarcodeTemplate()
        {
            var list = new List<InvBarcodeTemplate>();
            list.AddRange(new[]
            {
                new InvBarcodeTemplate
                {
                    BarcodeId = 1,
                    ArabicName = "باركود افتراضي",
                    LatinName = "Default Barcode",
                    Code = 1,
                    IsDefault = true,
                    CanDelete = false,
                }
            });
            return list.ToArray();
        }
        public InvBarcodeItems[] setInvBarcodeItems()
        {
            var list = new List<InvBarcodeItems>();
            list.AddRange(new[]
            {
                new InvBarcodeItems
                {
                    Id = 1,
                    BarcodeId = 1,
                    BarcodeItemType = (int)BarcodeItemType.Container,
                    Width = 300,
                    Height = 300,
                    PositionX = 10,
                    PositionY = 10,
                    FontType = "",
                    FontSize = 16,
                    Bold = false,
                    Italic = false,
                    UnderLine = false,
                    AlignX = "center",
                    AlignY = "center",
                    Dock = "right",
                    Image = null,
                    ImageName = null,
                    IsLogo = false,
                    Direction = 10,
                    TextType = 0,
                    BeginSplitter = "",
                    EndSplitter = "",
                    BarcodeType = (int)BarcodeType.Code128
                }
            });
            return list.ToArray();
        }
        public GLPurchasesAndSalesSettings[] setGLPurchasesAndSalesSettings()
        {
            //var list = defultData.getlistOfGLPurchasesAndSalesSettingsTable();
            var list = defultData.New_getlistOfGLPurchasesAndSalesSettingsTable(); // uncomment for create databasSCript Code = #011144477
            
            return list.ToArray();
        }
        public SubCodeLevels[] setSubCodeLevels()
        {
            var list = new List<SubCodeLevels>();
            list.AddRange(new[]
            {
                new SubCodeLevels
                {
                    Id = 1,
                    GLGeneralSettingId = 1,
                    value = 2
                },
                new SubCodeLevels
                {
                    Id = 2,
                    GLGeneralSettingId = 1,
                    value = 4
                },
                new SubCodeLevels
                {
                    Id = 3,
                    GLGeneralSettingId = 1,
                    value = 4
                },
                new SubCodeLevels
                {
                    Id = 4,
                    GLGeneralSettingId = 1,
                    value = 4
                },
                new SubCodeLevels
                {
                    Id = 5,
                    GLGeneralSettingId = 1,
                    value = 4
                }
            });
            return list.ToArray();
        }
        public InvJobs[] setInvJobs()
        {
            var jobs = new List<InvJobs>();
            jobs.AddRange(new[]
            {
                new InvJobs
                {
                    Id = 1,
                    Code = 1,
                    ArabicName = "مدير النظام",
                    LatinName = "Administrator",
                    Status = (int)Status.Active,
                },
                new InvJobs
                {
                    Id = 2,
                    Code = 3,
                    ArabicName = "كاشير",
                    LatinName = "Casher",
                    Status = (int)Status.Active,
                }
            });
            return jobs.ToArray();
        }
        public GLOtherAuthorities[] setGLOtherAuthorities()
        {
            var list = new List<GLOtherAuthorities>();
            list.AddRange(new[]
            {
                new GLOtherAuthorities
                {
                    Id = -1,
                    ArabicName = "ضريبةالقيمة المضافة",
                    LatinName = "Vat",
                    Code = 1,
                    BranchId = 1,
                    CanDelete = false,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.TaxesOwedSettlement,
                    Status = 1
                },
                new GLOtherAuthorities
                {
                    Id = 1,
                    ArabicName = "الكهرباء",
                    LatinName = "Electricity bill",
                    Code = 2,
                    BranchId = 1,
                    CanDelete = true,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.ElectricBill,
                    Status = 1
                },
                new GLOtherAuthorities
                {
                    Id = 2,
                    ArabicName = "مياه",
                    LatinName = "Water bill",
                    Code = 3,
                    BranchId = 1,
                    CanDelete = true,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.WaterBill,
                    Status = 1
                }
            });
            return list.ToArray();
        }
        public InvSalesMan_Branches[] setInvSalesMan_Branches()
        {
            var list = new List<InvSalesMan_Branches>();
            list.AddRange(new[]
            {
                new InvSalesMan_Branches
                {
                    BranchId =1,
                    SalesManId = 1
                }
            });
            return list.ToArray();
        }
        public ScreenName[] setScreenName()
        {
            var list = new List<ScreenName>();

            
            var screens = returnSubForms.returnRules().Select(x => new ScreenName
            {
                Id = x.subFormCode,
                ScreenNameAr = x.arabicName,
                ScreenNameEn = x.latinName
            });
            list.AddRange(screens);
            return list.ToArray();

        }
        public GLIntegrationSettings[] setGLIntegrationSettings()
        {
            var list = new List<GLIntegrationSettings>();
            list.AddRange(new[]
            {
                new GLIntegrationSettings
                {
                    Id = 1,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Safes_MainData,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.Cashinthesafe,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,                    
                },
                new GLIntegrationSettings
                {
                    Id = 2,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Banks_MainData,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.cashinthebank,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,
                },
                new GLIntegrationSettings
                {
                    Id = 3,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Suppliers_Purchases,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.Suppliers,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,
                },
                new GLIntegrationSettings
                {
                    Id = 4,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Customers_Sales,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.Customers,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,
                },
                new GLIntegrationSettings
                {
                    Id = 5,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Employees_MainUnits,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.Employees,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,
                },
                new GLIntegrationSettings
                {
                    Id = 6,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.Salesmen_Sales,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.salesMan,
                    linkingMethodId = (int)FinancialAccountRelationSettings.ChooseoneAccountTobeDefult,
                },
                new GLIntegrationSettings
                {
                    Id = 7,
                    GLBranchId= 1,
                    screenId = (int)SubFormsIds.OtherAuthorities_MainData,
                    GLFinancialAccountId = (int)FinanicalAccountDefultIds.OtherAuthorities,
                    linkingMethodId = (int)FinancialAccountRelationSettings.CreateAccountAutomaticWithTheParentAccountId,
                }
            });

            return list.ToArray();
        }

        public GLCostCenter[] setGLCostCenter()
        {
            var list = new List<GLCostCenter>();
            list.AddRange(new[]
            {
                new GLCostCenter
                {
                    Id = 1,
                    ArabicName = "مراكز التكلفة",
                    LatinName = "Cost Center",
                    BranchId = 1,
                    CC_Nature =1 ,
                    Type = 2,
                    AutoCode = "1"
                }
            });
            return list.ToArray();
        }
    }
}
