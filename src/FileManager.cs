using Newtonsoft.Json;
using System.Text;
using Vault.models;

namespace Vault
{
    class FileManager
    {
        private VaultRSAProvider _rsa;
        private DriveHandler _driveHandler;

        public FileManager()
        {
            _driveHandler = new DriveHandler();
            //_rsa = new VaultRSAProvider($"{_driveHandler.getDriveInfo().Name}\\Users\\{Environment.UserName}\\.vault");
            _rsa = new VaultRSAProvider($"C:\\Users\\{Environment.UserName}\\.vault");
        }

        public void SaveEntry(string filePath, Entry entry)
        {
            //_driveHandler.scannForDrive();

            try
            {
                using (var stream = File.Open(filePath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        string serializedEntry = _rsa.Encrypt(JsonConvert.SerializeObject(entry)) + "\n";

                        writer.Write(Encoding.ASCII.GetBytes(serializedEntry));

                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleManager.ShowMessage(ex.Message);
            }
        }

        public List<Entry> LoadEntries(string filePath)
        {
            //_driveHandler.scannForDrive();

            string data;

            try
            {
                byte[] byteData = File.ReadAllBytes(filePath);

                data = Encoding.ASCII.GetString(byteData);
            }
            catch (Exception ex)
            {
                ConsoleManager.ShowMessage(ex.Message);

                data = "";
            }

            string[] chunkedData = data.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            List<Entry> entries = new List<Entry>();

            foreach (string chunk in chunkedData)
            {
                var xd = _rsa.Decrypt(chunk);
                Entry? entry = JsonConvert.DeserializeObject<Entry>(xd);

                if (entry != null)
                    entries.Add(entry);
            }
            return entries;
        }
    }
}
