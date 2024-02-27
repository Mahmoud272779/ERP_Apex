using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Helpers
{
    public class Pagenation<T>
    {

        public static List<T> pagenationList(int PageSize, int PageNumber, List<T> list )
        {
            if (PageSize > 0 && PageNumber > 0)
            {
                list = list.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            }
            return list;
        }
        public static List<T> pagenationList(int PageSize, int PageNumber, IQueryable<T> list)
        {
            if (PageSize > 0 && PageNumber > 0)
            {
                list = list.Skip((PageNumber - 1) * PageSize).Take(PageSize);
            }
            return list.ToList();
        }

    }
}
