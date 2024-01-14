using System.Collections;

namespace Nexify.Data.Helpers
{
    public static class PropertySorter
    {
        public static List<T> SortDescendingByProperty<T>(List<T> items, Func<T, DateTime> dateSelector)
        {
            items.Sort((item1, item2) => dateSelector(item2).CompareTo(dateSelector(item1)));
            return items;
        }

        public static List<T> SortAscendingBySubproperty<T>(IEnumerable<T> items, Func<T, DateTime> dateSelector, string subproperty)
        {
            foreach (var item in items)
            {
                var propertyInfo = item.GetType().GetProperty(subproperty);
                if (propertyInfo != null)
                {
                    var subItems = propertyInfo.GetValue(item) as IEnumerable<T>;
                    if (subItems != null && subItems.Any())
                    {
                        var sortedSubItems = subItems
                            .OrderBy(dateSelector)
                            .ToList();

                        propertyInfo.SetValue(item, sortedSubItems);
                    }
                }
            }
            return items.ToList();
        }

    }
}
