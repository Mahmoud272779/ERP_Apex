using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.LinqExtensions
{
    public static class LinqExtensions 
    {
        public static IQueryable<TSource> TakeIfNotNull<TSource>(this IQueryable<TSource> source, int? count)
            //public static IQueryable<TResult> TakeIfNotNull<TResult>(this IEnumerable<TResult> source, int? count)
            {
                return !count.HasValue ? source : source.Take(count.Value);
            }
        
    }
}
