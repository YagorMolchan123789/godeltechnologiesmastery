using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Common.Exceptions
{
    [Serializable]
    public class DocumentApiValidationException : DocumentApiBaseException
    {
        public DocumentApiValidationException()
        {
        }

        public DocumentApiValidationException(string message) : base(message)
        {
        }

        public DocumentApiValidationException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
