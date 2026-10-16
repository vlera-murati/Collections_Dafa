using BookTourr;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Collections.Concurrent;
using System.Security.RightsManagement;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace BookTourr.Logic
{
    public class AppData
    {
        //public List<Country> AllCountries { get; private set; }

        //Pjesa per readOnly
        //public ReadOnlyCollection<Country> AllCountries { get; private set; }

        public IReadOnlyCollection<Country> AllCountries { get; private set; }


        //Pjesa per ImmutableList
        //public ImmutableList<Country> AllCountries { get; private set; }


        //Kemi dy lloje te sortimi sortimi ne baze te SortedDictionary

        //Pjesa per readOnly
        private readonly SortedDictionary<CountryKey, Country> _allCountriesByKey
    = new SortedDictionary<CountryKey, Country>();

        public IReadOnlyDictionary<CountryKey, Country> AllCountriesByKey
            => _allCountriesByKey;

        //Pjesa e Immutable
    //    public ImmutableSortedDictionary<CountryKey, Country> AllCountriesByKey
    //    { get; private set; }
    //= ImmutableSortedDictionary<CountryKey, Country>.Empty;

        //Kemi sortimin ne baze te sorted List
        //public SortedList<CountryKey, Country> AllCountriesByKey { get; private set; }

        public LinkedList<Country> ItineraryBuilder { get; } = new LinkedList<Country>();

        public SortedDictionary<string, Tour> AllTours { get; private set; }
            = new SortedDictionary<string, Tour>();

        public Stack<ItineraryChange> ChangeLog { get;}
        = new Stack<ItineraryChange>();

        public Stack<ItineraryChange> ChangeeLog { get; }
        = new Stack<ItineraryChange>();

        public List<Customer> Customers { get; private set; }
        = new List<Customer>()
        {
            new Customer("Vlera"), new Customer("Frasher") };

        //Per pjesen e queue qe eshte normale para se me vendos
        //public Queue<(Customer TheCustomer, Tour TheTour)>
        //    BookingRequests
        //{ get; } = new Queue<(Customer, Tour)>();

        public ConcurrentQueue<(Customer TheCustomer, Tour TheTour)>
            BookingRequests
        { get; } = new ConcurrentQueue<(Customer, Tour)>();




        public AppData(string csvFilePath)
        {
            CsvReader reader = new CsvReader(csvFilePath);

            //Eshte gjithmone e rendesishme qe te jete performnaca me e mire ne memorie kur shkojm edhe i
            //Ruajm te dhenat
            var countries = reader.ReadAllCountries().OrderBy(x => x.Name).ToList();
            this.AllCountries = countries.AsReadOnly();
            //this.AllCountries = countries.ToImmutableList();
    
            countries.Add(new Country("Zanzibar", "ZI", "Europe", 1775378));


            //this.AllCountriesByKey = new SortedDictionary<CountryKey, Country>();

            foreach (Country country in AllCountries)
            {
                CountryKey key = new CountryKey
                {
                    Code = country.Code
                };

                //AllCountriesByKey.Add(key, country);
                _allCountriesByKey.Add(key, country);
                //AllCountriesByKey = AllCountriesByKey.Add(key, country);
            }

        }

    }
}