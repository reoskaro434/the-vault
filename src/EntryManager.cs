using Vault.models;

namespace Vault.src
{
    class EntryManager
    {
        private List<ComplexEntry> _entries;

        public EntryManager()
        {
            _entries = new List<ComplexEntry>();
        }

        public void AddEntry(Entry entry)
        {
            int index = _entries.FindIndex(e => e.Key == entry.Key);

            if (index == -1)
            {
                _entries.Add(new ComplexEntry(entry.Data, entry.Key, entry.Timestamp));
            }
            else
            {
                _entries[index].Data.Add(entry.Data);
                _entries[index].Timestamp.Add(entry.Timestamp);
            }
        }

        public void AddEntryList(List<Entry> entries)
        {
            foreach (Entry entry in entries)
            {
                AddEntry(entry);
            }
        }

        public List<ComplexEntry> GetEntryList()
        {
            return _entries.OrderBy(e=>e.Key).ToList();
        }
    }
}
