using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageTutorial.Services
{
    public interface IAzureBlobFactory
    {
        Task<CloudBlobContainer> GetBlobContainer();
    }
}
