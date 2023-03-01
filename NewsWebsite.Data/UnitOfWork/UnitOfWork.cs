using AutoMapper;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public NewsDBContext _Context { get; }
        private ICategoryRepository _categoryRepository;
        private IMapper _mapper;
        public UnitOfWork(NewsDBContext context , IMapper mapper)
        {
            _Context = context;
            _mapper = mapper;
        }

        public IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IBaseRepository<TEntity> repository = new BaseRepository<TEntity,NewsDBContext>(_Context);
            return repository;
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                    _categoryRepository = new CategoryRepository(_Context , _mapper);

                return _categoryRepository;
            }
        }
        public async Task Commit()
        {
            await _Context.SaveChangesAsync();
        }
    }
}
