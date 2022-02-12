using System.Text;
using PasswordGenerator;
using TextCopy;

namespace Vault
{
    internal class Core
    {
        private string _input;

        private bool _end;

        private string _mainPath;

        private int _keySize;

        private string _vaultDataFilePath;

        private VaultRSAProvider _rsa;

        private SortedDictionary<string, string> _secrets;

        public Core()
        {
            _end = false;
            _mainPath = $"C:\\Users\\{Environment.UserName}\\.vault";
            _vaultDataFilePath = $"{_mainPath}\\vault_data";
            _keySize = 4096; //1024, 2048, 4096
            _rsa = new VaultRSAProvider(_mainPath, _keySize);
        }

        public void Execute()
        {
            Console.WriteLine("##################################################");
            Console.WriteLine("Welcome to the Vault, to list commands type \'help\'");
            Console.WriteLine("##################################################");

            while (!_end)
            {
                _input = GetInput();

                _input = _input.Split(" " , StringSplitOptions.RemoveEmptyEntries)[0];

                CheckCommand();
            }
        }

        private string GetInput()
        {
            Console.Write("\n>>");
            string? result = Console.ReadLine();

            return result == null ? "" : result;
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
            Console.WriteLine("\nDone!");
        }

        private void EndProgram()
        {
            _end = true;
        }

        private void GenerateData()
        {
            var pwd = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true);

            pwd.LengthRequired(8);
            Console.WriteLine(pwd.Next());

            pwd.LengthRequired(16);
            Console.WriteLine(pwd.Next());

            pwd.LengthRequired(24);
            Console.WriteLine(pwd.Next());

            pwd.LengthRequired(32);
            Console.WriteLine(pwd.Next());

            pwd.LengthRequired(48);
            Console.WriteLine(pwd.Next());
        }

        private void SaveData()
        {
            try
            {
                using (var stream = File.Open(_vaultDataFilePath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        var data = "";

                        Console.WriteLine("Insert key...");
                        data += GetInput();

                        data += ":";

                        Console.WriteLine("Insert data...");
                        data += GetInput();

                        data = _rsa.Encrypt(data);

                        data += ":";

                        writer.Write(Encoding.ASCII.GetBytes(data));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Help()
        {
            Console.WriteLine("save - allows to save the data");
            Console.WriteLine("generate - generates passwords");
            Console.WriteLine("end - closes the program");
            Console.WriteLine("list - shows all entries");
            Console.WriteLine("get - shows data associated with inserted key");
        }

        private void Default()
        {
            Console.WriteLine("Unknown command");
        }

        private void LoadData()
        {
            string data;

            try
            {
                byte[] byteData = File.ReadAllBytes(_vaultDataFilePath);

                data = Encoding.ASCII.GetString(byteData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                data = "";
            }

            string[] chunkedData = data.Split(":", StringSplitOptions.RemoveEmptyEntries);

            _secrets = new SortedDictionary<string, string>();

            foreach (string chunk in chunkedData)
            {
                string[] entry = _rsa.Decrypt(chunk).Split(":", StringSplitOptions.RemoveEmptyEntries);

                _secrets.Add(entry[0], entry[1]);
            }
        }

        private void ListData()
        {
            LoadData();

            foreach (var entry in _secrets)
            {
                Console.WriteLine(entry.Key);
            }
        }

        private void GetData()
        {
            LoadData();

            Console.WriteLine("Insert a value you're looking for");

            string text = GetInput();

            SortedDictionary<string, string> secrets = new SortedDictionary<string, string>(_secrets.Where(x => x.Key.Contains(text, StringComparison.OrdinalIgnoreCase)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

            if (secrets.Count > 1)
            {
                Console.WriteLine("Your input is associated with more than one key");

                foreach (var entry in secrets)
                {
                    Console.WriteLine(entry.Key);
                }
            } else if (secrets.Count == 1)
            {
                foreach (var entry in secrets)
                {
                    ClipboardService.SetText(entry.Value);
                    Console.WriteLine($"Value from {entry.Key} saved in clipboard");
                    Console.WriteLine($"To show sensitve data type: {entry.Key}");

                    string input = GetInput();

                    if (input == entry.Key)
                        Console.WriteLine($"{entry.Key} : {entry.Value}");
                }
            } else
            {
                Console.WriteLine("Cannot find result");
            }
        }
    }
}
