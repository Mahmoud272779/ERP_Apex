using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace App.Application.Helpers.Service_helper.offlinePOS
{
    public  class convertDataTableToString
    {
        public static string convertDT_ToString(DataTable dt)
        {
            string res = "";
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                if (res != "")
                    res += ";";
                res += dt.Columns[k].ColumnName;
            }
            res += Environment.NewLine;
            res += string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(";", x.ItemArray)));
            return res;
        }
        
        public static DataTable convertStringToDataTable(string data ,bool GetList,ref List<int> storeList , ref List<int> itemList ,ref List<int> branchList , ref List<int> itemUnitsList)
        {
            if (data.Trim().Length == 0)
                return null;
            string btrn = "@#$#$^#$";
            data = Regex.Replace(data, @"\\r\\n?|\n", btrn);

            string[] arr = data.Split(new[] { btrn }, StringSplitOptions.None);
            // first index of that array is columnName 
            DataTable res = new DataTable();
            string[] colArr = arr[0].Split(';');
            foreach (string cname in colArr)
            {
                res.Columns.Add(cname.Trim());
            }
            for (int k = 1; k < arr.Length-1; k++)
            {
                res.Rows.Add();
                string[] dataArr = arr[k].Split(';');
                for (int j = 0; j < res.Columns.Count; j++)
                {
                    if (dataArr.Length > j)
                        res.Rows[k - 1][j] = dataArr[j].Trim();
                  
                }
                if (GetList)
                {
                    if (res.Columns.Contains("StoreId"))
                    {
                        storeList.Add(int.Parse(res.Rows[k - 1]["StoreId"].ToString()));
                        branchList.Add(int.Parse(res.Rows[k - 1]["BranchID"].ToString()));

                    }
                    else if (res.Columns.Contains("ItemId"))
                    { 
                            itemList.Add(int.Parse( res.Rows[k - 1]["ItemId"].ToString()));
                    //    itemUnitsList.Add(int.Parse( res.Rows[k - 1]["UnitId"].ToString()));

                    }
                   
                    
                }
            }
            return res;
        }
    }
}
