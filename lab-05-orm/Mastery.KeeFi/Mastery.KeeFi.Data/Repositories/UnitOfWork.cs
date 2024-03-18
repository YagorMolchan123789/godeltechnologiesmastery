using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDbContext _context;

        public UnitOfWork(MainDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        private IClientsRepository _clientsRepository;
        
        public IClientsRepository Clients
        {
            get
            {
                if (_clientsRepository == null)
                {
                    _clientsRepository = new ClientsRepository(_context);
                }

                return _clientsRepository;
            }
        }

        private IDocumentsMetadataRepository _documentsMetadataRepository;

        public IDocumentsMetadataRepository Documents
        {
            get
            {
                if (_documentsMetadataRepository == null)
                {
                    _documentsMetadataRepository = new DocumentsMetadataRepository(_context);
                }

                return _documentsMetadataRepository;
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
