using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Kolekcje
{
    public partial class ZawartoscKolekcji : ContentPage
    {
        public ObservableCollection<CollectionElement> ListaElementow { get; set; } // Zmiana nazwy w³aœciwoœci
        public MainPage MainPage { get; set; }
        public int KolekcjaId { get; set; }

        public ZawartoscKolekcji(int id, MainPage mainPage)
        {
            InitializeComponent();
            this.KolekcjaId = id;
            this.MainPage = mainPage;
            Label label = new Label();
            label.Text = id.ToString();
            Content = label;
            ListaElementow = mainPage.Collections.FirstOrDefault(item => item.Id == id)?.Elements;
            BindingContext = this;
            CollectionView collectionView = new CollectionView();
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var button = new Button();
                button.SetBinding(Button.TextProperty, "Name");
                return button;
            });
            Content = collectionView;
        }

        private async void NowyElement_Clicked(object sender, EventArgs e)
        {
            string nazwa = await DisplayPromptAsync("WprowadŸ nazwê", "Nazwa: ");
            if (nazwa != null)
            {
                int id = 0;
                if (ListaElementow.Count > 0)
                {
                    id = ListaElementow[ListaElementow.Count - 1].Id + 1;
                }
                else
                {
                    id = KolekcjaId * 100;
                }
                var elementKolekcji = new CollectionElement(id, nazwa);
                ListaElementow.Add(elementKolekcji);
                MainPage.SaveData();
            }
        }
    }
}
