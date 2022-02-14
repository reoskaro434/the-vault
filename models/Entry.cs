namespace Vault.models
{
    class Entry
    {
        public string Data { get; set; }
        public string Key { get; set; }
        public int Timestamp { get; set; }
        public Entry(string data, string key, int timestamp)
        {
            Data = data;
            Timestamp = timestamp;
            Key = key;
        }

    }
}
