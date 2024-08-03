// use this to add debug log messages when a pool is used/freed,
// or when something is waiting for a pool to be freed
//#define DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION

using System;
using System.Collections.Generic;
using System.Text;

namespace DLD.Utility
{
	/// <inheritdoc/>
	public class DependencyResolver : IDependencyResolver
	{
		// ======================================================================================

		const int SINGLETON_INITIAL_SIZE = 20;
		const int POOL_INITIAL_SIZE = 5;
		const int POOL_LIST_INITIAL_SIZE = 5;

		// ======================================================================================

		readonly IDictionary<Type, object> _singletons =
			new Dictionary<Type, object>(SINGLETON_INITIAL_SIZE);

		readonly IDictionary<string, object> _singletonsWithId =
			new Dictionary<string, object>(SINGLETON_INITIAL_SIZE);

		readonly IDictionary<Type, IList<IPooled>> _pools =
			new Dictionary<Type, IList<IPooled>>(POOL_INITIAL_SIZE);

		readonly IDictionary<string, IList<IPooled>> _poolsWithId =
			new Dictionary<string, IList<IPooled>>(POOL_INITIAL_SIZE);


		readonly IDictionary<Type, object> _poolsInUse =
			new Dictionary<Type, object>(POOL_INITIAL_SIZE);

		readonly IDictionary<string, object> _poolsWithIdInUse =
			new Dictionary<string, object>(POOL_INITIAL_SIZE);

		// ======================================================================================

		/// <inheritdoc/>
		public override string ToString()
		{
			// we could've used a IoC.GetFromPool<StringBuilder>()
			// but this ToString() is primarily meant to verify
			// the correctness of the pool system (in a unit test),
			// so to properly conduct the test, we won't use the
			// pool system here
			var sb = new StringBuilder(100);

			// ---------------------------------------------------

			sb.Append("Singletons: ");
			sb.Append(_singletons.Count);
			sb.Append("\n");

			foreach (var pair in _singletons)
			{
				sb.Append(pair.Key);
				sb.Append(": ");
				sb.Append(pair.Value);
			}

			sb.Append("----\n");

			// ---------------------------------------------------

			sb.Append("Pools: ");
			sb.Append(_pools.Count);
			sb.Append("\n");
			foreach (var poolPair in _pools)
			{
				lock (_poolsInUse[poolPair.Key])
				{
					sb.Append(poolPair.Key);
					sb.Append(": ");
					sb.Append(poolPair.Value.Count);
					sb.Append("\n");

					for (var i = 0; i < poolPair.Value.Count; i++)
					{
						sb.Append(i);
						sb.Append(" ");
						sb.Append(poolPair.Value[i].IsUnused ? "UNUSED" : "  USED");
						sb.Append(": ");
						sb.Append(poolPair.Value[i]);
						sb.Append("\n");
					}
				}
			}

			sb.Append("----\n");

			// ---------------------------------------------------

			sb.Append("Pools with ID: ");
			sb.Append(_poolsWithId.Count);
			sb.Append("\n");
			foreach (var poolPair in _poolsWithId)
			{
				lock (_poolsWithIdInUse[poolPair.Key])
				{
					sb.Append(poolPair.Key);
					sb.Append(": ");
					sb.Append(poolPair.Value.Count);
					sb.Append("\n");

					for (var i = 0; i < poolPair.Value.Count; i++)
					{
						sb.Append(i);
						sb.Append(" ");
						sb.Append(poolPair.Value[i].IsUnused ? "UNUSED" : "  USED");
						sb.Append(": ");
						sb.Append(poolPair.Value[i]);
						sb.Append("\n");
					}
				}
			}

			// ---------------------------------------------------

			return sb.ToString();
		}

		// ======================================================================================

		/// <inheritdoc/>
		public void Register<T>(T obj) where T : class
		{
			if (_singletons.ContainsKey(typeof(T)))
			{
				_singletons[typeof(T)] = obj;
			}
			else
			{
				_singletons.Add(typeof(T), obj);
			}
		}

		public bool IsRegistered<T>() where T : class
		{
			if (_singletons.ContainsKey(typeof(T)))
			{
				return _singletons[typeof(T)] != null;
			}

			return false;
		}

		public void Register<T>(T obj, string id) where T : class
		{
			if (id == null)
			{
				return;
			}

			_singletonsWithId[id] = obj;
		}

		public bool IsRegistered<T>(string id) where T : class
		{
			if (id == null)
			{
				return false;
			}

			if (_singletonsWithId.TryGetValue(id, out object value))
			{
				return value != null;
			}

			return false;
		}

		/// <inheritdoc/>
		public T Resolve<T>() where T : class
		{
			if (!_singletons.ContainsKey(typeof(T)))
			{
				return default;
			}

			return (T)_singletons[typeof(T)];
		}

		public T Resolve<T>(string id) where T : class
		{
			if (id == null || !_singletonsWithId.ContainsKey(id))
			{
				return default;
			}

			return (T)_singletonsWithId[id];
		}

		/// <inheritdoc/>
		public T Get<T>() where T : class, new()
		{
			if (typeof(T).IsSubclassOf(typeof(UnityEngine.MonoBehaviour)) ||
			    typeof(T) == typeof(UnityEngine.MonoBehaviour))
			{
				// MonoBehaviour classes aren't allowed to be instantiated
				// with the default constructor. Just return null.
				return null;
			}

			T obj = Resolve<T>();

			if (obj != null)
			{
				return obj;
			}

			T newInstance = new T();
			Register(newInstance);
			return newInstance;
		}

		public T Get<T>(string id) where T : class, new()
		{
			if (typeof(T).IsSubclassOf(typeof(UnityEngine.MonoBehaviour)) ||
			    typeof(T) == typeof(UnityEngine.MonoBehaviour))
			{
				// MonoBehaviour classes aren't allowed to be instantiated
				// with the default constructor. Just return null.
				return null;
			}

			T obj = Resolve<T>(id);

			if (obj != null)
			{
				return obj;
			}

			T newInstance = new T();
			Register(newInstance, id);
			return newInstance;
		}

		// ======================================================================================

		/// <inheritdoc/>
		public void Remove<T>(T obj) where T : class
		{
			if (_singletons.ContainsKey(typeof(T)))
			{
				_singletons.Remove(typeof(T));
			}
		}

		public void Remove(string id)
		{
			if (id == null)
			{
				return;
			}

			if (_singletonsWithId.ContainsKey(id))
			{
				_singletonsWithId.Remove(id);
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_singletons.Clear();
			_pools.Clear();
			_poolsWithId.Clear();

			_poolsInUse.Clear();
			_poolsWithIdInUse.Clear();
		}

		// ======================================================================================

		/// <inheritdoc/>
		public T GetFromPool<T>() where T : class, IPooled, new()
		{
			if (!_poolsInUse.ContainsKey(typeof(T)))
			{
				_poolsInUse[typeof(T)] = new object();
			}

			lock (_poolsInUse[typeof(T)])
			{
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now in use");
#endif

				// ---------------------------------------------
				// create the pool list for this if needed

				if (!_pools.ContainsKey(typeof(T)))
				{
					_pools[typeof(T)] = new List<IPooled>(POOL_LIST_INITIAL_SIZE);

					// since this is a new list, we already know it's empty
					// and that there aren't any pre-existing unused instances yet
					// so create a new one and return that

					T newInstanceForNewList = new T();

					newInstanceForNewList.OnTakenFromPool();
					_pools[typeof(T)].Add(newInstanceForNewList);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
					Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
					return newInstanceForNewList;
				}

				// ---------------------------------------------
				// find an unused instance to return

				var pool = _pools[typeof(T)];

				for (int n = 0, len = pool.Count; n < len; ++n)
				{
					if (pool[n].IsUnused)
					{
						pool[n].OnTakenFromPool();
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
						Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
						return (T)pool[n];
					}
				}

				// ---------------------------------------------
				// no unused instances (all are currently in use)
				// so create a new one and return that

				var newInstance = new T();
				newInstance.OnTakenFromPool();
				pool.Add(newInstance);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
				return newInstance;
			}
		}

		/// <inheritdoc/>
		public T GetFromPool<T>(string id) where T : class, IPooled, new()
		{
			if (!_poolsWithIdInUse.ContainsKey(id))
			{
				_poolsWithIdInUse[id] = new object();
			}

			lock (_poolsWithIdInUse[id])
			{
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now in use");
#endif

				if (!_poolsWithId.ContainsKey(id))
				{
					_poolsWithId[id] = new List<IPooled>(POOL_LIST_INITIAL_SIZE);

					T newInstanceForNewList = new T();

					newInstanceForNewList.OnTakenFromPool();
					_poolsWithId[id].Add(newInstanceForNewList);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
					Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
					return newInstanceForNewList;
				}

				// ---------------------------------------------

				var pool = _poolsWithId[id];

				for (int n = 0, len = pool.Count; n < len; ++n)
				{
					if (pool[n].IsUnused)
					{
						pool[n].OnTakenFromPool();
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
						Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
						return (T)pool[n];
					}
				}

				// ---------------------------------------------

				var newInstance = new T();
				newInstance.OnTakenFromPool();
				pool.Add(newInstance);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
				return newInstance;
			}
		}

		/// <inheritdoc/>
		public T GetFromPool<T>(Func<T> constructor) where T : class, IPooled
		{
			if (!_poolsInUse.ContainsKey(typeof(T)))
			{
				_poolsInUse[typeof(T)] = new object();
			}

			lock (_poolsInUse[typeof(T)])
			{
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now in use");
#endif

				if (!_pools.ContainsKey(typeof(T)))
				{
					_pools[typeof(T)] = new List<IPooled>(POOL_LIST_INITIAL_SIZE);

					T newInstanceForNewList = constructor();

					newInstanceForNewList.OnTakenFromPool();
					_pools[typeof(T)].Add(newInstanceForNewList);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
					Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif

					return newInstanceForNewList;
				}

				// ---------------------------------------------

				var pool = _pools[typeof(T)];

				for (int n = 0, len = pool.Count; n < len; ++n)
				{
					if (pool[n].IsUnused)
					{
						pool[n].OnTakenFromPool();
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
						Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
						return (T)pool[n];
					}
				}

				// ---------------------------------------------

				var newInstance = constructor();
				newInstance.OnTakenFromPool();
				pool.Add(newInstance);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
				return newInstance;
			}
		}

		/// <inheritdoc/>
		public T GetFromPool<T>(string id, Func<T> constructor) where T : class, IPooled
		{
			if (!_poolsWithIdInUse.ContainsKey(id))
			{
				_poolsWithIdInUse[id] = new object();
			}

			lock (_poolsWithIdInUse[id])
			{
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now in use");
#endif
				if (!_poolsWithId.ContainsKey(id))
				{
					_poolsWithId[id] = new List<IPooled>(POOL_LIST_INITIAL_SIZE);

					T newInstanceForNewList = constructor();

					newInstanceForNewList.OnTakenFromPool();
					_poolsWithId[id].Add(newInstanceForNewList);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
					Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
					return newInstanceForNewList;
				}

				// ---------------------------------------------

				var pool = _poolsWithId[id];

				for (int n = 0, len = pool.Count; n < len; ++n)
				{
					if (pool[n].IsUnused)
					{
						pool[n].OnTakenFromPool();
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
						Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
						return (T)pool[n];
					}
				}

				// ---------------------------------------------

				var newInstance = constructor();
				newInstance.OnTakenFromPool();
				pool.Add(newInstance);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} (ID: {id}) is now free");
#endif
				return newInstance;
			}
		}

		/// <inheritdoc/>
		public T GetFromPool<T>(System.Type typeToGet) where T : class, IPooled, new()
		{
			if (!_poolsInUse.ContainsKey(typeof(T)))
			{
				_poolsInUse[typeof(T)] = new object();
			}

			lock (_poolsInUse[typeof(T)])
			{
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now in use");
#endif

				// ---------------------------------------------
				// create the pool list for this if needed

				if (!_pools.ContainsKey(typeToGet))
				{
					_pools[typeToGet] = new List<IPooled>(POOL_LIST_INITIAL_SIZE);

					// since this is a new list, we already know it's empty
					// and that there aren't any pre-existing unused instances yet
					// so create a new one and return that

					var newInstanceForNewList = (T)System.Activator.CreateInstance(typeToGet);
					newInstanceForNewList.OnTakenFromPool();
					_pools[typeToGet].Add(newInstanceForNewList);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
					Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
					return newInstanceForNewList;
				}

				// ---------------------------------------------
				// find an unused instance to return

				var pool = _pools[typeToGet];

				for (int n = 0, len = pool.Count; n < len; ++n)
				{
					if (pool[n].IsUnused)
					{
						pool[n].OnTakenFromPool();
#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
						Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
						return (T)pool[n];
					}
				}

				// ---------------------------------------------
				// no unused instances (all are currently in use)
				// so create a new one and return that

				// create an instance of that class using the default parameterless constructor
				var newInstance = (T)System.Activator.CreateInstance(typeToGet);

				newInstance.OnTakenFromPool();
				pool.Add(newInstance);

#if DLD_IOC_DEBUG_POOL_MUTUAL_EXCLUSION
				Debug.Log($"DependencyResolver: Pool {typeof(T).Name} is now free");
#endif
				return newInstance;
			}
		}

		// ======================================================================================
	}
}