﻿using JRepo.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace JRepo.MongoDb
{
    public class MongoRepository<TKey, T> : IRepository<TKey, T> where T : IId<TKey>
    {
        public MongoRepository(IMongoCollection<T> mongoCollection = null)
        {
            MongoCollection = mongoCollection;
        }

        public MongoRepository(IMongoDatabase mongoDatabase) : this(mongoDatabase,
            typeof(T).Name.Replace("model", "").Replace("models", ""))
        {
        }


        public MongoRepository(IMongoDatabase mongoDatabase, string storeName)
        {
            MongoCollection = mongoDatabase.GetCollection<T>(storeName);
        }
        
        public IMongoCollection<T> MongoCollection { get; }

        public Task CreateAsync(T obj)
        {
            return MongoCollection.InsertOneAsync(obj);
        }

        public IQueryable<T> Queryable()
        {
            return MongoCollection.AsQueryable();
        }

        public Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return MongoCollection.DeleteOneAsync(predicate);
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var res = await MongoCollection.FindAsync<T>(predicate);
            return await res.ToListAsync();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await (await MongoCollection.FindAsync(predicate)).FirstOrDefaultAsync();
        }

        public Task ReplaceOneAsync(Expression<Func<T, bool>> predicate, T replaceObj)
        {
            return MongoCollection.ReplaceOneAsync(predicate, replaceObj);
        }

        public Task UpdateAsync(Expression<Func<T, bool>> predicate, object updateObject)
        {
            return MongoCollection.UpdateOneAsync(predicate, new ObjectUpdateDefinition<T>(updateObject));
        }
        
        public Task UpdateStringAsync(Expression<Func<T, bool>> predicate, string updateJson)
        {
            //return MongoCollection.UpdateOneAsync(predicate, new JsonUpdateDefinition<T>(updateJson));
            return MongoCollection.UpdateOneAsync(predicate, new BsonDocumentUpdateDefinition<T>(BsonDocument.Parse(updateJson)));
        }
    }
}
