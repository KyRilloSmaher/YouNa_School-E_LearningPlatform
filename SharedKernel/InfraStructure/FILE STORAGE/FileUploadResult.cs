using SharedKernel.Domain.VALUE_OBJECTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.InfraStructure.FILE_STORAGE
{
    public sealed class FileUploadResult
    {
        public FileUrl Url { get; }
        public long Size { get; }

        public FileUploadResult(FileUrl url, long size)
        {
            Url = url;
            Size = size;
        }
    }
}
