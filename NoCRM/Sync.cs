using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using NoCRM.Models;
using Serilog;

namespace NoCRM
{
    public class Sync
    {
        public static string Separator => "_";
        private const int ProspectingListCapacity = 4999;
        private const int MaxInsertCount = 100;

        private readonly List<NoCrmProspect> _crmRecords;
        private readonly List<CapitaProspect> _inserts;
        private readonly string _targetDistrict;
        private readonly NoCrmProspect _crmProspectOfTargetDistrict;
        
        public Sync()
        {
            var capitaRecords = CapitaCommunication.GetNewData().OrderByDescending(_ => _.District).ToList();
            
            ////////////////////////////////////// for testing
            //var test = NoCrmCommunication.Ping();
            //capitaRecords = capitaRecords.Skip(0).Take(6000).ToList();
            //ExcelWriting.ExportRecords(capitaRecords.ToImmutableArray(), @"Capita Export.csv");
            //////////////////////////////////////
             
            Log.Information($"Got { capitaRecords.Count } new Prospects from Capita");
            _crmRecords = NoCrmCommunication.GetAllProspects().ToList();
            Log.Information($"Got { _crmRecords.Count } Prospects from NoCRM");
            
            var excludedIds = new HashSet<string>(_crmRecords.Select(crm => crm.CapitaId));
            _inserts = capitaRecords.Where(cap => !excludedIds.Contains(cap.Id)).ToList();
            Log.Information($"{ _inserts.Count } Prospects are inserts");
            
            IList<CapitaProspect> alreadyInCrmRecords = capitaRecords.Where(cap => excludedIds.Contains(cap.Id)).ToList();
            Log.Information($"{ alreadyInCrmRecords.Count } Prospects are already in NoCRM");

            var updates = new List<CapitaProspect>();
            foreach (var capitalRecord in alreadyInCrmRecords)
            {
                var matchingCrmRecord = _crmRecords.First(_ => _.CapitaId == capitalRecord.Id);
                var fieldThatDoesNotMatch = capitalRecord.FieldDoesNotMatch(matchingCrmRecord);
                if (fieldThatDoesNotMatch != string.Empty)
                {
                    updates.Add(capitalRecord);
                    Log.Information($"NoCrm [ProspectingList: {matchingCrmRecord.ProspectingListTitle}] does not match Capita Prospect [CapitaId: {capitalRecord.Id}, Nom: {matchingCrmRecord.Name}] because {fieldThatDoesNotMatch} is different:");
                }
            }

            // Insert Prospects
            Log.Information($"Inserting { _inserts.Count } Prospects to NoCRM");
            var infiniteLoopBrakeCounter = 0;
            while (_inserts.Any())
            {
                if (infiniteLoopBrakeCounter++ > 10000)
                {
                    Log.Error("Sync was stuck in an infinite loop");
                    break;
                }

                // I haven't tested this yet
                _targetDistrict = _inserts.First().District;
                _crmProspectOfTargetDistrict = CrmProspectOfTargetDistrict;
                if (CrmContainsTargetDistrict)
                {
                    var targetProspectingListId = GetLastProspectingListIdOfTargetDistrict();
                    var targetProspectingListTitle = GetLastProspectingListTitleOfTargetDistrict();
                    if (ProspectingListIsFull(targetProspectingListId))
                    {
                        CreateAllProspectingLists(targetProspectingListTitle);
                    }
                    else
                    {
                        AddToProspectingList(targetProspectingListId);
                        CreateAllProspectingLists(targetProspectingListTitle);
                    }
                }
                else
                {
                    CreateAllProspectingLists($"{Separator} 0");
                }
            }
            
            // Update Prospects
            Log.Information($"Updating { updates.Count } Prospects to NoCRM");
            foreach (var capitaProspectToUpdate in updates)
            {
                var crmProspect = _crmRecords.FirstOrDefault(crm => crm.CapitaId == capitaProspectToUpdate.Id);
                var response = NoCrmCommunication.UpdateProspect(crmProspect.ProspectingListId, crmProspect.Id, capitaProspectToUpdate);
            }
        }

        private bool CrmContainsTargetDistrict => _crmProspectOfTargetDistrict != null;
        private NoCrmProspect CrmProspectOfTargetDistrict => _crmRecords.FirstOrDefault(x => x.ProspectingListDistrictName == _targetDistrict);
        private int NumberOfRecordsToInsert => _inserts.Count(x => x.District == _targetDistrict);

        private void CreateAllProspectingLists(string fullUpProspectingListTitle)
        {
            if (NumberOfRecordsToInsert <= 0)
                return;
            
            var bound = (NumberOfRecordsToInsert - 1) / ProspectingListCapacity + 1;
            for (var i = 0; i < bound; i++)
            {
                var newProspectingListNumber = fullUpProspectingListTitle.AfterSeparator() + 1 + i;
                var newProspectingListTitle = $"{_targetDistrict} {Separator} {newProspectingListNumber}";
                var cappedRecordsToInsert = GetCappedRecordsToInsert(ProspectingListCapacity);
                var response = NoCrmCommunication.CreateProspectingList(cappedRecordsToInsert, newProspectingListTitle);
                RemoveFromInserts(cappedRecordsToInsert);
            }
        }
        
        private void AddToProspectingList(int prospectingListId)
        {
            var remainingListCapacity = GetRemainingListCapacity(prospectingListId);
            while (remainingListCapacity > 0)
            {
                var recordsToInsertCount = remainingListCapacity > MaxInsertCount ? MaxInsertCount : remainingListCapacity;
                var cappedRecordsToInsert = GetCappedRecordsToInsert(recordsToInsertCount);
                if (!cappedRecordsToInsert.Any())
                    break;
                var response =  NoCrmCommunication.AddProspectsToProspectingList(prospectingListId, cappedRecordsToInsert);
                RemoveFromInserts(cappedRecordsToInsert);
                remainingListCapacity -= cappedRecordsToInsert.Count;
            }
        }
        
        private List<CapitaProspect> GetCappedRecordsToInsert(int numberOfRecordsToInsert)
        {
            return _inserts
                .Where(x => x.District == _targetDistrict)
                .Take(numberOfRecordsToInsert).ToList();
        }

        private int GetRemainingListCapacity(int prospectingListId)
        {
            return ProspectingListCapacity - NumberOfProspectsInList(prospectingListId);
        }

        private int GetLastProspectingListIdOfTargetDistrict()
        {
            var crmProspectInLastProspectingListForTargetDistrict = _crmRecords.Aggregate((curMax, x) =>
                x.ProspectingListDistrictName == _targetDistrict && x.ProspectingListDistrictNumber >= curMax.ProspectingListDistrictNumber ? x : curMax);
            return crmProspectInLastProspectingListForTargetDistrict.ProspectingListId;
        }
        
        private string GetLastProspectingListTitleOfTargetDistrict()
        {
            var crmProspectInLastProspectingListForTargetDistrict = _crmRecords.Aggregate((curMax, x) =>
                x.ProspectingListDistrictName == _targetDistrict && x.ProspectingListDistrictNumber >= curMax.ProspectingListDistrictNumber ? x : curMax);
            return crmProspectInLastProspectingListForTargetDistrict.ProspectingListTitle;
        }
        
        private int NumberOfProspectsInList(int prospectingListId)
        {
            return _crmRecords.Count(x => x.ProspectingListId == prospectingListId);
        }
        
        private bool ProspectingListIsFull(int prospectingListId)
        {
            return _crmRecords.Count(x => x.ProspectingListId == prospectingListId) >= ProspectingListCapacity;
        }

        private void RemoveFromInserts(IEnumerable<CapitaProspect> prospectsToRemove)
        {
            var idsToRemove = new HashSet<string>(prospectsToRemove.Select(_ => _.Id));
            _inserts.RemoveAll(_ => idsToRemove.Contains(_.Id));
        }
    }
}