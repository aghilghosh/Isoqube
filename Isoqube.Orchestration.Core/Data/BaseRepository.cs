using MongoDB.Bson;
using MongoDB.Driver;

namespace Isoqube.Orchestration.Core.Configurations.Data
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IMongoCollection<TEntity> GetContextCollection();
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(string id);
        Task<TEntity?> GetByObjectIdAsync(string id);
        Task<TEntity> GetSingleAsync(FilterDefinition<TEntity> predicate);
        Task<IEnumerable<TEntity>> GetManyAsync(FilterDefinition<TEntity> predicate);
        Task<TEntity> InsertAsync(TEntity entity, CreateIndexOptions options = null, IndexKeysDefinition<TEntity> indexKeysDefinition = null);
        Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, CreateIndexOptions options = null, IndexKeysDefinition<TEntity> indexKeysDefinition = null);
        Task<TEntity> UpdateAsync(string id, TEntity entity);
        Task DeleteAsync(string id);
    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public BaseRepository(IMongoDatabase database, string collectionName)
        {
            Collection = database.GetCollection<TEntity>(collectionName);
        }

        public IMongoCollection<TEntity> GetContextCollection()
        {
            return Collection;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq("id", id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity?> GetByObjectIdAsync(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetSingleAsync(FilterDefinition<TEntity> predicate)
        {
            return await Collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(FilterDefinition<TEntity> predicate)
        {
            var result = await Collection.FindAsync(predicate);
            return await result.ToListAsync();
        }

        public async Task<TEntity> InsertAsync(TEntity entity, CreateIndexOptions options = null, IndexKeysDefinition<TEntity> indexKeysDefinition = null)
        {
            if (options is not null && indexKeysDefinition is not null)
            {
                await Collection.Indexes.CreateOneAsync(indexKeysDefinition, options);
            }

            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, CreateIndexOptions options = null, IndexKeysDefinition<TEntity> indexKeysDefinition = null)
        {
            if (options is not null && indexKeysDefinition is not null)
            {
                await Collection.Indexes.CreateOneAsync(indexKeysDefinition, options);
            }

            await Collection.InsertManyAsync(entities);
            return entities;
        }

        public async Task<TEntity> UpdateAsync(string id, TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            await Collection.ReplaceOneAsync(filter, entity);
            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));
            await Collection.DeleteOneAsync(filter);
        }
    }
}
