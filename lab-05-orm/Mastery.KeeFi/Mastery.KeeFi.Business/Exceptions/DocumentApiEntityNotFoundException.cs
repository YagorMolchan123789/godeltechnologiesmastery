using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Exceptions
{
    [Serializable]
    public class DocumentApiEntityNotFoundException : DocumentApiBaseException
    {
        public DocumentApiEntityNotFoundException()
        {
        }

        public DocumentApiEntityNotFoundException(string message) : base(message)
        {
        }

        public DocumentApiEntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
