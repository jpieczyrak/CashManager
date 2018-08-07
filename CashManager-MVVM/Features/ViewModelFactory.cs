using System;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features
{
	public class ViewModelFactory
	{
		private readonly Func<Type, ViewModelBase> _factory;

		public ViewModelFactory(Func<Type, ViewModelBase> factory) => _factory = factory;

		public T Create<T>() where T : ViewModelBase => (T) _factory.Invoke(typeof(T));
	}
}
