// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

// Use this to add support for pooling Unity prefabs.
// The prefab would need to have a MonoBehaviour class
// that implements DLD.Utility.IPooled.
// #define DLD_IOC_USES_UNITY

using System;

namespace DLD.Utility
{
	/// <summary>
	/// Used for handling the retrieval of a registered Type instance.
	/// </summary>
	public interface IDependencyResolver
	{
		// =======================================================

		bool IsRegistered<T>() where T : class;

		bool IsRegistered<T>(string id) where T : class;

		/// <summary>
		/// Registers an existing class instance as a singleton.
		/// It will be returned whenever the specified type is requested via
		/// <see cref="Resolve{T}"/> or <see cref="Get{T}"/>.
		/// This can also be used for overwriting what was already registered.
		/// If null is passed, then the next time <see cref="Resolve{T}"/> is called, it will return null,
		/// and the next time <see cref="Get{T}"/> is called, it will automatically create a new instance.
		/// </summary>
		/// <typeparam name="T">data type of the instance to register</typeparam>
		/// <param name="obj">the instance that will be accessible to others when
		/// <see cref="Resolve{T}"/> or <see cref="Get{T}"/> is called.</param>
		void Register<T>(T obj) where T : class;

		void Register<T>(T obj, string id) where T : class;

		/// <summary>
		/// Returns an instance of the requested type if it was registered.
		/// Otherwise, this returns null.
		///
		/// The instance returned, if any, is effectively a singleton,
		/// because this method always returns the same instance.
		/// Which instance gets returned can be changed using <see cref="Register{T}"/>.
		/// If null is passed to <see cref="Register{T}"/>,
		/// then when this method is called, it will also return null.
		/// </summary>
		/// <typeparam name="T">data type of the instance to get</typeparam>
		/// <returns>the instance asked for, or null if there wasn't any registered</returns>
		T Resolve<T>() where T : class;

		T Resolve<T>(string id) where T : class;

		/// <summary>
		/// Returns the instance registered for the requested type, if it exists.
		/// If not, it automatically creates a new instance (using parameterless constructor),
		/// which is then registered.
		///
		/// This will never return null, because it will create a new instance if needed.
		///
		/// The instance returned is effectively a singleton,
		/// because this method always returns the same instance.
		/// Which instance gets returned can be changed using <see cref="Register{T}"/>.
		/// If null is passed to <see cref="Register{T}"/>,
		/// then when this method is called, it will create a new instance.
		/// </summary>
		/// <typeparam name="T">data type of the instance to get</typeparam>
		/// <returns>The instance asked for</returns>
		T Get<T>() where T : class, new();

		T Get<T>(string id) where T : class, new();

		// =======================================================

		/// <summary>
		/// Unregister the singleton instance so that it will no longer be returned, when asked for by other code.
		/// Instead, null will be returned the next time <see cref="Resolve{T}()"/> is called,
		/// until a different instance is registered, either using <see cref="Register{T}(T)"/> or <see cref="Get{T}()"/>.
		/// </summary>
		/// <typeparam name="T">data type of the instance to remove</typeparam>
		/// <param name="obj">the instance to remove</param>
		void Remove<T>(T obj) where T : class;

		/// <summary>
		/// Unregister the singleton instance identified with string id so that it will no longer be returned, when asked for by other code.
		/// Instead, null will be returned the next time <see cref="Resolve{T}(string)"/> is called,
		/// until a different instance is registered, either using <see cref="Register{T}(T, string)"/> or <see cref="Get{T}(string)"/>.
		/// </summary>
		/// <param name="id">The identifier for the instance to be removed</param>
		void Remove(string id);

		/// <summary>
		/// Remove all registered singleton instances.
		/// </summary>
		void Clear();

		// =======================================================

#if DLD_IOC_USES_UNITY
		/// <summary>
		/// <para>Return a ready-to-use instance of a Unity prefab from a pool.
		/// The instance returned is either something that was created before
		/// and re-used, or a newly created one.</para>
		///
		/// <para>Use this for creating re-usable projectiles, characters, weapons,
		/// particle emitters, etc.</para>
		///
		/// <para>This will properly use
		/// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object, UnityEngine.Transform)"/>
		/// when creating a new instance.</para>
		///
		/// <para>Since this uses UnityEngine's Instantiate, this method can only work from the main thread.</para>
		/// </summary>
		/// <param name="prefab">the "master copy" of the prefab to get</param>
		/// <param name="parent">which game object to parent the returned instance</param>
		/// <param name="preloadOnly">if true, prefab returned will not be marked as used</param>
		/// <typeparam name="T">data type of the prefab to return. this would normally be a subclass of
		/// <see cref="UnityEngine.MonoBehaviour"/></typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance created via
		/// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object, UnityEngine.Transform)"/>.</returns>
		T GetPrefabFromPool<T>(T prefab, UnityEngine.Transform parent, bool preloadOnly = false)
			where T : UnityEngine.MonoBehaviour, IPooled;

		/// <summary>
		/// <para>Returns a ready-to-use instance of a Unity prefab from a pool
		/// that belongs to the group identified via the string <see cref="id"/>.
		/// The instance returned is either something that was created before
		/// and re-used, or a newly created one.</para>
		///
		/// <para>Use this for creating re-usable projectiles, characters, weapons,
		/// particle emitters, etc.</para>
		///
		/// <para>This will properly use
		/// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object, UnityEngine.Transform)"/>
		/// when creating a new instance.</para>
		///
		/// <para>Since this uses UnityEngine's Instantiate, this method can only work from the main thread.</para>
		/// </summary>
		/// <param name="id">ID of the pool to get from. Use this to create separate pools for
		/// prefab instances even if their type is the same.</param>
		/// <param name="prefab">the "master copy" of the prefab to get</param>
		/// <param name="parent">which game object to parent the returned instance</param>
		/// <param name="preloadOnly">if true, prefab returned will not be marked as used</param>
		/// <typeparam name="T">data type of the prefab to return. this would normally be a subclass of
		/// <see cref="UnityEngine.MonoBehaviour"/></typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance created via
		/// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object, UnityEngine.Transform)"/>.</returns>
		T GetPrefabFromPool<T>(string id, T prefab, UnityEngine.Transform parent, bool preloadOnly = false)
			where T : UnityEngine.MonoBehaviour, IPooled;
#endif

		// =======================================================

		/// <summary>
		/// <para>Return a ready-to-use instance of the type specified, from a pool.</para>
		/// <para>The instance returned is either something that was created before
		/// and re-used, or a newly created one (via the parameter-less constructor).</para>
		/// <para>This method is thread-safe. Provided the rest of your code follows the rules
		/// (call <see cref="IPooled.ReleaseToPool"/> on an <see cref="IPooled"/> object once finished using it, and
		/// do not keep using it after calling <see cref="IPooled.ReleaseToPool"/>),
		/// it's guaranteed that you are the only user of the instance you receive from this method.</para>
		/// </summary>
		/// <typeparam name="T">data type of the instance to get</typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// (created via the parameter-less constructor).</returns>
		T GetFromPool<T>() where T : class, IPooled, new();

		/// <summary>
		/// <para>Return a new usable instance of the type specified, from a pool
		/// that belongs to the group identified via the string <see cref="id"/></para>
		/// <para>The instance returned is either something that was created before
		/// and re-used, or a newly created one (via the default constructor).</para>
		/// <para>This method is thread-safe. Provided the rest of your code follows the rules
		/// (call <see cref="IPooled.ReleaseToPool"/> on an <see cref="IPooled"/> object once finished using it, and
		/// do not keep using it after calling <see cref="IPooled.ReleaseToPool"/>), it's guaranteed
		/// that you are the only user of the instance you receive from this method.</para>
		/// </summary>
		/// <param name="id">ID of the pool to get from. Use this to create separate pools for
		/// instances even if their type is the same.</param>
		/// <typeparam name="T">data type of the instance to get</typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// (created via the parameter-less constructor).</returns>
		T GetFromPool<T>(string id) where T : class, IPooled, new();

		/// <summary>
		/// <para>Return a new usable instance of the type specified, from a pool.</para>
		/// <para>Use this overload if you want the object's instantiation to use
		/// a constructor that has parameters.</para>
		///
		/// <para>The instance returned is either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// created via the supplied <see cref="System.Func{T}"/> callback.</para>
		///
		/// <para>This method is thread-safe. Provided the rest of your code follows the rules
		/// (call <see cref="IPooled.ReleaseToPool"/> on an <see cref="IPooled"/> object once finished using it, and
		/// do not keep using it after calling <see cref="IPooled.ReleaseToPool"/>), it's guaranteed
		/// that you are the only user of the instance you receive from this method.</para>
		/// </summary>
		/// <param name="constructor">should return a new instance of the object</param>
		/// <typeparam name="T">the type of object wanted</typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// created via the supplied <see cref="System.Func{T}"/> callback.</returns>
		T GetFromPool<T>(Func<T> constructor) where T : class, IPooled;

		/// <summary>
		/// <para>Return a new usable instance of the type specified, from a pool
		/// that belongs to the group identified via the string <see cref="id"/></para>
		/// <para>Use this overload if you want the object's instantiation to use
		/// a constructor that has parameters.</para>
		///
		/// <para>The instance returned is either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// created via the supplied <see cref="System.Func{T}"/> callback.</para>
		///
		/// <para>This method is thread-safe. Provided the rest of your code follows the rules
		/// (call <see cref="IPooled.ReleaseToPool"/> on an <see cref="IPooled"/> object once finished using it, and
		/// do not keep using it after calling <see cref="IPooled.ReleaseToPool"/>), it's guaranteed
		/// that you are the only user of the instance you receive from this method.</para>
		/// </summary>
		/// <param name="id">ID of the pool to get from. Use this to create separate pools for
		/// instances even if their type is the same.</param>
		/// <param name="constructor">should return a new instance of the object</param>
		/// <typeparam name="T">the type of object wanted</typeparam>
		/// <returns>A usable instance, either a previously created instance (that is
		/// now unused and ready for new usage), or a completely new instance
		/// created via the supplied <see cref="System.Func{T}"/> callback.</returns>
		T GetFromPool<T>(string id, Func<T> constructor) where T : class, IPooled;

		/// <summary>
		/// <para>Return a ready-to-use instance of the type specified, from a pool.
		/// The instance returned is either something that was created before
		/// and re-used, or a newly created one.</para>
		/// <para>Use this overload if what you have is a System.Type,
		/// or you want to instantiate a concrete type,
		/// but get an interface in return.</para>
		/// <para>This method is thread-safe. Provided the rest of your code follows the rules
		/// (call <see cref="IPooled.ReleaseToPool"/> on an <see cref="IPooled"/> object once finished using it, and
		/// do not keep using it after calling <see cref="IPooled.ReleaseToPool"/>), it's guaranteed
		/// that you are the only user of the instance you receive from this method.</para>
		/// </summary>
		/// <param name="typeToGet">data type of the instance to get</param>
		/// <typeparam name="T">data type of the instance to be type-casted to</typeparam>
		/// <returns></returns>
		T GetFromPool<T>(System.Type typeToGet) where T : class, IPooled, new();

		// =======================================================
	}
}