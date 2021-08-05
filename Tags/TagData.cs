using System.Collections.Generic;

namespace Redemption.Tags
{
    /// <summary> This class stores lists of content Ids associated with the tag that it represents. </summary>
    public sealed class TagData
    {
        private readonly bool[] _idToValue; //Accessed for quick HasTag checks.
        private readonly List<int> _entryList;
        private readonly IReadOnlyList<int> _readonlyEntryList; //Accessed for GetEntriesWithTag.

        internal TagData(int maxEntries)
        {
            _idToValue = new bool[maxEntries];
            _entryList = new List<int>();
            _readonlyEntryList = _entryList.AsReadOnly();
        }

        /// <summary> Returns whether or not the content piece with the Id has this tag. </summary>
        /// <param name="id"> The content id. </param>
        public bool Has(int id) => _idToValue[id];

        /// <summary> Returns a list of content Ids that have this tag. </summary>
        public IReadOnlyList<int> GetEntries() => _readonlyEntryList;

        /// <summary> Sets whether or not the content piece with the provided Id has this tag. </summary>
        /// <param name="id"> The content id. </param>
        /// <param name="value"> Whether or not the tag should be present for the provided content id. </param>
        public void Set(int id, bool value)
        {
            _idToValue[id] = value;

            if (!value)
                _entryList.Remove(id);
            else if (!_entryList.Contains(id)) _entryList.Add(id);
        }
    }
}