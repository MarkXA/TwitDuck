using System;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace TwitDuck
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();

			// Observe slider zooms and window resizes

			var zooms = ZoomSlider.ObserveDependencyProperty<double>(RangeBase.ValueProperty).Select(n => (int)n);
			var resizes = RootGrid.ObserveEvent<SizeChangedEventArgs>(nameof(RootGrid.SizeChanged));

			// Save the zoom level to settings after it hasn't changed for a while

			zooms.Throttle(TimeSpan.FromSeconds(1)).UiSubscribe(SaveCurrentZoom);

			// Resize the webview when the slider changes or the window resizes (with very mild debounce)

			zooms.DistinctUntilChanged()
				.CombineLatest(resizes, (n, _) => n)
				.Throttle(TimeSpan.FromMilliseconds(10))
				.UiSubscribe(ResizeWebView);

			// Set the initial zoom (default 1000)

			ZoomSlider.Value = ApplicationData.Current.LocalSettings.Values["SliderZoom"] as int? ?? 1000;
		}

		private static void SaveCurrentZoom(int zoomValue)
		{
			ApplicationData.Current.LocalSettings.Values["SliderZoom"] = zoomValue;
		}

		private void ResizeWebView(int sliderZoom)
		{
			if (ZoomGrid == null || RootGrid == null)
				return;

			var actualZoom = sliderZoom / 1000.0;

			// There's an oddity where zooms around 0.80 can go wildly wrong. So hack it :)

			if (actualZoom > 0.79 && actualZoom <= 0.8)
				actualZoom = 0.79;
			else if (actualZoom < 0.81 && actualZoom >= 0.8)
				actualZoom = 0.81;

			ZoomGrid.Width = MeasureGrid.ActualWidth / actualZoom;
			ZoomGrid.Height = MeasureGrid.ActualHeight / actualZoom;
			ZoomTransform.ScaleX = ZoomTransform.ScaleY = actualZoom;
		}
	}
}