using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MRMongoTools.Infrastructure.Attr;
using MRMongoTools.Infrastructure.Enum;
using MRMongoTools.Infrastructure.Interface;
using MRMongoTools.Infrastructure.Settings;
using MRMongoTools.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MRMongoTools.Component
{
    public abstract class Repository<T> : IRepository<T>
        where T : class, IEntity, new()
    {
        protected IMongoClient _client { get; set; }
        protected IMongoDatabase _database { get; set; }
        protected IMongoCollection<T> _collection { get; set; }

        public Repository(MRDatabaseConnectionSettings settings) : this(settings.ConnectionString, settings.Database) { }
        public Repository(string connection, string database) : this(new MongoClient(connection), database) { }
        public Repository(IMongoClient client, string database) : this(client, client.GetDatabase(database)) { }
        public Repository(IMongoClient client, IMongoDatabase database)
        {
            _client = client;
            _database = database;

            var collectionAttr = (CollectionAttr)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionAttr));
            var collectionName = collectionAttr == null && !string.IsNullOrWhiteSpace(collectionAttr.Name) ? nameof(T) : collectionAttr.Name;

            _collection = _database.GetCollection<T>(collectionName);
        }

        public static R Factory<R>(string collection, string database)
            where R : Repository<T>, IRepository<T> => (R)Activator.CreateInstance(typeof(R), new object[] { collection, database });

        protected virtual QueryBuilder<T> _builder => new QueryBuilder<T>();

        #region create

        public virtual async Task<T> Insert(T entity)
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
            entity.CreateTime = DateTime.UtcNow;
            entity.UpdateTime = null;

            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> Insert(IEnumerable<T> list)
        {
            if (list == null || !list.Any()) return new List<T>();

            var modList = list.ToList();

            modList.ForEach((x) =>
            {
                x.Id = ObjectId.GenerateNewId().ToString();
                x.CreateTime = DateTime.UtcNow;
                x.UpdateTime = null;
            });

            await _collection.InsertManyAsync(modList);

            return modList;
        }

        #endregion

        #region get

        public virtual async Task<T> Get(string id)
            => await _collection.Find(_builder.Eq(x => x.Id, id).Filter).FirstOrDefaultAsync();

        public virtual async Task<IEnumerable<T>> Get(IEnumerable<string> ids)
            => await _collection.Find(_builder.In(x => x.Id, ids).Filter).ToListAsync();

        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> search)
            => await _collection.Find(_builder.Where(search).Filter).ToListAsync();

        public virtual async Task<T> Get<F>(Expression<Func<T, F>> field, F value)
             => await _collection.Find(_builder.Eq(field, value).Filter).FirstOrDefaultAsync();

        public virtual async Task<IEnumerable<T>> GetSorted(Expression<Func<T, object>> sort, bool desc)
            => await _collection.Find(x => true).Sort(_builder.Sorting(sort, desc).Sort).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetSorted(Expression<Func<T, object>> sort, bool desc, int skip, int limit)
            => await _collection.Find(x => true).Sort(_builder.Sorting(sort, desc).Sort).Skip(skip).Limit(limit).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc)
            => await _collection.Find(_builder.Where(search).Filter).Sort(_builder.Sorting(sort, desc).Sort).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc, int skip, int limit)
            => await _collection.Find(_builder.Where(search).Filter).Sort(_builder.Sorting(sort, desc).Sort).Skip(skip).Limit(limit).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetIn<F>(Expression<Func<T, F>> field, IEnumerable<F> values)
            => await _collection.Find(_builder.In(field, values).Filter).ToListAsync();

        public virtual async Task<T> GetInFirst<F>(Expression<Func<T, F>> field, IEnumerable<F> values)
            => await _collection.Find(_builder.In(field, values).Filter).FirstOrDefaultAsync();

        public virtual async Task<IEnumerable<T>> GetInSorted<F>(Expression<Func<T, F>> field, IEnumerable<F> values, Expression<Func<T, object>> sort, bool desc)
            => await _collection.Find(_builder.In(field, values).Filter).Sort(_builder.Sorting(sort, desc).Sort).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetInSorted<F>(Expression<Func<T, F>> field, IEnumerable<F> values, Expression<Func<T, object>> sort, bool desc, int skip, int limit)
            => await _collection.Find(_builder.In(field, values).Filter).Sort(_builder.Sorting(sort, desc).Sort).Skip(skip).Limit(limit).ToListAsync();

        public virtual async Task<T> GetFirst(Expression<Func<T, bool>> search)
            => await _collection.Find(_builder.Where(search).Filter).FirstOrDefaultAsync();

        public virtual async Task<T> GetFirstSorted(Expression<Func<T, bool>> search, Expression<Func<T, object>> sort, bool desc)
            => await _collection.Find(_builder.Where(search).Filter).Sort(_builder.Sorting(sort, desc).Sort).FirstOrDefaultAsync();

        protected virtual async Task<IEnumerable<T>> GetByQuery(QueryBuilder<T> query)
        {
            if (query.Filter == null)
                query.Where(x => true);

            var fluent = _collection.Find(query.Filter);

            if (query.Sort != null)
                fluent = fluent.Sort(query.Sort).Skip(query.PropertySkip).Limit(query.PropertyLimit);

            if (query.Projection != null)
            {
                fluent = fluent.Project<T>(query.Projection);
            }

            return await fluent.ToListAsync();
        }

        protected virtual async Task<T> GetByQueryFirst(QueryBuilder<T> query)
        {
            if (query.Filter == null)
                query.Where(x => true);

            var fluent = _collection.Find(query.Filter);

            if (query.Sort != null)
                fluent = fluent.Sort(query.Sort).Skip(query.PropertySkip);

            if (query.Projection != null)
            {
                fluent = fluent.Project<T>(query.Projection);
            }

            return await fluent.FirstOrDefaultAsync();
        } 

        #endregion

        #region count

        public virtual async Task<long> Count(Expression<Func<T, bool>> search)
            => await _collection.CountDocumentsAsync(_builder.Where(search).Filter);

        public virtual async Task<long> Count<F>(Expression<Func<T, F>> field, F value)
            => await _collection.CountDocumentsAsync(_builder.Eq(field, value).Filter);

        public virtual async Task<bool> Any(Expression<Func<T, bool>> search)
            => (await Count(search)) > 0;

        public virtual async Task<bool> Any<F>(Expression<Func<T, F>> field, F value)
            => (await Count(field, value)) > 0;

        public virtual async Task<bool> ExistsOne(Expression<Func<T, bool>> search)
            => (await Count(search)) == 1;

        public virtual async Task<bool> ExistsOne<F>(Expression<Func<T, F>> field, F value)
            => (await Count(field, value)) == 1;

        public virtual async Task<bool> ExistsOne(string id)
            => (await Count(x => x.Id, id)) == 1;

        #endregion

        #region update

        public virtual async Task<T> Replace(T entity)
        {
            entity.UpdateTime = DateTime.UtcNow;
            await _collection.ReplaceOneAsync(_builder.Eq(x => x.Id, entity.Id).Filter, entity);
            return entity;
        }

        public virtual async Task<long> Replace(IEnumerable<T> entities)
        {
            var tasks = new List<Task>();
            var list = entities?.Where(x => x != null).ToList() ?? new List<T>();

            foreach (var entity in entities)
            {
                entity.UpdateTime = DateTime.UtcNow;
                tasks.Add(Replace(entity));
            }

            await Task.WhenAll(tasks);

            return list.Count;
        }

        protected async Task UpdateByQuery(QueryBuilder<T> query)
        {
            if (query.Filter == null)
                query.Where(x => true);

            query.UpdateSet(x => x.UpdateTime, DateTime.UtcNow);

            await _collection.UpdateOneAsync(query.Filter, query.Update);
        }

        protected async Task UpdateManyByQuery(QueryBuilder<T> query)
        {
            if (query.Filter == null)
                query.Where(x => true);

            query.UpdateSet(x => x.UpdateTime, DateTime.UtcNow);

            await _collection.UpdateManyAsync(query.Filter, query.Update);
        }

        #endregion

        #region delete

        public virtual async Task DeleteSoft(T entity)
            => await DeleteSoft(entity.Id);

        public virtual async Task DeleteSoft(string id)
           => await UpdateByQuery(_builder.Eq(x => x.Id, id).UpdateSet(x => x.State, EntityState.Archived));

        public virtual async Task DeleteSoftFirst(Expression<Func<T, bool>> search)
            => await UpdateByQuery(_builder.Where(search).UpdateSet(x => x.State, EntityState.Archived));

        public virtual async Task DeleteSoftAll(Expression<Func<T, bool>> search)
            => await UpdateManyByQuery(_builder.Where(search).UpdateSet(x => x.State, EntityState.Archived));

        public virtual async Task DeleteHard(T entity)
            => await DeleteHard(entity.Id);

        public virtual async Task DeleteHard(string id)
            => await _collection.DeleteOneAsync(_builder.Eq(x => x.Id, id).Filter);

        public virtual async Task DeleteHardFirst(Expression<Func<T, bool>> search)
            => await _collection.DeleteOneAsync(_builder.Where(search).Filter);

        public virtual async Task DeleteHardAll(Expression<Func<T, bool>> search)
            => await _collection.DeleteManyAsync(_builder.Where(search).Filter);

        #endregion
    }
}
