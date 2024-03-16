using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IClientsRepository ClientsRepository { get; }

        IDocumentsMetadataRepository DocumentsMetadataRepository { get; }

        void SaveChanges();

    }
}
