using System;
using System.Collections.Generic;
using System.Text;

namespace BookTourr.Logic
{
    public class Tour
    {
        public string Name { get; }

        public Country[] Itineraty { get; }

        public override string ToString() =>
            $"{Name} ({Itineraty.Length} countries)";

        public Tour(string name, Country[] itineraty)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Emri i vizitave nuk duhet te kete hapsire",nameof(name));
            }
            else if(itineraty == null)
            {
                throw new ArgumentException("Nuk mundet te jete null", nameof(itineraty));
            }
            else if(itineraty.Length == 0)
            {
                throw new ArgumentException("Tour must have at least one country");
            }

            this.Name = name;
            this.Itineraty = itineraty;
        }
    }
}
