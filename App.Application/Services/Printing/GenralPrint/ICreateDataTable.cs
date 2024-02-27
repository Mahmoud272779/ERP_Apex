using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.GenralPrint
{
    public interface ICreateDataTable
    {
        Task<List<DataTable>> CreateDataTables<T, T1, T2>(DataToCreateDataTable<T, T1, T2> data, TablesNames tablesNames, object otherData, bool isBarcode = false, List<object> obj = null);

    }
}
