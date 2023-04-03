using Microsoft.EntityFrameworkCore;
using Repository.Data;

namespace Repository.UnitOfWork
{
    public class UnitOrWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider serviceProvider;

        public UnitOrWorkFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IUnitOfWork create()
        {
            var _context = (DbContext)serviceProvider.GetService(typeof(IEntityContext));
            return new UnitOfWork<DbContext>(_context, serviceProvider);
        }
    }
}
