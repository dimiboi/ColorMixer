using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace ColorMixer.Extensions
{
    public static class ReactiveListExtensions
    {
        public static void RemoveRange<T>(this IReactiveList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items.ToArray())
            {
                list.Remove(item);
            }
        }
    }
}