using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IClientsRepository Clients { get; }

        IDocumentsMetadataRepository Documents { get; }

        void SaveChanges();
    }
}
