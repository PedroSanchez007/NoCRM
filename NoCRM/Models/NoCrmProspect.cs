using System;
using System.Collections.Generic;
using Serilog;

namespace NoCRM.Models
{
    public class NoCrmProspect
    {
        public int ProspectingListId { get; set; }
        public string ProspectingListTitle { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string CapitaId { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int Surface { get; set; }
        public int Rooms { get; set; }
        public string EnergyClass { get; set; }
        public string LocalisationTown { get; set; }
        public string Department { get; set; }
        public string District { get; set; }
        public string Index { get; set; }
        public string Cost { get; set; }
        public string Phone { get; set; }
        public DateTime ParseDate { get; set; }
        public string Status { get; set; }
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
            Date = raw[3].ConvertToDateTime();
            Type = raw[4];
            Surface = raw[5].ConvertToInt();
            Rooms = raw[6].ConvertToInt();
            EnergyClass = raw[7];
            LocalisationTown = raw[8];
            Department = raw[9];
            District = raw[10];
            Index = raw[11];
            Cost = raw[12];
            Phone = raw[13];
            ParseDate = raw[14].ConvertToDateTime();
            Status = raw.Count < 16 ? string.Empty : raw[15];
        }
    }
}