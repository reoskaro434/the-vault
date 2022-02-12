using System.Security.Cryptography;
using System.Text;

namespace Vault
{
    class VaultRSAProvider
    {
        private static RSACryptoServiceProvider ?_csp;

        private string ?_stringPublicKey;

        private string ?_stringPrivateKey;

        private int _keySize;

        private string _directoryPath;

        private string _publicKeyPath;

        private string _privateKeyPath;

        public VaultRSAProvider(string directoryPath, int keySize)
        {
            _keySize = keySize;
            _directoryPath = directoryPath;
            _publicKeyPath = directoryPath + "\\vault_pub_key";
            _privateKeyPath = directoryPath + "\\vault_priv_key";

            try
            {
                string publicKey = File.ReadAllText(_publicKeyPath);
                string privateKey = File.ReadAllText(_privateKeyPath);

                _stringPublicKey = publicKey;
                _stringPrivateKey = privateKey;

                Console.WriteLine("The Vault credentials obtained from the files\n");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine("Cannot obtain ssh keys...");
                Console.WriteLine("Provide a valid shh keys if you want to use it to the following paths:");
                Console.WriteLine(_publicKeyPath);
                Console.WriteLine(_privateKeyPath);
                Console.WriteLine("Auto generating a new key pair...");


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
            _csp = new RSACryptoServiceProvider(dwKeySize: _keySize);

            _stringPrivateKey = _csp.ToXmlString(true);
            _stringPublicKey = _csp.ToXmlString(false);

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            Console.WriteLine("Saving key pairs...");

            File.WriteAllText(_publicKeyPath, _stringPublicKey);

            Console.WriteLine($"Done! {_publicKeyPath}");

            File.WriteAllText(_privateKeyPath, _stringPrivateKey);

            Console.WriteLine($"Done! {_privateKeyPath}\n");
        }
    }
}
