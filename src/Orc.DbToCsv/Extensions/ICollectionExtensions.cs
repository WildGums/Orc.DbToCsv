// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ICollectionExtensions
    {
        #region Methods
        public static TTarget FindTypeOrCreateNew<T, TTarget>(this ICollection<T> collection, Func<TTarget> func)
            where TTarget : T
        {
            var result = collection.OfType<TTarget>().FirstOrDefault();
            if (result != null)
            {
                return result;
            }

            result = func();
            collection.Add(result);
            return result;
        }
        #endregion
    }
}
