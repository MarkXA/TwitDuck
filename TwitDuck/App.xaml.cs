using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TwitDuck
{
	sealed partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			var rootFrame = Window.Current.Content as Frame;

			if (rootFrame == null)
			{
				rootFrame = new Frame();

				rootFrame.NavigationFailed +=
					(sender, args) => { throw new Exception("Failed to load Page " + args.SourcePageType.FullName); };

				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
				rootFrame.Navigate(typeof(MainPage), e.Arguments);

			Window.Current.Activate();
		}
	}
}