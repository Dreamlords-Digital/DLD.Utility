﻿// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using NUnit.Framework;

namespace DLD.Utility.Tests
{
	[TestFixture]
	public class PooledCollectionTests
	{
		// ===============================================================================

		[Test(Description = "Ensure that a reused PooledList<T> starts out empty even though it had contents in its previous use.")]
		public void PooledList_WhenReusedFromPool_EmptyAtStart()
		{
			var intList = IoC.GetFromPool<PooledList<int>>();
			Assert.AreEqual(0, intList.Count);

			intList.Add(1);
			Assert.AreEqual(1, intList.Count);

			intList.Add(45);
			Assert.AreEqual(2, intList.Count);

			intList.Add(-467);
			Assert.AreEqual(3, intList.Count);

			// this will mark the instance as usable again
			// the next time IoC.GetFromPool<>() is called
			intList.Dispose();

			// -------------------------------------------------------

			// the instance we get should be same as the earlier one
			// since Dispose() was called
			var otherIntList = IoC.GetFromPool<PooledList<int>>();

			Assert.AreSame(intList, otherIntList);

			// the instance we get should have called Clear() on its own
			// the moment it was obtained from the pool
			Assert.AreEqual(0, otherIntList.Count);

			// since it's same instance, Count for this should also be 0
			Assert.AreEqual(0, intList.Count);
		}

		// ===============================================================================

		[Test(Description = "Ensure that a reused PooledStack<T> starts out empty even though it had contents in its previous use.")]
		public void PooledStack_WhenReusedFromPool_EmptyAtStart()
		{
			var intStack = IoC.GetFromPool<PooledStack<int>>();
			Assert.AreEqual(0, intStack.Count);

			intStack.Push(1);
			Assert.AreEqual(1, intStack.Count);

			intStack.Push(45);
			Assert.AreEqual(2, intStack.Count);

			intStack.Push(-467);
			Assert.AreEqual(3, intStack.Count);

			// this will mark the instance as usable again
			// the next time IoC.GetFromPool<>() is called
			intStack.Dispose();

			// -------------------------------------------------------

			// the instance we get should be same as the earlier one
			// since Dispose() was called
			var otherIntStack = IoC.GetFromPool<PooledStack<int>>();

			Assert.AreSame(intStack, otherIntStack);

			// the instance we get should have called Clear() on its own
			// the moment it was obtained from the pool
			Assert.AreEqual(0, otherIntStack.Count);

			// since it's same instance, Count for this should also be 0
			Assert.AreEqual(0, intStack.Count);
		}

		// ===============================================================================

		[Test(Description = "Ensure that a reused PooledQueue<T> starts out empty even though it had contents in its previous use.")]
		public void PooledQueue_WhenReusedFromPool_EmptyAtStart()
		{
			var intQueue = IoC.GetFromPool<PooledQueue<int>>();
			Assert.AreEqual(0, intQueue.Count);

			intQueue.Enqueue(1);
			Assert.AreEqual(1, intQueue.Count);

			intQueue.Enqueue(45);
			Assert.AreEqual(2, intQueue.Count);

			intQueue.Enqueue(-467);
			Assert.AreEqual(3, intQueue.Count);

			// this will mark the instance as usable again
			// the next time IoC.GetFromPool<>() is called
			intQueue.Dispose();

			// -------------------------------------------------------

			// the instance we get should be same as the earlier one
			// since Dispose() was called
			var otherIntQueue = IoC.GetFromPool<PooledQueue<int>>();

			Assert.AreSame(intQueue, otherIntQueue);

			// the instance we get should have called Clear() on its own
			// the moment it was obtained from the pool
			Assert.AreEqual(0, otherIntQueue.Count);

			// since it's same instance, Count for this should also be 0
			Assert.AreEqual(0, intQueue.Count);
		}

		// ===============================================================================

		[Test(Description = "Ensure that a reused PooledHashSet<T> starts out empty even though it had contents in its previous use.")]
		public void PooledHashSet_WhenReusedFromPool_EmptyAtStart()
		{
			var intHashSet = IoC.GetFromPool<PooledHashSet<int>>();
			Assert.AreEqual(0, intHashSet.Count);

			intHashSet.Add(1);
			Assert.AreEqual(1, intHashSet.Count);

			intHashSet.Add(45);
			Assert.AreEqual(2, intHashSet.Count);

			intHashSet.Add(-467);
			Assert.AreEqual(3, intHashSet.Count);

			// this will mark the instance as usable again
			// the next time IoC.GetFromPool<>() is called
			intHashSet.Dispose();

			// -------------------------------------------------------

			// the instance we get should be same as the earlier one
			// since Dispose() was called
			var otherIntHashSet = IoC.GetFromPool<PooledHashSet<int>>();

			Assert.AreSame(intHashSet, otherIntHashSet);

			// the instance we get should have called Clear() on its own
			// the moment it was obtained from the pool
			Assert.AreEqual(0, otherIntHashSet.Count);

			// since it's same instance, Count for this should also be 0
			Assert.AreEqual(0, intHashSet.Count);
		}

		// ===============================================================================

		[Test(Description = "Ensure that a reused PooledDictionary<T> starts out empty even though it had contents in its previous use.")]
		public void PooledDictionary_WhenReusedFromPool_EmptyAtStart()
		{
			var stringIntDictionary = IoC.GetFromPool<PooledDictionary<string, int>>();
			Assert.AreEqual(0, stringIntDictionary.Count);

			stringIntDictionary.Add("a", 1);
			Assert.AreEqual(1, stringIntDictionary.Count);

			stringIntDictionary.Add("b", 45);
			Assert.AreEqual(2, stringIntDictionary.Count);

			stringIntDictionary.Add("c", -467);
			Assert.AreEqual(3, stringIntDictionary.Count);

			// this will mark the instance as usable again
			// the next time IoC.GetFromPool<>() is called
			stringIntDictionary.Dispose();

			// -------------------------------------------------------

			// the instance we get should be same as the earlier one
			// since Dispose() was called
			var otherStringIntDictionary = IoC.GetFromPool<PooledDictionary<string, int>>();

			Assert.AreSame(stringIntDictionary, otherStringIntDictionary);

			// the instance we get should have called Clear() on its own
			// the moment it was obtained from the pool
			Assert.AreEqual(0, otherStringIntDictionary.Count);

			// since it's same instance, Count for this should also be 0
			Assert.AreEqual(0, stringIntDictionary.Count);
		}

		// ===============================================================================

		[Test(Description = "Ensure that IoC.GetFromPool<PooledList<T>> will return a new instance if all current instances are used.")]
		public void GetFromPool_WhenNoUnusedIsAvailable_ShouldReturnNewInstance()
		{
			var intList = IoC.GetFromPool<PooledList<int>>();
			Assert.AreEqual(0, intList.Count);

			intList.Add(1);
			Assert.AreEqual(1, intList.Count);

			intList.Add(45);
			Assert.AreEqual(2, intList.Count);

			intList.Add(-467);
			Assert.AreEqual(3, intList.Count);

			// -------------------------------------------------------

			// the instance we get should be a new one
			// since `intList` is still in use
			var otherIntList = IoC.GetFromPool<PooledList<int>>();

			Assert.AreNotSame(intList, otherIntList);

			// since this is new, Count should be 0
			Assert.AreEqual(0, otherIntList.Count);

			// Count of other instance should be untouched
			Assert.AreEqual(3, intList.Count);
		}

		// ===============================================================================
	}
}