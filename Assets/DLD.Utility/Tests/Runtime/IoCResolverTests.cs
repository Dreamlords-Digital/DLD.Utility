// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using NUnit.Framework;

namespace DLD.Utility.Tests
{
	[TestFixture]
	public class IoCResolverTests
	{
		[SetUp]
		public void SetUp()
		{
		}

		// ----------------------------------------------------

		[Test]
		public void DependencyResolver_WhenAssigned_NoExceptions()
		{
			var resolver = new DependencyResolver();
			IoC.SetDependencyResolver(resolver);
		}

		[Test]
		public void DependencyResolver_WhenAssigned_CanRegisterInstances()
		{
			var resolver = new DependencyResolver();
			IoC.SetDependencyResolver(resolver);
			IoC.Register<IDummy>(new Dummy());
		}

		[Test]
		public void DependencyResolver_AfterRegisteringInstance_CanRetrieveSameInstanceAsInterface()
		{
			var dummyToRegister = new Dummy();
			var resolver = new DependencyResolver();
			resolver.Register<IDummy>(dummyToRegister);


			IoC.SetDependencyResolver(resolver);

			var test = IoC.Resolve<IDummy>();
			Assert.IsInstanceOf(typeof(IDummy), test);

			Assert.IsTrue(ReferenceEquals(test, dummyToRegister));
		}

		[Test]
		public void DependencyResolver_NotAssigned_CanRegisterDependencies()
		{
			var resolver = new DependencyResolver();

			var objectToRegister = new Dummy();
			resolver.Register<IDummy>(objectToRegister);
		}

		[Test]
		public void DependencyResolver_NotAssigned_CanResolveDependencies()
		{
			var resolver = new DependencyResolver();

			var objectToRegister = new Dummy();
			resolver.Register<IDummy>(objectToRegister);

			var objectFromResolver = resolver.Resolve<IDummy>();

			Assert.IsInstanceOf(typeof(IDummy), objectFromResolver);
			Assert.AreSame(objectToRegister, objectFromResolver);
		}

		// ----------------------------------------------------

		internal interface IDummy
		{
		}

		internal class Dummy : IDummy
		{
		}
	}
}