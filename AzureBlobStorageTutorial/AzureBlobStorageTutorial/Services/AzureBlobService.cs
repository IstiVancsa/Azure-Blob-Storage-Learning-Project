using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageTutorial.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IAzureBlobFactory _azureBlobFactory;
        public AzureBlobService(IAzureBlobFactory azureBlobFactory)
        {
            _azureBlobFactory = azureBlobFactory;
        }

        public async Task<IEnumerable<Uri>> GetBlobsAsync()
        {
            //due to it's singleton we can do this
            var container = await _azureBlobFactory.GetBlobContainer();
            // Gets all Cloud Block Blobs in the blobContainerName and passes them to teh view
            List<Uri> allBlobs = new List<Uri>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var response = await container.ListBlobsSegmentedAsync(blobContinuationToken);

                foreach (IListBlobItem blob in response.Results)
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        allBlobs.Add(blob.Uri);
                }
                blobContinuationToken = response.ContinuationToken;
            } while (blobContinuationToken != null);

            return allBlobs;
        }

        public async Task UploadAsync(IFormFileCollection files)
        {
            var container = await _azureBlobFactory.GetBlobContainer();

            foreach (var file in files)
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(GetRandomBlobName(file.FileName));
                await using (var stream = file.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }
            }
        }

        public async Task DeleteImageAsync(string fileUri)
        {
            var container = await _azureBlobFactory.GetBlobContainer();
            Uri uri = new Uri(fileUri);
            string filename = Path.GetFileName(uri.LocalPath);

            var blob = container.GetBlockBlobReference(filename);
            await blob.DeleteIfExistsAsync();
        }

        public async Task DeleteAllAsync()
        {
            var container = await _azureBlobFactory.GetBlobContainer();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var response = await container.ListBlobsSegmentedAsync(blobContinuationToken);

                foreach (IListBlobItem blob in response.Results)
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                }
                blobContinuationToken = response.ContinuationToken;
            } while (blobContinuationToken != null);
        }

        /// <summary> 
        /// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
        /// </summary> 
        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
