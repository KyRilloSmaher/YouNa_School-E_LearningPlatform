using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.EXCEPTIONS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain.VALUE_OBJECTS
{
    public sealed class FileUrl : ValueObject
    {
        public string Value { get; }

        public FileUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("File URL cannot be empty.");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
