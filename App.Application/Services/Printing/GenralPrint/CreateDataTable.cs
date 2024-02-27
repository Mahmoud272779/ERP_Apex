using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.GenralPrint
{
    public  class CreateDataTable:ICreateDataTable
    {
        private readonly ICompanyDataService _CompanyDataService;
        public CreateDataTable(ICompanyDataService companyDataService)
        {
            _CompanyDataService = companyDataService;
        }

            public  async Task<List<DataTable>> CreateDataTables<T, T1, T2>(DataToCreateDataTable<T, T1, T2> data, TablesNames tablesNames, object otherData, bool isBarcode = false,List<object> obj=null)
        {
            var companydata = await _CompanyDataService.GetCompanyData(true);

            Type myTypeCompany = companydata.Data.GetType();
            IList<PropertyInfo> propsCompany = new List<PropertyInfo>(myTypeCompany.GetProperties());


            DataTable CompanyTable = new DataTable("CompanyData");


            DataRow drCompany = CompanyTable.NewRow();
            List<DataTable> tables = new List<DataTable>();

            if (data.DataObjet != null)
            {
                Type mainDataType = data.DataObjet.GetType();
                IList<PropertyInfo> propsmainData = new List<PropertyInfo>(mainDataType.GetProperties());
                DataTable mainDataTable = new DataTable(tablesNames.ObjectName);
                DataRow drmainData = mainDataTable.NewRow();
                foreach (var Property in propsmainData)
                {

                    mainDataTable.Columns.Add(Property.Name);

                    drmainData[Property.Name] = Property.GetValue(data.DataObjet);

                }
                mainDataTable.Rows.Add(drmainData);
                // mainDataTable.TableName = tablesNames.ObjectName;
                tables.Add(mainDataTable);

            }
            if (obj != null)
            {
                if (obj.Count > 0)
                {
                    Type objType = obj[0].GetType();
                    IList<PropertyInfo> objPropsList = new List<PropertyInfo>(objType.GetProperties());
                    DataTable objTable = new DataTable("SalesTotals");
                    DataRow drlist = objTable.NewRow();
                    for (int i = 0; i < obj.Count(); i++)
                    {
                        foreach (var prop in objPropsList)
                        {
                            if (i == 0)
                            {
                                objTable.Columns.Add(prop.Name);
                            }


                            drlist[columnName: prop.Name] = prop.GetValue(obj[i]);

                        }
                        objTable.Rows.Add(drlist);
                        drlist = objTable.NewRow();
                    }
                    //firstListTable.TableName = tablesNames.FirstListName;
                    tables.Add(objTable);

                }

            }
            if (data.FirstList != null)
            {
                if (data.FirstList.Count > 0)
                {
                    Type firstListType = data.FirstList[0].GetType();
                    IList<PropertyInfo> firstPropsList = new List<PropertyInfo>(firstListType.GetProperties());
                    DataTable firstListTable = new DataTable(tablesNames.FirstListName);
                    DataRow drlist = firstListTable.NewRow();
                    for (int i = 0; i < data.FirstList.Count(); i++)
                    {
                        foreach (var prop in firstPropsList)
                        {
                            if (i == 0)
                            {
                                firstListTable.Columns.Add(prop.Name);
                            }


                            drlist[columnName: prop.Name] = prop.GetValue(data.FirstList[i]);

                        }
                        firstListTable.Rows.Add(drlist);
                        drlist = firstListTable.NewRow();
                    }
                    //firstListTable.TableName = tablesNames.FirstListName;
                    tables.Add(firstListTable);

                }
            }
            if (data.SecondList != null)
            {
                if (data.SecondList.Count > 0)
                {
                    Type secondListType = data.SecondList[0].GetType();
                    IList<PropertyInfo> secondPropsList = new List<PropertyInfo>(secondListType.GetProperties());
                    DataTable secondListTable = new DataTable(tablesNames.SecondListName);
                    DataRow drSecondlist = secondListTable.NewRow();
                    for (int i = 0; i < data.SecondList.Count(); i++)
                    {
                        foreach (var prop in secondPropsList)
                        {
                            if (i == 0)
                            {
                                secondListTable.Columns.Add(prop.Name);
                            }


                            drSecondlist[columnName: prop.Name] = prop.GetValue(data.SecondList[i]);

                        }
                        secondListTable.Rows.Add(drSecondlist);
                        drSecondlist = secondListTable.NewRow();
                    }
                    // secondListTable.TableName = tablesNames.SecondListName;
                    tables.Add(secondListTable);
                }
            }



            foreach (var Property in propsCompany)
            {
                CompanyTable.Columns.Add(Property.Name);

                if (Property.Name != "imageFile")
                {
                    var value = Property.GetValue(companydata.Data);
                    if (value == null)
                    {
                        drCompany[Property.Name] = "";
                    }
                    else
                    {


                        var columnData = Property.GetValue(companydata.Data).ToString();
                        if (columnData == "null")
                        {
                            drCompany[Property.Name] = "";
                        }
                        else
                            drCompany[Property.Name] = Property.GetValue(companydata.Data).ToString();

                    }
                }
            }
            CompanyTable.Rows.Add(drCompany);

            if (!isBarcode)
            {
                Type OtherDataType = otherData.GetType();

                IList<PropertyInfo> otherDataProps = new List<PropertyInfo>(OtherDataType.GetProperties());
                DataTable otherDataTable = new DataTable("ReportOtherData");
                DataRow drotherData = otherDataTable.NewRow();


                foreach (var Property in otherDataProps)
                {

                    otherDataTable.Columns.Add(Property.Name);

                    drotherData[Property.Name] = Property.GetValue(otherData);

                }
                otherDataTable.Rows.Add(drotherData);
                //otherDataTable.TableName = "ReportOtherData";
                tables.Add(otherDataTable);


            }
            tables.Add(CompanyTable);
            return tables;
        }

    }

}
