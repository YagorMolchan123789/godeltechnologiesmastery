using Mastery.KeeFi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KeeFiDbContext _context;

        public UnitOfWork(KeeFiDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        private IClientsRepository _clientsRepository;

        public IClientsRepository ClientsRepository
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
        public IDocumentsMetadataRepository DocumentsMetadataRepository
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
    }
}
