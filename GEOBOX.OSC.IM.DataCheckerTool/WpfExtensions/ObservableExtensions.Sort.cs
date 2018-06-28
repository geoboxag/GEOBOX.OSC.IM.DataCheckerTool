using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GEOBOX.OSC.IM.DataCheckerTool.WpfExtensions
{
    public static class ObservableExtensions
    {
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, bool sortDescending = false)
        {
            if (source == null) return;

            var comparer = Comparer<TKey>.Default;

            for (var counter = source.Count - 1; counter >= 0; counter--)
            {
                for (var j = 1; j <= counter; j++)
                {
                    var o1 = source[j - 1];
                    var o2 = source[j];
                    var comparison = comparer.Compare(keySelector(o1), keySelector(o2));
                    if (sortDescending && comparison < 0)
                    {
                        source.Move(j, j - 1);
                    }
                    else if (!sortDescending && comparison > 0)
                    {
                        source.Move(j - 1, j);
                    }
                }
            }
        }
    }
}
