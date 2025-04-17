using GerenciadorPedidos.Domain.Interfaces.IUnitOfWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Volo.Abp;
using GerenciadorPedidos.Infra.Context;

namespace GerenciadorPedidos.Infra.UnitOfWork
{
    public class UnitOfWork<TEntity> : IUnitOfWork<TEntity> where TEntity : class
    {
        #region 'Properties'

        protected readonly PedidoDbContext _context;

        private bool disposed = false;

        protected DbSet<TEntity> DbSet
        {
            get
            {
                return _context.Set<TEntity>();
            }
        }

        #endregion

        public UnitOfWork(PedidoDbContext context)
        {
            _context = context;
        }

        #region 'Methods: Create/Update/Remove/Save'

        public TEntity Create(TEntity model)
        {
            try
            {
                //if (model is EntityDates entityDates)
                //    entityDates.GetType().GetProperty("DhCri")?.SetValue(entityDates, DateTime.Now);

                DbSet.Add(model);
                Save();
                return model;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public List<TEntity> Create(List<TEntity> models)
        {
            try
            {
                //models.ForEach(model => {
                //    if (model is EntityDates entityDates)
                //    {
                //        entityDates.GetType().GetProperty("DhCri")?.SetValue(entityDates, DateTime.Now);
                //    }
                //});

                DbSet.AddRange(models);
                Save();
                return models;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        private EntityEntry<TEntity> NewMethod(TEntity model)
        {
            return _context.Entry(model);
        }

        public bool Update(TEntity model)
        {
            try
            {
                EntityEntry<TEntity> entry = NewMethod(model);

                DbSet.Attach(model);

                entry.State = EntityState.Modified;

                return Save() > 0;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public bool Update(List<TEntity> models)
        {
            try
            {
                foreach (TEntity register in models)
                {
                    EntityEntry<TEntity> entry = _context.Entry(register);
                    DbSet.Attach(register);
                    entry.State = EntityState.Modified;
                }

                return Save() > 0;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public bool Delete(TEntity model)
        {
            try
            {
                EntityEntry<TEntity> _entry = _context.Entry(model);
                DbSet.Attach(model);
                _entry.State = EntityState.Deleted;

                return Save() > 0;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public bool Delete(params object[] Keys)
        {
            try
            {
                var model = DbSet.Find(Keys);
                if (null != model)
                    return Delete(model);

                return false;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public bool Delete(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                var model = DbSet.Where(where).FirstOrDefault();
                return model != null && Delete(model);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        #endregion

        #region 'Methods: Search'

        public TEntity? Find(params object[] Keys)
        {
            try
            {
                return DbSet.Find(Keys);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public TEntity? Find(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return DbSet.AsNoTracking().FirstOrDefault(where);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public TEntity? Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, object> includes)
        {
            try
            {
                IQueryable<TEntity>? _query = DbSet;

                if (includes != null)
                    _query = includes(_query) as IQueryable<TEntity>;

                return _query?.SingleOrDefault(predicate);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return DbSet.Where(where);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public IQueryable<TEntity>? Query(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, object> includes)
        {
            try
            {
                IQueryable<TEntity>? _query = DbSet;

                if (includes != null)
                    _query = includes(_query) as IQueryable<TEntity>;

                return _query?.Where(predicate).AsQueryable();
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Query(x => true);
        }

        #endregion

        #region 'Assyncronous Methods'

        public async Task<TEntity> CreateAsync(TEntity model)
        {
            try
            {
                //if (model is EntityDates entityDates)
                //    entityDates.GetType().GetProperty("DhCri")?.SetValue(entityDates, DateTime.Now);

                DbSet.Add(model);
                await SaveAsync();
                return model;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(TEntity model)
        {
            try
            {
                EntityEntry<TEntity> entry = _context.Entry(model);

                DbSet.Attach(model);

                entry.State = EntityState.Modified;

                return await SaveAsync() > 0;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(TEntity model)
        {
            try
            {
                EntityEntry<TEntity> entry = _context.Entry(model);

                DbSet.Attach(model);

                entry.State = EntityState.Deleted;

                return await SaveAsync() > 0;
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(params object[] Keys)
        {
            try
            {
                var model = await DbSet.FindAsync(Keys);
                return model != null && await DeleteAsync(model);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                var model = await DbSet.FirstOrDefaultAsync(where);

                return model != null && await DeleteAsync(model);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        #endregion

        #region 'Search Methods Async'

        public IAsyncEnumerable<TEntity>? GetAllAsync()
        {
            try
            {
                return DbSet.AsNoTracking().AsAsyncEnumerable();
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<TEntity?> GetAsync(params object[] Keys)
        {
            try
            {
                return await DbSet.FindAsync(Keys);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return await DbSet.AsNoTracking().FirstOrDefaultAsync(where);
            }
            catch (Exception ex)
            {
               throw new BusinessException(ex.Message);
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        #endregion
    }
}
