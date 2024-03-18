using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Exceptions
{
    [Serializable]
    public class DocumentApiBaseException : Exception
    {
        public DocumentApiBaseException()
        {
        }

        public DocumentApiBaseException(string message) : base(message)
        {
        }

        public DocumentApiBaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
