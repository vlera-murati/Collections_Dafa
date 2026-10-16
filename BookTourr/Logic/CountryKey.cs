using System;

namespace BookTourr.Logic
{
    public class CountryKey : IComparable<CountryKey>
    {
        public string Code { get; set; }

        public int CompareTo(CountryKey other)
        {
            if (other == null)
                return 1;

            return string.Compare(
                Code,
                other.Code,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}