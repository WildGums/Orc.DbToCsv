﻿namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ICollectionExtensions
    {
        public static TTarget FindTypeOrCreateNew<T, TTarget>(this ICollection<T> collection, Func<TTarget> func)
            where TTarget : T
        {
            var result = collection.OfType<TTarget>().FirstOrDefault();
            if (result == null)
            {
                result = func();
                collection.Add(result);
            }
            return result;
        }
    }
}