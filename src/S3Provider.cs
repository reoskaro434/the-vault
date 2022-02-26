using Amazon.S3;
using Amazon.S3.Model;

namespace Vault.src
{
    class S3Provider
    {
        string _bucketName = "the-v-storage";
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
                BucketName = _bucketName,
                Key = _fileName,
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
                var request = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = _fileName
                };
                GetObjectResponse response = _client.GetObjectAsync(request).GetAwaiter().GetResult();
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
                ConsoleManager.ShowMessage("Cannot download a specified file");
                ConsoleManager.ShowMessage(ex.Message);
                return false;
            }
        }
    }
}
