using System.Security.Cryptography;
using System.Text;

namespace Vault
{
    class VaultRSAProvider
    {
        private static RSACryptoServiceProvider ?_csp;
        private string _stringPublicKey;
        private string _stringPrivateKey;
        private string _directoryPath;
        private string _publicKeyPath;
        private string _privateKeyPath;
        private int _keySize;

        public VaultRSAProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
            _publicKeyPath = directoryPath + "\\vault_pub_key";
            _privateKeyPath = directoryPath + "\\vault_priv_key";
            _keySize = 8192; //1024, 2048, 4096, 8192
            _stringPrivateKey = "";
            _stringPublicKey = "";
            try
            {
                string publicKey = File.ReadAllText(_publicKeyPath);
                string privateKey = File.ReadAllText(_privateKeyPath);

                _stringPublicKey = publicKey;
                _stringPrivateKey = privateKey;

                ConsoleManager.ShowMessage("The Vault credentials has been obtained\n");
            } catch (Exception ex)
            {
                ConsoleManager.ShowMessage(ex.Message);

                ConsoleManager.ShowMessage("Cannot obtain ssh keys...");
                ConsoleManager.ShowMessage("Provide a valid shh keys if you want to use it to the following paths:");
                ConsoleManager.ShowMessage(_publicKeyPath);
                ConsoleManager.ShowMessage(_privateKeyPath);
                ConsoleManager.ShowMessage("Close the program and start again or hit enter if you want to create a new key pair");

                ConsoleManager.GetInput();

                ConsoleManager.ShowMessage("Auto generating a new key pair...");

                GenerateKeys();
            }
        }

        public string Encrypt(string text)
        {
            _csp = new RSACryptoServiceProvider();
            _csp.FromXmlString(_stringPublicKey);

            return Convert.ToBase64String(
                _csp.Encrypt(Encoding.Unicode.GetBytes(text), false));
        }

        public string Decrypt(string text)
        {
            _csp = new RSACryptoServiceProvider();
            _csp.FromXmlString(_stringPrivateKey);
            
            return Encoding.Unicode.GetString(
                _csp.Decrypt(Convert.FromBase64String(text), false));
        }

        private void GenerateKeys()
        {
            _csp = new RSACryptoServiceProvider(_keySize);

            _stringPrivateKey = _csp.ToXmlString(true);
            _stringPublicKey = _csp.ToXmlString(false);

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            ConsoleManager.ShowMessage("Saving key pairs...");

            File.WriteAllText(_publicKeyPath, _stringPublicKey);

            ConsoleManager.ShowMessage($"Done! {_publicKeyPath}");

            File.WriteAllText(_privateKeyPath, _stringPrivateKey);

            ConsoleManager.ShowMessage($"Done! {_privateKeyPath}\n");
        }
    }
}
