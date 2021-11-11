using System;
using System.Collections.Generic;
using System.Globalization;

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
        public string PrefixedDistrict => "region_" + District;

        public string[] AsNoCrmPostRecord()
        {
            return new[]
            {
                Name ?? string.Empty,
                Id ?? string.Empty,
                Category ?? string.Empty,
                Date.ToString("d", new CultureInfo("en-GB")),
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
                ParseDate.ToString("d", new CultureInfo("en-GB")),
                Status ?? string.Empty
            };
        }
        
        public string FieldDoesNotMatch(NoCrmProspect crmProspect)
        {
            // There is no need to check Capital Id because by definition they are the same.
            // Do not check ParseData because a newer version does not mean that the data has changed.
            if (!Name.IsMatch(crmProspect.Name)) return $"Field name: {nameof(Name)}, Capita: {Name}, NoCRM: {crmProspect.Name}";
            if (!Category.IsMatch(crmProspect.Category)) return $"Field name: {nameof(Category)}, Capita: {Category}, NoCRM: {crmProspect.Category}";
            if (!Date.ToString(new CultureInfo("en-GB")).IsMatch(crmProspect.Date.ToString(new CultureInfo("en-GB")))) return $"Field name: {nameof(Date)}, Capita: {Date}, NoCRM: {crmProspect.Date}";
            if (!Type.IsMatch(crmProspect.Type)) return $"Field name: {nameof(Type)}, Capita: {Type}, NoCRM: {crmProspect.Type}";
            if (!Surface.ToString().IsMatch(crmProspect.Surface.ToString())) return $"Field name: {nameof(Surface)}, Capita: {Surface}, NoCRM: {crmProspect.Surface}";
            if (!Rooms.ToString().IsMatch(crmProspect.Rooms.ToString())) return $"Field name: {nameof(Rooms)}, Capita: {Rooms}, NoCRM: {crmProspect.Rooms}";
            if (!EnergyClass.IsMatch(crmProspect.EnergyClass)) return $"Field name: {nameof(EnergyClass)}, Capita: {EnergyClass}, NoCRM: {crmProspect.EnergyClass}";
            if (!LocalisationTown.IsMatch(crmProspect.LocalisationTown)) return $"Field name: {nameof(LocalisationTown)}, Capita: {LocalisationTown}, NoCRM: {crmProspect.LocalisationTown}";
            if (!Department.IsMatch(crmProspect.Department)) return $"Field name: {nameof(Department)}, Capita: {Department}, NoCRM: {crmProspect.Department}";
            if (!District.IsMatch(crmProspect.District)) return $"Field name: {nameof(District)}, Capita: {District}, NoCRM: {crmProspect.District}";
            if (!Index.IsMatch(crmProspect.Index)) return $"Field name: {nameof(Index)}, Capita: {Index}, NoCRM: {crmProspect.Index}";
            if (!Cost.IsMatch(crmProspect.Cost)) return $"Field name: {nameof(Cost)}, Capita: {Cost}, NoCRM: {crmProspect.Cost}";
            if (!Phone.IsMatch(crmProspect.Phone)) return $"Field name: {nameof(Phone)}, Capita: {Phone}, NoCRM: {crmProspect.Phone}";
            if (!Status.IsMatch(crmProspect.Status)) return $"Field name: {nameof(Status)}, Capita: {Status}, NoCRM: {crmProspect.Status}";

            return string.Empty;
        }
    }

    public class RecordComparer : IEqualityComparer<CapitaProspect>
    {
        public bool Equals(CapitaProspect x, CapitaProspect y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            
            if (ReferenceEquals(x, y)) 
                return true;

            return x.Phone == y.Phone;
        }
        
        public int GetHashCode(CapitaProspect capitaProspect)
        {
            // Do not change this hashing algorithm as it allows us to remove duplicates based on phone.
            var hashPhone = capitaProspect.Phone.GetHashCode();

            return hashPhone;           
        }
    }
}