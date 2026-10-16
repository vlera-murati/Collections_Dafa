using System;
using System.Collections.Generic;
using BookTourr;

namespace BookTourr.AppData
{
    public class AppData
    {
        public List<Country> AllCountries { get; private set; }

        public AppData(string csvFilePath)
        {
            CsvReader reader = new CsvReader(csvFilePath);

            AllCountries = reader.ReadAllCountries();
        }
    }
}