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

		[Test(Description = "Ensure that calling IoC.SetDependencyResolver works fine without any exceptions.")]
		public void DependencyResolver_WhenAssigned_NoExceptions()
		{
			var resolver = new DependencyResolver();
			IoC.SetDependencyResolver(resolver);
		}

		[Test(Description = "Ensure that we can call IoC.Register<T> just fine.")]
		public void DependencyResolver_WhenAssigned_CanRegisterInstances()
		{
			var resolver = new DependencyResolver();
			IoC.SetDependencyResolver(resolver);
			IoC.Register<IDummy>(new Dummy());
		}

		[Test(Description = "Ensure that IoC.Resolve<T> will return same instance registered in DependencyResolver.Register<T> since that DependencyResolver was assigned in IoC.SetDependencyResolver.")]
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

		[Test(Description = "Ensure that calling DependencyResolver.Register<T> works.")]
		public void DependencyResolver_NotAssigned_CanRegisterDependencies()
		{
			var resolver = new DependencyResolver();

			var objectToRegister = new Dummy();
			resolver.Register<IDummy>(objectToRegister);
		}

		[Test(Description = "Ensure that DependencyResolver.Resolve<T> will return same instance registered in DependencyResolver.Register<T>.")]
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