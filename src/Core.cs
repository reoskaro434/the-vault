using PasswordGenerator;
using TextCopy;
using Vault.models;

namespace Vault
{
    internal class Core
    {
        private string _input;
        private bool _end;
        private string _vaultDataFilePath;
        private List<Entry> _entries;
        private FileManager _fileManager;

        public Core()
        {
            _end = false;
            _input = "";
            _entries = new List<Entry>();
            _vaultDataFilePath = $"C:\\Users\\{Environment.UserName}\\.vault\\vault_data";
            _fileManager = new FileManager();
        }

        public void Execute()
        {
            ConsoleManager.ShowMessage("##################################################");
            ConsoleManager.ShowMessage("Welcome to the Vault, to list commands type \'help\'");
            ConsoleManager.ShowMessage("##################################################");

            _entries = _fileManager.LoadEntries(_vaultDataFilePath);

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
             int passLenght = 24;

            ClipboardService.SetText(
               (new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true)
                .LengthRequired(passLenght).Next()));

            ConsoleManager.ShowMessage($"Example password has been copied to the clipboard ({passLenght})");

            ConsoleManager.ShowMessage("Insert key...");

            var key = ConsoleManager.GetInput();

            ConsoleManager.ShowMessage("Insert data...");

            var data = ConsoleManager.GetInput();

            var time = DateTime.Now;

            Entry entry = new(data, key, time);

            _fileManager.SaveEntry(_vaultDataFilePath, entry);

            _entries.Add(entry);
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
            foreach (var entry in _entries)
            {
                ConsoleManager.ShowMessage(entry.Key);
            }
        }

        private void GetData()
        {
            ConsoleManager.ShowMessage("Insert a value you're looking for");

            string text = ConsoleManager.GetInput();

            List<Entry> entries = new List<Entry>(_entries.Where(x => x.Key.Contains(text, StringComparison.OrdinalIgnoreCase)));

            if (entries.Count > 1)
            {
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
                    ClipboardService.SetText(entry.Data);
                    ConsoleManager.ShowMessage($"Value from {entry.Key} saved in clipboard");
                    ConsoleManager.ShowMessage($"To show sensitve data type: {entry.Key}");

                    string input = ConsoleManager.GetInput();

                    if (input == entry.Key)
                        ConsoleManager.ShowMessage($"{entry.Key} : {entry.Data}");
                }
            }
            else
            {
                ConsoleManager.ShowMessage("Cannot find result");
            }
        } 
    }
}
