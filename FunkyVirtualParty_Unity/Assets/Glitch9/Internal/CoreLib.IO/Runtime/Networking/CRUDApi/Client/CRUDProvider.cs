using Cysharp.Threading.Tasks;
using System;

namespace Glitch9.IO.Networking.RESTApi
{
    public interface ICRUDProvider<T>
    {
        event Action<T> OnCreate;
        event Action<T> OnRetrieve;
        event Action<T[]> OnList;
        event Action<bool> OnDelete;

        UniTask<T> CreateAsync(params object[] args);
        UniTask<T> RetrieveAsync(string id, params object[] args);
        UniTask<T> RetrieveAsync(string id, bool createIfNotFound, params object[] args);
        UniTask<T[]> QueryAsync(params object[] args);
        UniTask<bool> DeleteAsync(string id, params object[] args);
    }

    public class NotSupportedCrudOperationException : NotSupportedException
    {
        public NotSupportedCrudOperationException(object sender, string apiName, CRUDMethod method)
            : base($"{apiName} API does not support the {method} request for the {sender.ToSenderName()} endpoint.")
        {
        }

        public NotSupportedCrudOperationException(object sender, Enum apiName, CRUDMethod method)
            : base($"{apiName} API does not support the {method} request for the {sender.ToSenderName()} endpoint.")
        {
        }
    }

    public abstract class CRUDProvider<T> : ICRUDProvider<T>
    {
        public event Action<T> OnCreate;
        public event Action<T> OnRetrieve;
        public event Action<T> OnUpdate;
        public event Action<T[]> OnList;
        public event Action<bool> OnDelete;

        protected readonly string _objectName;
        protected readonly ILogger _logger;

        protected CRUDProvider(ILogger logger)
        {
            _objectName = typeof(T).Name;
            _logger = logger;
        }

        public async UniTask<T> CreateAsync(params object[] args)
        {
            T obj = await CreateInternalAsync(args);
            OnCreate?.Invoke(obj);
            return obj;
        }

        public UniTask<T> RetrieveAsync(string id, params object[] args) => RetrieveAsync(id, false, args);
        public async UniTask<T> RetrieveAsync(string id, bool createIfNotFound, params object[] args)
        {
            T obj = await RetrieveInternalAsync(id, args);

            if (obj == null)
            {
                if (createIfNotFound)
                {
                    _logger?.Warning($"{_objectName}({id}) retrieval failed. Creating new {_objectName}.");
                    await UniTask.Delay(RESTApiV5.Config.kMinOperationDelayInMillis);
                    obj = await CreateInternalAsync(args);
                }
                else
                {
                    _logger?.Warning($"{_objectName}({id}) not found.");
                }
            }

            OnRetrieve?.Invoke(obj);
            return obj;
        }

        public async UniTask<T> UpdateAsync(string id, params object[] args)
        {
            T obj = await UpdateInternalAsync(id, args);
            OnUpdate?.Invoke(obj);
            return obj;
        }

        public async UniTask<T[]> QueryAsync(params object[] args)
        {
            T[] objs = await ListInternalAsync(args);
            OnList?.Invoke(objs);
            return objs;
        }

        public async UniTask<bool> DeleteAsync(string id, params object[] args)
        {
            bool deleted = await DeleteInternalAsync(id, args);
            OnDelete?.Invoke(deleted);
            return deleted;
        }

        protected abstract UniTask<T> CreateInternalAsync(params object[] args);
        protected abstract UniTask<T> RetrieveInternalAsync(string id, params object[] args);
        protected abstract UniTask<T> UpdateInternalAsync(string id, params object[] args);
        protected abstract UniTask<T[]> ListInternalAsync(params object[] args);
        protected abstract UniTask<bool> DeleteInternalAsync(string id, params object[] args);
    }
}