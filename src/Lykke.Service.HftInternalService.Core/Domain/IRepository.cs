﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MongoDB.Driver;

namespace Lykke.Service.HftInternalService.Core.Domain
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);

        Task Add(IEnumerable<TEntity> items);

        Task Update(TEntity entity);

        Task Update(IEnumerable<TEntity> items);

        Task Delete(TEntity entity);

        Task Delete(IEnumerable<TEntity> entities);

        Task<IAsyncCursor<TEntity>> All();

        Task<TEntity> Get(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> Get(Guid id);

        IQueryable<TEntity> FilterBy(Expression<Func<TEntity, bool>> expression);
    }
}
