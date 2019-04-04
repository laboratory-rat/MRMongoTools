using MongoDB.Driver;
using MRMongoTools.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRMongoTools.Tools
{
    public class QueryBuilder<T>
        where T : class, IEntity, new()
    {
        #region builders

        public FilterDefinitionBuilder<T> FilterBuilder { get; set; } = new FilterDefinitionBuilder<T>();
        public SortDefinitionBuilder<T> SortBuilder { get; set; } = new SortDefinitionBuilder<T>();
        public UpdateDefinitionBuilder<T> UpdateBuilder { get; set; } = new UpdateDefinitionBuilder<T>();
        public ProjectionDefinitionBuilder<T> ProjectionBuilder { get; set; } = new ProjectionDefinitionBuilder<T>();

        #endregion

        #region definitions

        public FilterDefinition<T> Filter { get; set; }
        public SortDefinition<T> Sort { get; set; }
        public UpdateDefinition<T> Update { get; set; }
        public ProjectionDefinition<T> Projection { get; set; }

        #endregion

        #region limits

        public int? PropertySkip { get; set; }
        public int? PropertyLimit { get; set; }
        public bool PropertyDesc { get; set; }

        #endregion

        #region filter

        public QueryBuilder<T> Where(Expression<Func<T, bool>> search)
        {
            if (search == null)
                search = x => true;

            AddOrCreate(FilterBuilder.Where(search));
            return this;
        }
        public QueryBuilder<T> In<F>(Expression<Func<T, F>> field, IEnumerable<F> values)
        {
            AddOrCreate(FilterBuilder.In(field, values));
            return this;
        }
        public QueryBuilder<T> Eq<F>(Expression<Func<T, F>> field, F value)
        {
            AddOrCreate(FilterBuilder.Eq(field, value));
            return this;
        }
        public QueryBuilder<T> Match<F>(Expression<Func<T, IEnumerable<F>>> list, Expression<Func<F, bool>> match)
        {
            AddOrCreate(FilterBuilder.ElemMatch(list, match));
            return this;
        }

        #endregion

        #region sort

        public QueryBuilder<T> Limit(int? limit)
        {
            PropertyLimit = limit;
            return this;
        }

        public QueryBuilder<T> Skip(int? skip)
        {
            PropertySkip = skip;
            return this;
        }

        public QueryBuilder<T> Sorting(Expression<Func<T, object>> sort, bool desc)
        {
            if (sort == null)
                sort = x => x.Id;

            if (Sort == null)
            {
                if (desc)
                    Sort = SortBuilder.Descending(sort);
                else
                    Sort = SortBuilder.Ascending(sort);
            }
            else
            {
                if (desc)
                    Sort = Sort.Descending(sort);
                else
                    Sort = Sort.Ascending(sort);
            }

            return this;
        }

        #endregion

        #region update

        public QueryBuilder<T> UpdateSet<F>(Expression<Func<T, F>> field, F value)
        {
            if (Update == null)
                Update = UpdateBuilder.Set(field, value);
            else
                Update = Update.Set(field, value);

            return this;
        }

        public QueryBuilder<T> UpdateAddToSet<F>(Expression<Func<T, IEnumerable<F>>> collection, F value)
        {
            if (Update == null)
                Update = UpdateBuilder.AddToSet(collection, value);
            else
                Update = Update.AddToSet(collection, value);

            return this;
        }

        public QueryBuilder<T> UpdateAddToSetEach<F>(Expression<Func<T, IEnumerable<F>>> collection, IEnumerable<F> values)
        {
            if (Update == null)
                Update = UpdateBuilder.AddToSetEach(collection, values);
            else
                Update = Update.AddToSetEach(collection, values);

            return this;
        }

        public QueryBuilder<T> UpdatePush<F>(Expression<Func<T, IEnumerable<F>>> collection, F value)
        {
            if (Update == null)
                Update = UpdateBuilder.Push(collection, value);
            else
                Update = Update.Push(collection, value);

            return this;
        }

        public QueryBuilder<T> UpdatePull<F>(Expression<Func<T, IEnumerable<F>>> collection, F item)
        {
            if (Update == null)
                Update = UpdateBuilder.Pull(collection, item);
            else
                Update = Update.Pull(collection, item);

            return this;
        }

        public QueryBuilder<T> UpdatePull<F>(Expression<Func<T, IEnumerable<F>>> collection, IEnumerable<F> values)
        {
            if (Update == null)
                Update = UpdateBuilder.PullAll(collection, values);
            else
                Update = Update.PullAll(collection, values);

            return this;
        }

        public QueryBuilder<T> UpdatePullWhere<F>(Expression<Func<T, IEnumerable<F>>> collection, Expression<Func<F, bool>> search)
        {
            if (Update == null)
                Update = UpdateBuilder.PullFilter(collection, search);
            else
                Update = Update.PullFilter(collection, search);

            return this;
        }

        #endregion

        #region projection

        public QueryBuilder<T> ProjectionInclude(Expression<Func<T, object>> field)
        {
            if(Projection == null)
                Projection = ProjectionBuilder.Include(field);
            else
                Projection = Projection.Include(field);

            return this;
        }
        public QueryBuilder<T> ProjectionExclude(Expression<Func<T, object>> field)
        {
            if (Projection == null)
                Projection = ProjectionBuilder.Exclude(field);
            else
                Projection = Projection.Exclude(field);

            return this;
        }

        #endregion

        #region utilites

        public FilterDefinition<T> And(FilterDefinition<T> f1, FilterDefinition<T> f2) => And(new FilterDefinition<T>[] { f1, f2 });
        public FilterDefinition<T> And(FilterDefinition<T>[] args) => FilterBuilder.And(args);
        public FilterDefinition<T> Or(FilterDefinition<T>[] args) => FilterBuilder.Or(args);

        protected void AddOrCreate(FilterDefinition<T> defenition)
        {
            if (Filter == null)
                Filter = defenition;
            else
                Filter = And(Filter, defenition);
        }

        #endregion

    }
}
