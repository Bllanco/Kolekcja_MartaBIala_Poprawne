using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.Controls;
using Kolekcje;

namespace Kolekcje
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Collection> Collections { get; set; }
        public CollectionView collectionView;

        public MainPage()
        {
            System.Diagnostics.Debug.WriteLine("Initializing...");
            InitializeComponent(); // Dodaj tę metodę
            System.Diagnostics.Debug.WriteLine("Loading data...");
            LoadData();
            System.Diagnostics.Debug.WriteLine("Data loaded.");
            BindingContext = this;
            collectionView = new CollectionView();
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var button = new Button();
                button.SetBinding(Button.TextProperty, "Name");
                button.Clicked += async (sender, e) =>
                {
                    var collection = (sender as Button).BindingContext as Collection;
                    await Navigation.PushAsync(new ZawartoscKolekcji(collection.Id, this));
                };
                return button;
            });
            Content = collectionView;
        }
        private async void NewCollection_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Enter name", "Name: ");
            if (name != null)
            {
                int id = 1;
                if (Collections?.Count > 0)
                {
                    id = Collections[Collections.Count - 1].Id + 1;
                }

                Collections.Add(new Collection(id, name));
                SaveData();
            }
        }

        public void SaveData()
        {
            string file = "C:\\Users\\marta\\Desktop\\Collections\\Collections\\data.csv";

            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (var item in Collections)
                {
                    writer.WriteLine($"{item.Id};0;{item.Name}");
                }
                foreach (var item in Collections)
                {
                    foreach (var element in item.Elements)
                    {
                        writer.WriteLine($"{element.Id};{item.Id};{element.Name}");
                    }
                }
            }
        }

        private void LoadData()
        {
            Collections = new ObservableCollection<Collection>();
            try
            {
                string file = "C:\\dev\\Collections\\Collections\\data.csv";
                if (File.Exists(file))
                {
                    System.Diagnostics.Debug.WriteLine("File found.");
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        string[] elements = line.Split(';');
                        int id = int.Parse(elements[0]);
                        int parentId = int.Parse(elements[1]);
                        if (parentId == 0)
                        {
                            Collection collection = new Collection(id, elements[2]);
                            Collections.Add(collection);
                        }
                        else
                        {
                            Collections[parentId - 1].Elements.Add(new CollectionElement(id, elements[2]));
                        }
                        System.Diagnostics.Debug.WriteLine("Added collection " + id + ", " + elements[1]);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("File not found.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

    }

    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<CollectionElement> Elements { get; set; }
        public Collection(int id, string name)
        {
            Id = id;
            Name = name;
            Elements = new ObservableCollection<CollectionElement>();
        }
    }

    public class CollectionElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CollectionElement(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
