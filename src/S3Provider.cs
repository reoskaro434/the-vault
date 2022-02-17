using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.src
{
    class S3Provider
    {
        private AmazonS3Client _client;
        private string _filePath;
        private string _fileName;


        public S3Provider(string filePath, string fileName)
        {
            _client = new AmazonS3Client(region: Amazon.RegionEndpoint.EUCentral1);
            _fileName = fileName;
            _filePath = filePath;
        }

        public bool SaveData()
        {
            var request = new PutObjectRequest()
            {
                BucketName = "vault-data-storage",
                Key = "vault-data",
                FilePath = _filePath
            };
            try
            {
                _client.PutObjectAsync(request).Wait();

                return true;
            }
            catch (Exception ex)
            {
                ConsoleManager.ShowMessage(ex.Message);
                return false;
            }
        }

        public bool LoadData()
        {
            try
            {

             GetObjectResponse response = _client.GetObjectAsync("vault-data-storage", _fileName).GetAwaiter().GetResult();
                MemoryStream ms = new MemoryStream();
             using (Stream stream = response.ResponseStream)
                {
                    stream.CopyTo(ms);
      
                }

                File.WriteAllBytes(_filePath, ms.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                ConsoleManager.ShowMessage(ex.Message);
                return false;
            }
        }
    }
}
