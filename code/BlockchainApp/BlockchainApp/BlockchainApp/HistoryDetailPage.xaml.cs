﻿using BlockchainAPI;
using BlockchainAPI.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BlockchainApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryDetailPage : ContentPage
	{
        BlockchainClient client;
        CreatePackage package;
        public HistoryDetailPage (CreatePackage package, BlockchainClient client)
		{
            this.client = client;
            this.package = package;
            InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = package;
            recipient.Text = package.recipient.Substring(35);
            sender.Text = package.sender.Substring(35);
            propertyListView.ItemsSource = GetProperties(package).Split(new char[] { ',' });
        }

        string GetProperties(CreatePackage package)
        {
            string properties = "";
            for (int i = 0; i < package.contents.Count; i++)
            {
                string property = package.contents[i];
                property = property.Replace("%20", " ");
                if (i == package.contents.Count - 1)
                    properties = properties + property;
                else
                    properties = properties + property + ", ";
            }

            return properties;
        }

        async void Back(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}