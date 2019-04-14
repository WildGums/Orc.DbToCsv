// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCacheHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Automation.Peers;
    using Catel.Caching;
    using Catel.Reflection;
    using DatabaseManagement;

    internal static class TypeCacheHelper
    {
        private static readonly CacheStorage<Type, CacheStorage<Type, HashSet<Type>>> TypeCache = new CacheStorage<Type, CacheStorage<Type, HashSet<Type>>>();

        public static IList<Type> GetTypesByGenericArgument(Type genericType, Type genericTypeArgument)
        {
            if (!TypeCache.Contains(genericType))
            {
                InitializeGenericTypeCache(genericType);
            }

            var genericTypeCache = TypeCache[genericType];
            return genericTypeCache.Contains(genericTypeArgument) ? genericTypeCache[genericTypeArgument].ToList() : new List<Type>();
        }

        public static void InitializeGenericTypeCache(Type genericType)
        {
            var genericTypeDescendantsCache = TypeCache.GetFromCacheOrFetch(genericType, () => new CacheStorage<Type, HashSet<Type>>());

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var loadedAssembly in loadedAssemblies)
            {
                try
                {
                    var genericTypeDescendants = loadedAssembly.GetTypesEx()
                        .Where(x => typeof(DbProvider)
                                        .IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && !x.IsGenericType)
                                        .ToList();
                    foreach (var genericTypeDescendant in genericTypeDescendants)
                    {
                        var summaryGenericInterfaceType = genericTypeDescendant.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
                        var argumentType = summaryGenericInterfaceType?.GetGenericArguments()[0];
                        if (argumentType == null)
                        {
                            continue;
                        }
                        
                        var cachedTypes = genericTypeDescendantsCache.GetFromCacheOrFetch(argumentType, () => new HashSet<Type>());
                        cachedTypes.Add(genericTypeDescendant);
                    }
                }
                catch (Exception)
                {
                    //do not handle 
                }
            }
        }
    }
}
