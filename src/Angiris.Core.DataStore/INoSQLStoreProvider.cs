namespace Angiris.Core.DataStore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using System.Threading.Tasks;

	public interface INoSqlStoreProvider<T> : IDisposable
	{
        //void Initialize();
		Task<T> CreateEntity(T entity);

        Task<T> ReadEntity(string id);

        Task<T> UpdateEntity(string id, T entity);

        Task DeleteEntity(string id);
 

	}
}

