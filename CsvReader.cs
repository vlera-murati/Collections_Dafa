using System.Collections.Generic;
using System.IO;

namespace BookTourr
{
    public class CsvReader
    {
        private string _csvFilePath;

        public CsvReader(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        public List<Country> ReadAllCountries()
        {
            List<Country> countries = new List<Country>();

            using (StreamReader sr = new StreamReader(_csvFilePath))
            {
                sr.ReadLine();

                string csvLine;

                while ((csvLine = sr.ReadLine()) != null)
                {
                    countries.Add(ReadCountryFromCsvLine(csvLine));
                }
            }

            return countries;
        }

        private Country ReadCountryFromCsvLine(string csvLine)
        {
            string[] values = csvLine.Split(',');

            return new Country(
                values[0],
                values[1],
                values[2],
                int.Parse(values[3])
            );
        }
    }

}

