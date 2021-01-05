using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageTutorial.Services
{
    public class AzureBlobFactory : IAzureBlobFactory
    {
        private CloudBlobClient _cloudBlobClient = null;
        private CloudBlobContainer _cloudBlobContainer = null;
        private IConfiguration _configuration;

        public AzureBlobFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public async Task<CloudBlobContainer> GetBlobContainer()
        {
            if (_cloudBlobContainer == null)
            {
                if (_cloudBlobClient == null)
                    this.getClient();

                var containerName = _configuration.GetValue<string>("ContainerName");
                if (containerName == null)
                    throw new ArgumentException("Configuration must contain ContainerName");

                _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
                if (await _cloudBlobContainer.CreateIfNotExistsAsync())
                    await _cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            }

            return _cloudBlobContainer;
        }

        private CloudBlobClient getClient()
        {
            if (_cloudBlobClient == null)
            {
                var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");
                if (string.IsNullOrEmpty(storageConnectionString))
                {
                    throw new ArgumentException("Configuration must contain StorageConnectionString");
                }

                if (!CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
                {
                    throw new Exception("Could not create storage account with StorageConnectionString configuration");
                }
                _cloudBlobClient = storageAccount.CreateCloudBlobClient();
            }
            return _cloudBlobClient;
        }
    }
}
