using System;
using System.Collections.Generic;
using System.Globalization;
using Serilog;

namespace NoCRM.Models
{
    public class NoCrmProspect
    {
        public int ProspectingListId { get; }
        public string ProspectingListTitle { get; }
        public string Name { get; }
        public int Id { get; }
        public string CapitaId { get; }
        public string Category { get; }
        public DateTime Date { get; }
        public string Type { get; }
        public int Surface { get; }
        public int Rooms { get;  }
        public string EnergyClass { get;  }
        public string LocalisationTown { get; }
        public string Department { get; }
        public string District { get; }
        public string Index { get; }
        public string Cost { get; }
        public string Phone { get; }
        public DateTime ParseDate { get; }
        public string Status { get; }
        public string ProspectingListDistrictName => ProspectingListTitle.BeforeSeparator();
        public int ProspectingListDistrictNumber => ProspectingListTitle.AfterSeparator();

        public NoCrmProspect(int prospectingListId, string prospectingListTitle, int id, IReadOnlyList<string> raw)
        {
            if (raw.Count < 15)
            {
                Log.Error($"Prospect was returned from NoCRM with {raw.Count} data fields instead of 16. Prospect List Name: {prospectingListTitle}, Prospect Id: {Id}");
                return;
            }
            ProspectingListTitle = prospectingListTitle;
            ProspectingListId = prospectingListId;
            Id = id;
            Name = raw[0];
            CapitaId = raw[1];
            Category = raw[2];
            Date = DateTime.Parse(raw[3], new CultureInfo("en-GB"));
            Type = raw[4];
            Surface = ConvertToInt(raw[5]);
            Rooms = ConvertToInt(raw[6]);
            EnergyClass = raw[7];
            LocalisationTown = raw[8];
            Department = raw[9];
            District = raw[10];
            Index = raw[11];
            Cost = raw[12];
            Phone = raw[13];
            ParseDate = DateTime.Parse(raw[14], new CultureInfo("en-GB"));
            Status = raw.Count < 16 ? string.Empty : raw[15];
        }
        
        private static int ConvertToInt(string input)
        {
            if (int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var convertedInteger))
            {
                return convertedInteger;
            }
            Log.Error($"Could not convert {input} to an integer");
            return default;
        }
    }
}