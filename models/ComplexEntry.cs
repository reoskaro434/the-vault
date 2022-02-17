namespace Vault.models
{
    class ComplexEntry
    {
        public List<string> Data { get; set; }
        public string Key { get; set; }
        public List<int> Timestamp { get; set; }
        public ComplexEntry(string data, string key, int timestamp)
        {
            Data = new List<string>();
            Data.Add(data);

            Timestamp = new List<int>();
            Timestamp.Add(timestamp);

            Key = key;

        }
    }
}
