using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GEOBOX.OSC.IM.DataCheckerTool
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> AsHierarchy<T>(this IEnumerable<T> collection,
            Func<T, T> parentSelector,
            Expression<Func<T, IEnumerable<T>>> childrenSelector, T root = default(T))
        {
            collection = (collection ?? Enumerable.Empty<T>()).ToArray();
            var items = collection.Where(x => parentSelector(x).Equals(root)).ToArray();
            foreach (var item in items)
            {
                var childrenProperty = (childrenSelector.Body as MemberExpression)?.Member as PropertyInfo;
                childrenProperty?.SetValue(item, collection.AsHierarchy(parentSelector, childrenSelector, item), null);
            }
            return items;
        }
    }
}
