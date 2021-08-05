using System;
using System.Collections.Generic;

namespace Redemption.Tags
{
    public static class TagExtensions
    {
        public static void Populate(this TagData tag, int length, Func<int, bool> predicate)
        {
            for (int i = 0; i < length; i++)
                if (predicate(i))
                    tag.Set(i, true);
        }

        public static void PopulateFromSets(this TagData tag, bool[] sets)
        {
            for (int i = 0; i < sets.Length; i++)
                if (sets[i])
                    tag.Set(i, true);
        }

        public static void SetMultiple(this TagData tag, params int[] ids) => tag.SetMultiple((IReadOnlyList<int>) ids);

        public static void SetMultiple(this TagData tag, IReadOnlyList<int> ids)
        {
            foreach (int id in ids)
                tag.Set(id, true);
        }
    }
}