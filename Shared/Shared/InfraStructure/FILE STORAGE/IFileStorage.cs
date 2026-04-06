using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.InfraStructure.FILE_STORAGE
{
    public interface IFileStorage
    {
        Task<FileUploadResult> UploadAsync(
            Stream file,
            string fileName,
            StorageFileType fileType,
            CancellationToken cancellationToken = default);
    }
}
