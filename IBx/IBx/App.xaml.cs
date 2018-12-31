﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace IBx
{
	public partial class App : Application
	{
        static public int ScreenWidth;
        static public int ScreenHeight;
        static public string fixedModule = "";

        public App ()
		{
			InitializeComponent();

			MainPage = new IBx.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
