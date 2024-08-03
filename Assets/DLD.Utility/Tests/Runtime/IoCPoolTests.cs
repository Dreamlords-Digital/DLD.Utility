// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace DLD.Utility.Tests
{
	[TestFixture]
	public class IoCPoolTests
	{
		readonly IDependencyResolver _unitTestResolver = new DependencyResolver();
		IDependencyResolver _originalResolver;

		// ----------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			_originalResolver = IoC.GetCurrentResolver();
			_unitTestResolver.Clear();
			IoC.SetDependencyResolver(_unitTestResolver);
			Sum.ResetNumberOfInstances();
		}

		[TearDown]
		public void TearDown()
		{
			// put back the real resolver
			IoC.SetDependencyResolver(_originalResolver);
		}

		// ----------------------------------------------------

		class PoolableObject : IPooled
		{
			bool _isUsed;

			public bool IsUnused => !_isUsed;

			public void ReleaseToPool()
			{
				_isUsed = false;
			}

			public void OnTakenFromPool()
			{
				_isUsed = true;
			}
		}

		class PoolableOwnerCounter : IPooled
		{
			bool _isUsed;
			int _currentOwnerCount;

			public bool IsUnused => !_isUsed;

			public int CurrentOwnerCount => _currentOwnerCount;

			public void ReleaseToPool()
			{
				_currentOwnerCount -= 1;
				Assert.AreEqual(0, _currentOwnerCount);
				_isUsed = false;
			}

			public void OnTakenFromPool()
			{
				_currentOwnerCount += 1;
				Assert.AreEqual(1, _currentOwnerCount);
				_isUsed = true;
			}
		}

		class Sum : IPooled
		{
			bool _isUsed;

			public bool IsUnused => !_isUsed;

			public void ReleaseToPool()
			{
				_isUsed = false;
			}

			public void OnTakenFromPool()
			{
				_isUsed = true;
			}

			static int _numberOfInstances;

			public static void ResetNumberOfInstances()
			{
				_numberOfInstances = 0;
			}

			public Sum()
			{
				++_numberOfInstances;
			}

			readonly StringBuilder _sb = new StringBuilder();

			public void Append(string s)
			{
				_sb.Append(s);
			}

			public string CurrentString => _sb.ToString();

			int _currentSum;

			public void Add(int valueToAdd)
			{
				//Debug.Log($"Sum.Add: {_currentSum} + {valueToAdd} = {_currentSum + valueToAdd}");
				_currentSum += valueToAdd;
			}

			public int CurrentSum => _currentSum;

			public void Clear()
			{
				_currentSum = 0;
			}
		}

		// ----------------------------------------------------

		[Test(Description = "Initial check to make sure that the resolver we're using is exclusive for testing.")]
		public void DuringTesting_ResolverChanged()
		{
			Assert.AreNotSame(_originalResolver, _unitTestResolver);

			Assert.AreSame(IoC.GetCurrentResolver(), _unitTestResolver);
			Assert.AreNotSame(IoC.GetCurrentResolver(), _originalResolver);
		}

		[Test(Description = "Ensure the sum code in our test DLD.Utility.Tests.IoCPoolTests.Sum class works as expected.")]
		public void Add_ComesFromPool_CurrentSumIsCorrect()
		{
			var fromPool1 = IoC.GetFromPool<Sum>();
			fromPool1.Add(2);
			Assert.AreEqual(2, fromPool1.CurrentSum);
			fromPool1.Add(7);
			Assert.AreEqual(9, fromPool1.CurrentSum);
			fromPool1.ReleaseToPool();
			fromPool1.Clear();
			Assert.AreEqual(0, fromPool1.CurrentSum);
		}

		[Test(Description = "Ensure that IoC.GetFromPool called from multiple threads all return unique instances.")]
		public void GetFromPool_UsedInThreads_AreUniqueInstances()
		{
			var thread1 = new Thread(UsePoolableOwnerCounter);
			var thread2 = new Thread(UsePoolableOwnerCounter);
			var thread3 = new Thread(UsePoolableOwnerCounter);
			var thread4 = new Thread(UsePoolableOwnerCounter);
			var thread5 = new Thread(UsePoolableOwnerCounter2);
			var thread6 = new Thread(UsePoolableOwnerCounter2);

			thread6.Start();
			thread1.Start();
			thread2.Start();
			thread3.Start();
			thread4.Start();
			thread5.Start();

			Debug.Log(_unitTestResolver.ToString());

			var instanceInMainThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceInMainThread.CurrentOwnerCount);
			instanceInMainThread.ReleaseToPool();

			instanceInMainThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceInMainThread.CurrentOwnerCount);
			instanceInMainThread.ReleaseToPool();

			// let the threads run
			// 1 second should be more than enough
			Thread.Sleep(1000);

			instanceInMainThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceInMainThread.CurrentOwnerCount);
			instanceInMainThread.ReleaseToPool();

			// ensure all threads are finished
			thread1.Join();
			thread2.Join();
			thread3.Join();
			thread4.Join();
			thread5.Join();
			thread6.Join();

			instanceInMainThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceInMainThread.CurrentOwnerCount);
			instanceInMainThread.ReleaseToPool();

			Debug.Log(_unitTestResolver.ToString());
		}

		static void UsePoolableOwnerCounter()
		{
			var instance1GotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instance1GotWhileInThread.CurrentOwnerCount);

			var instance2GotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instance2GotWhileInThread.CurrentOwnerCount);

			var instance3GotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instance3GotWhileInThread.CurrentOwnerCount);

			var instance4GotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instance4GotWhileInThread.CurrentOwnerCount);

			instance1GotWhileInThread.ReleaseToPool();
			instance2GotWhileInThread.ReleaseToPool();
			instance3GotWhileInThread.ReleaseToPool();
			instance4GotWhileInThread.ReleaseToPool();
		}

		static void UsePoolableOwnerCounter2()
		{
			var instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();

			instanceGotWhileInThread = IoC.GetFromPool<PoolableOwnerCounter>();
			Assert.AreEqual(1, instanceGotWhileInThread.CurrentOwnerCount);
			instanceGotWhileInThread.ReleaseToPool();
		}

		[Test(Description = "Ensure that a thread that calls IoC.GetFromPool has sole control of the instance returned.")]
		public void GetFromPool_UsedInThreads_IsMutuallyExclusive()
		{
			var instanceFromMainThread = IoC.GetFromPool<Sum>();
			instanceFromMainThread.Add(100);
			Assert.AreEqual(100, instanceFromMainThread.CurrentSum);
			instanceFromMainThread.Clear();
			instanceFromMainThread.ReleaseToPool(); // allow instance to be reused, it will be used by one of the threads

			// we expect these threads to get different pooled objects
			// or they use the same pooled object, but one at a time
			//
			var thread1 = new Thread(AddToPooledSumMutuallyExclusive);
			var thread2 = new Thread(AddToPooledSumMutuallyExclusive);
			var thread3 = new Thread(AddToPooledSumMutuallyExclusive);
			var thread4 = new Thread(AddToPooledSumMutuallyExclusive);

			var threadList = new List<Thread>(4) { thread1, thread2, thread3, thread4 };

			// we give each thread a different parameter, shuffled so it's out of order
			var parameterList = new List<string>(4) { "1", "2", "3", "4" };
			parameterList.Shuffle();

			var sb = new StringBuilder();
			for (int n = 0, len = parameterList.Count; n < len; ++n)
			{
				sb.Append(parameterList[n]);
			}

			Debug.Log($"Shuffled parameters: {sb}");

			// one parameter for each thread
			Assert.IsTrue(parameterList.Count == threadList.Count);

			for (int n = 0, len = threadList.Count; n < len; ++n)
			{
				threadList[n].Start(parameterList[n]);
			}

			// let the threads run
			// 1 second should be more than enough
			Thread.Sleep(1000);

			// ensure all threads are finished
			thread1.Join();
			thread2.Join();
			thread3.Join();
			thread4.Join();

			Debug.Log(_unitTestResolver.ToString());

			// this is what happened to our instance:
			Debug.Log(instanceFromMainThread.CurrentString);

			// "11111" will happen if all threads managed to each get different instances (only 1st thread got the `instanceFromMainThread`)
			// "11111222223333344444" will happen if the threads ran one after the other
			// other combinations that are fine would be:
			// "111113333344444" (only 2nd thread got a different instance)
			// "22222111113333344444" (even though this is out of order, the threads ran one after the other, which means the mutual exclusivity worked)
			// "11111444443333322222"

			// What we're checking for is that the numbers don't mix out of order, like "12243412412233441331",
			// that would have meant that the threads ran simultaneously and were using the same instance

			Assert.IsTrue(instanceFromMainThread.CurrentString == "11111" ||
			              instanceFromMainThread.CurrentString == "22222" ||
			              instanceFromMainThread.CurrentString == "33333" ||
			              instanceFromMainThread.CurrentString == "44444" ||

			              // using https://numbergenerator.org/randomnumbergenerator/combinations-generator#!numbers=2&lines=5000&low=1&high=4&unique=true&order_matters=true&csv=&oddeven=&oddqty=0&sorted=true&sets=
			              instanceFromMainThread.CurrentString == "1111122222" ||
			              instanceFromMainThread.CurrentString == "1111133333" ||
			              instanceFromMainThread.CurrentString == "1111144444" ||
			              instanceFromMainThread.CurrentString == "2222211111" ||
			              instanceFromMainThread.CurrentString == "2222233333" ||
			              instanceFromMainThread.CurrentString == "2222244444" ||
			              instanceFromMainThread.CurrentString == "3333311111" ||
			              instanceFromMainThread.CurrentString == "3333322222" ||
			              instanceFromMainThread.CurrentString == "3333344444" ||
			              instanceFromMainThread.CurrentString == "4444411111" ||
			              instanceFromMainThread.CurrentString == "4444422222" ||
			              instanceFromMainThread.CurrentString == "4444433333" ||

			              // using https://numbergenerator.org/randomnumbergenerator/combinations-generator#!numbers=3&lines=5000&low=1&high=4&unique=true&order_matters=true&csv=&oddeven=&oddqty=0&sorted=true&sets=
			              instanceFromMainThread.CurrentString == "111112222233333" ||
			              instanceFromMainThread.CurrentString == "111112222244444" ||
			              instanceFromMainThread.CurrentString == "111113333322222" ||
			              instanceFromMainThread.CurrentString == "111113333344444" ||
			              instanceFromMainThread.CurrentString == "111114444422222" ||
			              instanceFromMainThread.CurrentString == "111114444433333" ||
			              instanceFromMainThread.CurrentString == "222221111133333" ||
			              instanceFromMainThread.CurrentString == "222221111144444" ||
			              instanceFromMainThread.CurrentString == "222223333311111" ||
			              instanceFromMainThread.CurrentString == "222223333344444" ||
			              instanceFromMainThread.CurrentString == "222224444411111" ||
			              instanceFromMainThread.CurrentString == "222224444433333" ||
			              instanceFromMainThread.CurrentString == "333331111122222" ||
			              instanceFromMainThread.CurrentString == "333331111144444" ||
			              instanceFromMainThread.CurrentString == "333332222211111" ||
			              instanceFromMainThread.CurrentString == "333332222244444" ||
			              instanceFromMainThread.CurrentString == "333334444411111" ||
			              instanceFromMainThread.CurrentString == "333334444422222" ||
			              instanceFromMainThread.CurrentString == "444441111122222" ||
			              instanceFromMainThread.CurrentString == "444441111133333" ||
			              instanceFromMainThread.CurrentString == "444442222211111" ||
			              instanceFromMainThread.CurrentString == "444442222233333" ||
			              instanceFromMainThread.CurrentString == "444443333311111" ||
			              instanceFromMainThread.CurrentString == "444443333322222" ||

			              // using https://numbergenerator.org/randomnumbergenerator/combinations-generator#!numbers=4&lines=5000&low=1&high=4&unique=true&order_matters=true&csv=&oddeven=&oddqty=0&sorted=true&sets=
			              instanceFromMainThread.CurrentString == "11111222223333344444" ||
			              instanceFromMainThread.CurrentString == "11111222224444433333" ||
			              instanceFromMainThread.CurrentString == "11111333332222244444" ||
			              instanceFromMainThread.CurrentString == "11111333334444422222" ||
			              instanceFromMainThread.CurrentString == "11111444442222233333" ||
			              instanceFromMainThread.CurrentString == "11111444443333322222" ||
			              instanceFromMainThread.CurrentString == "22222111113333344444" ||
			              instanceFromMainThread.CurrentString == "22222111114444433333" ||
			              instanceFromMainThread.CurrentString == "22222333331111144444" ||
			              instanceFromMainThread.CurrentString == "22222333334444411111" ||
			              instanceFromMainThread.CurrentString == "22222444441111133333" ||
			              instanceFromMainThread.CurrentString == "22222444443333311111" ||
			              instanceFromMainThread.CurrentString == "33333111112222244444" ||
			              instanceFromMainThread.CurrentString == "33333111114444422222" ||
			              instanceFromMainThread.CurrentString == "33333222221111144444" ||
			              instanceFromMainThread.CurrentString == "33333222224444411111" ||
			              instanceFromMainThread.CurrentString == "33333444441111122222" ||
			              instanceFromMainThread.CurrentString == "33333444442222211111" ||
			              instanceFromMainThread.CurrentString == "44444111112222233333" ||
			              instanceFromMainThread.CurrentString == "44444111113333322222" ||
			              instanceFromMainThread.CurrentString == "44444222221111133333" ||
			              instanceFromMainThread.CurrentString == "44444222223333311111" ||
			              instanceFromMainThread.CurrentString == "44444333331111122222" ||
			              instanceFromMainThread.CurrentString == "44444333332222211111",
				"CurrentString is expected to be \"11111\", \"22222\", \"33333\", \"44444\", or \"11111222223333344444\", but is {0}",
				instanceFromMainThread.CurrentString);
		}

		static void AddToPooledSumMutuallyExclusive(object arg)
		{
			var argAsString = (string)arg;

			var random = new System.Random();
			Thread.Sleep(random.Next(0, 10));

			// Since there's mutual exclusivity, this thread,
			// if it reaches this point while another thread hasn't finished,
			// will wait here while the pool is still in use.
			// That ensures we are the only user of the instance we receive.
			var instanceGotWhileInThread = IoC.GetFromPool<Sum>();

			// after the call to GetFromPool,
			// the other threads can finally use GetFromPool

			// repeat string 5 times
			instanceGotWhileInThread.Append(argAsString);
			instanceGotWhileInThread.Append(argAsString);
			instanceGotWhileInThread.Append(argAsString);
			instanceGotWhileInThread.Append(argAsString);
			instanceGotWhileInThread.Append(argAsString);

			instanceGotWhileInThread.ReleaseToPool();
		}

		[Test(Description = "Ensure that IPooled.IsUnused returns the correct value before and after calling IPooled.ReleaseToPool.")]
		public void IsUnused_WhenUsingFromPool_IsCorrect()
		{
			// { used }
			var fromPool1 = IoC.GetFromPool<PoolableObject>();
			Assert.IsFalse(fromPool1.IsUnused);


			// {1: used } {2: used }
			var fromPool2 = IoC.GetFromPool<PoolableObject>();
			Assert.IsFalse(fromPool2.IsUnused);

			Assert.AreNotSame(fromPool1, fromPool2);

			// ---------------------------

			// {1:unused} {2: used }
			fromPool1.ReleaseToPool();
			Assert.IsTrue(fromPool1.IsUnused);

			// {1: used } {2: used }
			var fromPool3 = IoC.GetFromPool<PoolableObject>();

			Assert.AreSame(fromPool1, fromPool3);
			Assert.AreNotSame(fromPool2, fromPool3);

			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsFalse(fromPool1.IsUnused);

			// ---------------------------

			// {1: used } {2: used } {3: used }
			var fromPool4 = IoC.GetFromPool<PoolableObject>();
			Assert.IsFalse(fromPool4.IsUnused);

			// fromPool4 is an entirely new one so it's not same with any existing
			Assert.AreNotSame(fromPool1, fromPool4);
			Assert.AreNotSame(fromPool2, fromPool4);
			Assert.AreNotSame(fromPool3, fromPool4);


			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsFalse(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);

			// ---------------------------

			// {1: used } {2: used } {3:unused}
			fromPool4.ReleaseToPool();
			Assert.IsTrue(fromPool4.IsUnused);

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsFalse(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);

			// ---------------------------

			// {1: used } {2: used } {3: used }
			var fromPool5 = IoC.GetFromPool<PoolableObject>();

			Assert.AreNotSame(fromPool1, fromPool5);
			Assert.AreNotSame(fromPool2, fromPool5);
			Assert.AreNotSame(fromPool3, fromPool5);
			Assert.AreSame(fromPool4, fromPool5);

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsFalse(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsFalse(fromPool4.IsUnused);
			Assert.IsFalse(fromPool5.IsUnused);

			// ---------------------------

			// {1: used } {2:unused} {3: used }
			fromPool2.ReleaseToPool();

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsTrue(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsFalse(fromPool4.IsUnused);
			Assert.IsFalse(fromPool5.IsUnused);

			// ---------------------------

			// {1:unused} {2:unused} {3:unused}
			fromPool1.ReleaseToPool();
			fromPool2.ReleaseToPool();
			fromPool3.ReleaseToPool();
			fromPool4.ReleaseToPool();
			fromPool5.ReleaseToPool();

			Assert.IsTrue(fromPool1.IsUnused);
			Assert.IsTrue(fromPool2.IsUnused);
			Assert.IsTrue(fromPool3.IsUnused);
			Assert.IsTrue(fromPool4.IsUnused);
			Assert.IsTrue(fromPool5.IsUnused);

			// ---------------------------

			// {1: used } {2:unused} {3:unused}
			var fromPool6 = IoC.GetFromPool<PoolableObject>();
			Assert.AreSame(fromPool1, fromPool6);

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsTrue(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsTrue(fromPool4.IsUnused);
			Assert.IsTrue(fromPool5.IsUnused);
			Assert.IsFalse(fromPool6.IsUnused);

			// ---------------------------

			// {1: used } {2: used } {3:unused}
			var fromPool7 = IoC.GetFromPool<PoolableObject>();
			Assert.AreSame(fromPool2, fromPool7);

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsFalse(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsTrue(fromPool4.IsUnused);
			Assert.IsTrue(fromPool5.IsUnused);
			Assert.IsFalse(fromPool6.IsUnused);
			Assert.IsFalse(fromPool7.IsUnused);

			// ---------------------------

			// {1: used } {2: used } {3: used }
			var fromPool8 = IoC.GetFromPool<PoolableObject>();
			Assert.AreSame(fromPool4, fromPool8);

			Assert.IsFalse(fromPool1.IsUnused);
			Assert.IsFalse(fromPool2.IsUnused);
			Assert.IsFalse(fromPool3.IsUnused);
			Assert.IsFalse(fromPool4.IsUnused);
			Assert.IsFalse(fromPool5.IsUnused);
			Assert.IsFalse(fromPool6.IsUnused);
			Assert.IsFalse(fromPool7.IsUnused);
			Assert.IsFalse(fromPool8.IsUnused);
		}
	}
}