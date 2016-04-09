using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Windows.UI.Xaml;

namespace TwitDuck
{
	public static class RxExtensionMethods
	{
		public static IObservable<T> ObserveDependencyProperty<T>(
			this DependencyObject dependencyObject,
			DependencyProperty dependencyProperty)
		{
			var valueSubj = new Subject<T>();
			dependencyObject.RegisterPropertyChangedCallback(
				dependencyProperty,
				(o, p) => valueSubj.OnNext((T)o.GetValue(p)));
			return valueSubj;
		}

		public static IObservable<T> ObserveEvent<T>(this object target, string eventName)
		{
			return Observable.FromEventPattern<T>(target, eventName).Select(pattern => pattern.EventArgs);
		}

		public static IDisposable UiSubscribe<T>(this IObservable<T> source, Action<T> onNext)
		{
			return source.ObserveOn(SynchronizationContext.Current).Subscribe(onNext);
		}
	}
}