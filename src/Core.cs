using PasswordGenerator;
using TextCopy;
using Vault.models;
using Vault.src;
using Amazon.IdentityManagement;

namespace Vault
{
    internal class Core
    {
        private string _input;
        private bool _end;
        private string _vaultDataFilePath;
        private string _fileName;
        private FileManager _fileManager;
        private EntryManager _entryManager;
        private S3Provider _s3;
        private AmazonIdentityManagementServiceClient _iam;

        public Core()
        {
            _iam = new AmazonIdentityManagementServiceClient(region: Amazon.RegionEndpoint.EUCentral1);
            _fileName = $"{_iam.GetUserAsync().Result.User.UserId}-VAULT";
            _end = false;
            _input = "";
            _vaultDataFilePath = $"C:\\Users\\{Environment.UserName}\\.vault\\{_fileName}";
            _fileManager = new FileManager();
            _entryManager = new EntryManager();
            _s3 = new S3Provider(_vaultDataFilePath, _fileName);

            _s3.LoadData();
        }

        public void Execute()
        {
            ConsoleManager.ShowMessage("##################################################");
            ConsoleManager.ShowMessage("Welcome to the Vault, to list commands type \'help\'");
            ConsoleManager.ShowMessage("##################################################");

            _entryManager.AddEntryList(_fileManager.LoadEntries(_vaultDataFilePath));

            while (!_end)
            {
                _input = ConsoleManager.GetInput();

                CheckCommand();
            }
        }

        private void CheckCommand()
        {
            switch (_input)
            {
                case Command.END_PROGRAM:
                    {
                        EndProgram();
                    }
                    break;
                case Command.GENERATE_DATA:
                    {
                        GenerateData();
                    }
                    break;
                case Command.SAVE_DATA:
                    {
                        SaveData();
                    }
                    break;
                case Command.HELP:
                    {
                        Help();
                    }
                    break;
                case Command.LIST_DATA:
                    {
                        ListData();
                    }
                    break;
                case Command.GET_DATA:
                    {
                        GetData();
                    }
                    break;
                default:
                    {
                        Default();
                    }
                    break;
            }
            ConsoleManager.ShowMessage("\nDone!");
        }

        private void EndProgram()
        {
            _end = true;
        }

        private void GenerateData()
        {
            var pwd = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true);

            pwd.LengthRequired(8);
            ConsoleManager.ShowMessage(pwd.Next());

            pwd.LengthRequired(16);
            ConsoleManager.ShowMessage(pwd.Next());

            pwd.LengthRequired(24);
            ConsoleManager.ShowMessage(pwd.Next());

            pwd.LengthRequired(32);
            ConsoleManager.ShowMessage(pwd.Next());

            pwd.LengthRequired(48);
            ConsoleManager.ShowMessage(pwd.Next());
        }

        private void SaveData()
        {
            int passLenght = 12;
            ClipboardService.SetText(
               (new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true)
                .LengthRequired(passLenght).Next()));

            ConsoleManager.ShowMessage($"Example password has been copied to the clipboard ({passLenght})");
            ConsoleManager.ShowMessage("Insert key...");
            var key = ConsoleManager.GetInput().ToLower();

            ConsoleManager.ShowMessage("Insert data...");
            var data = ConsoleManager.GetInput();

            int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            Entry entry = new(data, key, timestamp);

            _fileManager.SaveEntry(_vaultDataFilePath, entry);

            _s3.SaveData();

            _entryManager.AddEntry(entry);
        }

        private void Help()
        {
            ConsoleManager.ShowMessage("save - allows to save the data");
            ConsoleManager.ShowMessage("generate - generates passwords");
            ConsoleManager.ShowMessage("end - closes the program");
            ConsoleManager.ShowMessage("list - shows all entries");
            ConsoleManager.ShowMessage("get - shows data associated with inserted key");
        }

        private void Default()
        {
            ConsoleManager.ShowMessage("Unknown command");
        }

        private void ListData()
        {
            foreach (var entry in _entryManager.GetEntryList())
            {
                ConsoleManager.ShowMessage(entry.Key);
            }
        }

        private void GetData()
        {
            ConsoleManager.ShowMessage("Insert a value you're looking for");

            string text = ConsoleManager.GetInput();

            List<ComplexEntry> entries = new List<ComplexEntry>(_entryManager.GetEntryList().Where(x => x.Key.Contains(text, StringComparison.OrdinalIgnoreCase)));



            if (entries.Count > 1)
            {
                foreach (var entry in entries)
                {
                    if (entry.Key == text)
                    {
                        ClipboardService.SetText(entry.Data[entry.Data.Count - 1]);
                        ConsoleManager.ShowMessage($"Newest value from {entry.Key} saved in clipboard");
                        ConsoleManager.ShowMessage($"To show sensitve data type: {entry.Key}");

                        string input = ConsoleManager.GetInput();

                        if (input == entry.Key)
                        {
                            for (int i = 0; i < entry.Data.Count; i++)
                            {
                                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                                dateTime = dateTime.AddSeconds(entry.Timestamp[i]).ToLocalTime();
                                ConsoleManager.ShowMessage($"{i + 1}.{entry.Key} : {entry.Data[i]} : {dateTime}");
                            }
                            return;
                        }
                    }
                }

                ConsoleManager.ShowMessage("Your input is associated with more than one key");

                foreach (var entry in entries)
                {
                    ConsoleManager.ShowMessage(entry.Key);
                }
            }
            else if (entries.Count == 1)
            {
                foreach (var entry in entries)
                {          
                    ClipboardService.SetText(entry.Data[entry.Data.Count - 1]);
                    ConsoleManager.ShowMessage($"Newest value from {entry.Key} saved in clipboard");
                    ConsoleManager.ShowMessage($"To show sensitve data type: {entry.Key}");

                    string input = ConsoleManager.GetInput();

                    if (input == entry.Key)
                    {
                        for (int i = 0; i < entry.Data.Count; i++)
                        {
                            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                            dateTime = dateTime.AddSeconds(entry.Timestamp[i]).ToLocalTime();
                            ConsoleManager.ShowMessage($"{i+1}.{entry.Key} : {entry.Data[i]} : {dateTime}");
                        }
                    }
                }
            }
            else
            {
                ConsoleManager.ShowMessage("Cannot find result");
            }
        } 
    }
}
