using System;
using System.Collections.Generic;
using System.Text;

namespace BookTourr.Logic
{
    public enum ChangeType { Append, Insert, Remove}
    public class ItineraryChange
    {
        public ChangeType ChangeType { get; }

        public Country Value { get; }

        public int Index { get; }

        public ItineraryChange(ChangeType changeType,int index, Country countryRemoved)
        {
            this.ChangeType = changeType;
            this.Index = index;
            this.Value = countryRemoved;
        }
    }
}
