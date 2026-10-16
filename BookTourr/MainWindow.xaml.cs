using BookTourr.Logic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookTourr
{
    public partial class MainWindow : Window
    {
        private AppData appData;


        private IEnumerable<Tour> GetRequestedTours()
        {
            List<Tour> selectedTours = new List<Tour>();

            foreach (Tour tour in lbxToursToBoos.SelectedItems)
            {
                selectedTours.Add(tour);
            }

            return selectedTours;
        }

        public MainWindow()
        {
            InitializeComponent();

            appData = new AppData("countries.csv");

            ShowAllCountries();
            CustomerComboBox.ItemsSource = appData.Customers;
            TourComboBox.ItemsSource = appData.AllCountriesByKey.Values;

            UpdateBookingQueue();
            lbxToursToBoos.ItemsSource = appData.AllTours.Values.ToList();
        
        }

        private void ShowAllCountries()
        {
            CountriesListBox.Items.Clear();

            foreach (Country country in appData.AllCountriesByKey.Values)
            {
                CountriesListBox.Items.Add(
                    $"{country.Name} | {country.Code} | {country.Region} | Population: {country.Population}"
                );
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                ShowAllCountries();
                return;
            }

            CountryKey key = new CountryKey
            {
                Code = code,
            };

            //Kjo eshte pjesa kur e kemi me list
            //Country country = appData.AllCountries
            //    .Find(c => c.Code.Equals(
            //        code,
            //        System.StringComparison.OrdinalIgnoreCase));

            appData.AllCountriesByKey.TryGetValue(key, out Country country);

            CountriesListBox.Items.Clear();

            if (country != null)
            {
                CountriesListBox.Items.Add(
                    $"{country.Name} | {country.Code} | {country.Region} | Population: {country.Population}"
                );
            }
            else
            {
                CountriesListBox.Items.Add("Country not found.");
            }
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            CodeTextBox.Clear();
            ShowAllCountries();
        }

        private void btnAddToItinerar_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = CountriesListBox.SelectedIndex;

            if (selectedIndex == -1)
                return;

            Country selectedCountry =
                appData.AllCountriesByKey.Values.ElementAt(selectedIndex);
            int index = appData.ItineraryBuilder.Count;
            appData.ItineraryBuilder.AddLast(selectedCountry);
            //Stack e kemi vendos ketu
            var changes = new ItineraryChange(
                   ChangeType.Append, index, selectedCountry);
            appData.ChangeLog.Push(changes);


            UpdateItinerary();
        }


        private void UpdateItinerary()
        {
            ItineraryListBox.Items.Clear();

            foreach (Country country in appData.ItineraryBuilder)
            {
                ItineraryListBox.Items.Add(country.Name);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ItineraryListBox.SelectedIndex;

            if (selectedIndex == -1)
                return;

            LinkedListNode<Country> node =
                appData.ItineraryBuilder.First;

            for (int i = 0; i < selectedIndex; i++)
            {
                node = node.Next;
            }

            Country removedCountry = node.Value;
            appData.ItineraryBuilder.Remove(node);
            //Stack e kemi vendose ktu
            var change = new ItineraryChange(
                ChangeType.Remove, selectedIndex, removedCountry
                );
            appData.ChangeLog.Push(change);

            UpdateItinerary();
        }

        private void InsertBefore_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ItineraryListBox.SelectedIndex;

            if (selectedIndex == -1)
                return;

            int countryIndex = CountriesListBox.SelectedIndex;

            if (countryIndex == -1)
                return;

            Country newCountry =
                appData.AllCountriesByKey.Values.ElementAt(countryIndex);

            LinkedListNode<Country> node =
                appData.ItineraryBuilder.First;

            for (int i = 0; i < selectedIndex; i++)
            {
                node = node.Next;
            }

            appData.ItineraryBuilder.AddBefore(node, newCountry);
            //Stack e kemi vendose ketu
            var change = new ItineraryChange(
                ChangeType.Insert, selectedIndex, newCountry);
            appData.ChangeLog.Push(change);

            UpdateItinerary();
        }

        private void btnSaveTour_Click(object sender, RoutedEventArgs e)
        {
            string name = tbxTourName.Text.Trim();

            Country[] itinerary = appData.ItineraryBuilder.ToArray();

            try
            {
                Tour newTour = new Tour(name, itinerary);

                appData.AllTours.Add(newTour.Name, newTour);

                lbxToursToBoos.ItemsSource = null;
                lbxToursToBoos.ItemsSource = appData.AllTours.Values.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Cannot save tour",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            appData.ItineraryBuilder.Clear();

            tbxTourName.Clear();

            UpdateItinerary();

            MessageBox.Show("Tour added successfully!");
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (appData.ChangeLog.Count == 0)
                return;

            ItineraryChange lastChange = appData.ChangeLog.Pop();
            ChangeUndoer.Undo(appData.ItineraryBuilder, lastChange);
            UpdateItinerary();
        }

        //queues
        private async void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = CustomerComboBox.SelectedItem as Customer;
            Country country = TourComboBox.SelectedItem as Country;

            if (customer == null || country == null)
            {
                MessageBox.Show("Zgjedhe klientin dhe shtetin.");
                return;
            }

            Tour tour = new Tour(
                country.Name,
                new Country[] { country }
            );

            List<Task> tasks = new List<Task>();

            Task task = Task.Run(() =>
            this.appData.BookingRequests.Enqueue((customer, tour))
            );

            tasks.Add(task);

            await Task.WhenAll(tasks);

            UpdateBookingQueue();

            MessageBox.Show($"Rezervimi per {country.Name} u shtua ne radhe.");
        }

        private void ProcessNext_Click(object sender, RoutedEventArgs e)
        {
            if (appData.BookingRequests.Count == 0)
            {
                MessageBox.Show("Nuk ka rezervime ne radhe.");
                return;
            }

            //var request = appData.BookingRequests.Dequeue();
            bool success = appData.BookingRequests.TryDequeue(out var request);
            if (success)
            {
                request.TheCustomer.BookedTours.Add(request.TheTour);

                UpdateBookingQueue();
            }
            MessageBox.Show(
                $"{request.TheCustomer.Name} rezervoi turin {request.TheTour.Name}."
            );
        }

        private void UpdateBookingQueue()
        {
            BookingQueueListBox.Items.Clear();

            foreach (var request in appData.BookingRequests)
            {
                BookingQueueListBox.Items.Add(
                    $"{request.TheCustomer.Name} - {request.TheTour.Name}"
                );
            }

            QueueCountTextBlock.Text =
                $"Waiting bookings: {appData.BookingRequests.Count}";
        }

        private void lbxToursToBoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTours = GetRequestedTours();
            StringBuilder sb = new StringBuilder();

            foreach (Tour tour in selectedTours)
            {
                sb.Append($"{tour.Name}");
                foreach (Country country in tour.Itineraty)
                    sb.AppendLine($"{country.Name}");
                sb.AppendLine();
            }
            this.tbxToursItinerary.Text = sb.ToString();
            this.lbcCountriesSelection.ItemsSource = GetCountireInSelection();
        }

        //Me hashSet
        //private HashSet<Country> GetCountireInSelection()
        //{
        //    var countries = new HashSet<Country>();

        //    List<Tour> selectedTours = GetRequestedTours();
        //    foreach(Tour tour in selectedTours)
        //    {
        //        foreach (Country country in tour.Itineraty)
        //            countries.Add(country);
        //    }
        //    //Por kjo nuk eshte menyra ma e mire se eshte Duplicate shume dhe per keta neve nuk na bon
        //    //return countries.Distinct().ToList();
        //    return countries;
        //}

        //private SortedSet<Country> GetCountireInSelection()
        //{
        //    //var countries = new SortedSet<Country>();
        //     var countries = new SortedSet<Country>(
        //        Comparer<Country>.Create(
        //        (a, b) => string.Compare(a.Name, b.Name)
        //        )
        //    );

        //    List<Tour> selectedTours = GetRequestedTours();
        //    foreach (Tour tour in selectedTours)
        //    {
        //        foreach (Country country in tour.Itineraty)
        //            countries.Add(country);
        //    }
        //    //Por kjo nuk eshte menyra ma e mire se eshte Duplicate shume dhe per keta neve nuk na bon
        //    //return countries.Distinct().ToList();
        //    return countries;
        //}

        //Metoda me UnionWith
        //private SortedSet<Country> GetCountireInSelection()
        //{
        //    //var countries = new SortedSet<Country>();
        //    var countries = new SortedSet<Country>(
        //       Comparer<Country>.Create(
        //       (a, b) => string.Compare(a.Name, b.Name)
        //       )
        //   );

        //    List<Tour> selectedTours = GetRequestedTours();
        //    foreach (Tour tour in selectedTours)
        //    {
        //        countries.UnionWith(tour.Itineraty);
        //    }
        //    //Por kjo nuk eshte menyra ma e mire se eshte Duplicate shume dhe per keta neve nuk na bon
        //    //return countries.Distinct().ToList();
        //    return countries;
        //}

        //Metoda IntersectWith
        private SortedSet<Country> GetCountireInSelection()
        {
            var countries = new SortedSet<Country>(
                Comparer<Country>.Create(
                    (a, b) => string.Compare(a.Name, b.Name)
                )
            );

            var selectedTours = GetRequestedTours().ToList();

            if (selectedTours.Count == 0)
                return countries;

            countries.UnionWith(selectedTours[0].Itineraty);

            foreach (Tour tour in selectedTours.Skip(1))
            {
                countries.IntersectWith(tour.Itineraty);
            }

            return countries;
        }
    }
}