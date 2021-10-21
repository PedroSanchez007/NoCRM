using System;
using System.Collections.Generic;

namespace NoCRM.Models
{
    public class CapitaProspect
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int Surface { get; set; }
        public int Rooms { get; set; }
        public string EnergyClass { get; set; }
        public string Url { get; set; }
        public string LocalisationTown { get; set; }
        public string Department { get; set; }
        public string District { get; set; }
        public string Index { get; set; }
        public string Cost { get; set; }
        public string Phone { get; set; }
        public DateTime ParseDate { get; set; }
        public string Status { get; set; }
        // These fields aren't required.
        public string Furniture { get; set; }
        public bool Isbroken { get; set; }
        public string Ges { get; set; }
        public string Desciption { get; set; }
        public bool IsExported { get; set; }
        public string NoCRMSheetId { get; set; }
        public string NoCRMRecordId { get; set; }

        public string[] AsNoCrmPostRecord()
        {
            return new[]
            {
                Name ?? string.Empty,
                Id ?? string.Empty,
                Category ?? string.Empty,
                Date.ToString("d"),
                Type ?? string.Empty,
                Surface.ToString(),
                Rooms.ToString(),
                EnergyClass ?? string.Empty,
                LocalisationTown ?? string.Empty,
                Department ?? string.Empty,
                District ?? string.Empty,
                Index ?? string.Empty,
                Cost ?? string.Empty,
                Phone ?? string.Empty,
                ParseDate.ToString("d"),
                Status ?? string.Empty
            };
        }
    }

    public class RecordComparer : IEqualityComparer<CapitaProspect>
    {
        public bool Equals(CapitaProspect x, CapitaProspect y)
        {
            // if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            //     return false;
            //
            // if (ReferenceEquals(x, y)) 
            //     return true;

            return x.Phone == y.Phone;
        }
        
        public int GetHashCode(CapitaProspect capitaProspect)
        {
            var hashPhone = capitaProspect.Phone.GetHashCode();
            // var hashName = record.Name.GetHashCode();

            return hashPhone;           
        }
    }
}