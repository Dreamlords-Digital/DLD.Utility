// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System;

namespace DLD.Utility
{
	/// <summary>
	/// Inversion of Control.
	/// Allows retrieval of singletons and pooled objects.
	/// Works in both runtime and editor scripts.
	/// Functions as a "static gateway" to a <see cref="DependencyResolver"/>.
	/// </summary>
	public static class IoC
	{
		static IDependencyResolver _resolver;

		static void CreateDefaultDependencyResolverIfNeeded()
		{
			if (_resolver == null)
			{
				_resolver = new DependencyResolver();
			}
		}

		// =======================================================

		public static bool IsRegistered<T>(string id) where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.IsRegistered<T>(id);
		}

		/// <inheritdoc cref="IDependencyResolver.Register{T}(T)"/>
		public static void Register<T>(T obj) where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			_resolver.Register(obj);
		}

		public static void Register<T>(T obj, string id) where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			_resolver.Register(obj, id);
		}

		/// <inheritdoc cref="IDependencyResolver.Resolve{T}()"/>
		public static T Resolve<T>() where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.Resolve<T>();
		}

		public static T Resolve<T>(string id) where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.Resolve<T>(id);
		}

		/// <inheritdoc cref="IDependencyResolver.Get{T}()"/>
		public static T Get<T>() where T : class, new()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.Get<T>();
		}

		/// <inheritdoc cref="IDependencyResolver.Get{T}(string)"/>
		public static T Get<T>(string id) where T : class, new()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.Get<T>(id);
		}

		// =======================================================

		/// <inheritdoc cref="IDependencyResolver.Remove{T}(T)"/>
		public static void Remove<T>(T obj) where T : class
		{
			CreateDefaultDependencyResolverIfNeeded();

			_resolver.Remove(obj);
		}

		/// <inheritdoc cref="IDependencyResolver.Remove(string)"/>
		public static void Remove(string id)
		{
			CreateDefaultDependencyResolverIfNeeded();

			_resolver.Remove(id);
		}

		// =======================================================

#if DLD_IOC_USES_UNITY
		/// <inheritdoc cref="IDependencyResolver.GetPrefabFromPool{T}(T, UnityEngine.Transform, bool)"/>
		public static T GetPrefabFromPool<T>(T prefab, UnityEngine.Transform parent, bool preloadOnly = false)
			where T : UnityEngine.MonoBehaviour, IPooled
		{
			CreateDefaultDependencyResolverIfNeeded();

			// note: instantiating prefabs is on the main thread only,
			// so no need to guard against deadlock with the _poolInUse bool

			return _resolver.GetPrefabFromPool(prefab, parent, preloadOnly);
		}

		/// <inheritdoc cref="IDependencyResolver.GetPrefabFromPool{T}(string, T, UnityEngine.Transform, bool)"/>
		public static T GetPrefabFromPool<T>(string id, T prefab, UnityEngine.Transform parent, bool preloadOnly = false)
			where T : UnityEngine.MonoBehaviour, IPooled
		{
			CreateDefaultDependencyResolverIfNeeded();

			// note: instantiating prefabs is on the main thread only,
			// so no need to guard against deadlock with the _poolInUse bool

			return _resolver.GetPrefabFromPool(id, prefab, parent, preloadOnly);
		}
#endif

		// =======================================================

		/// <inheritdoc cref="IDependencyResolver.GetFromPool{T}()"/>
		public static T GetFromPool<T>() where T : class, IPooled, new()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.GetFromPool<T>();
		}

		/// <inheritdoc cref="IDependencyResolver.GetFromPool{T}(string)"/>
		public static T GetFromPool<T>(string id) where T : class, IPooled, new()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.GetFromPool<T>(id);
		}

		/// <inheritdoc cref="IDependencyResolver.GetFromPool{T}(Func{T})"/>
		public static T GetFromPool<T>(Func<T> constructor) where T : class, IPooled
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.GetFromPool(constructor);
		}

		/// <inheritdoc cref="IDependencyResolver.GetFromPool{T}(string, Func{T})"/>
		public static T GetFromPool<T>(string id, Func<T> constructor) where T : class, IPooled
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.GetFromPool(id, constructor);
		}

		/// <inheritdoc cref="IDependencyResolver.GetFromPool{T}(System.Type)"/>
		public static T GetFromPool<T>(System.Type typeToGet) where T : class, IPooled, new()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver.GetFromPool<T>(typeToGet);
		}

		// =======================================================

		/// <summary>
		/// Can be used to change the resolver, so that the dependency injections
		/// will return a different set of instances.
		/// </summary>
		/// <param name="resolver"></param>
		public static void SetDependencyResolver(IDependencyResolver resolver)
		{
			_resolver = resolver;
		}

		public static IDependencyResolver GetCurrentResolver()
		{
			CreateDefaultDependencyResolverIfNeeded();

			return _resolver;
		}
	}
}