namespace Angiris.Core.DataStore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using System.Threading.Tasks;

	public interface INoSQLStoreProvider<T> : IDisposable
	{
        void Initialize();
		async Task<T> CreateEntity(T entity);

        async Task<T> ReadEntity(string id);

        async Task<T> UpdateEntity(string id, T entity);

        async Task DeleteEntity(string id);

		async Task<IEnumerable<T>> QueryEntities();

        string HostName { get; }
        string AuthKey { get; }

	}
}

