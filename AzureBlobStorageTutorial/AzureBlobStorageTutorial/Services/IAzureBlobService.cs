using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AzureBlobStorageTutorial.Services
{
    public interface IAzureBlobService
    {
        Task<IEnumerable<Uri>> GetBlobsAsync();
        Task UploadAsync(IFormFileCollection files);
        Task DeleteImageAsync(string fileUri);
        Task DeleteAllAsync();
    }
}
