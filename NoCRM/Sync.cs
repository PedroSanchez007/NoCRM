using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NoCRM.Models;
using Serilog;

namespace NoCRM
{
    public class Sync
    {
        private const int ProspectingListCapacity = 4999;
        //private const int ProspectingListCapacity = 2;

        private readonly List<NoCrmProspect> _crmRecords;
        private readonly List<CapitaProspect> _inserts;
        private readonly string _targetDistrict;
        private readonly NoCrmProspect _crmProspectOfTargetDistrict;
        
        public Sync()
        {
            var capitaRecords = CapitaCommunication.GetNewData().OrderBy(_ => _.District).ToList();
            // for testing
            //capitaRecords = capitaRecords.Skip(0).Take(50).ToList();
            ExcelWriting.ExportRecords(capitaRecords.ToImmutableArray(), @"Capita Export.csv");
            //////////////////////////                                     
            Log.Information($"Got { capitaRecords.Count } new Prospects from Capita");
            _crmRecords = NoCrmCommunication.GetAllProspects().ToList();
            Log.Information($"Got { _crmRecords.Count() } Prospects from NoCRM");
            
            var excludedIds = new HashSet<string>(_crmRecords.Select(crm => crm.CapitaId));
            _inserts = capitaRecords.Where(cap => !excludedIds.Contains(cap.Id)).ToList();
            Log.Information($"{ _inserts.Count() } Prospects are inserts");
            
            IList<CapitaProspect> updates = capitaRecords.Where(cap => excludedIds.Contains(cap.Id)).ToList();
            Log.Information($"{ updates.Count() } Prospects are updates");

            // Insert Prospects
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
                    if (ProspectingListIsFull(targetProspectingListId))
                    {
                        CreateAllProspectingLists(targetProspectingListId);
                    }
                    else
                    {
                        var response = AddToProspectingList(targetProspectingListId++);
                        CreateAllProspectingLists(targetProspectingListId);
                    }
                }
                else
                {
                    CreateAllProspectingLists(1);
                }
            }
            
            // Update Prospects
            foreach (var capitaProspectToUpdate in updates)
            {
                var crmProspect = _crmRecords.FirstOrDefault(crm => crm.CapitaId == capitaProspectToUpdate.Id);
                var response = NoCrmCommunication.UpdateProspect(crmProspect.ProspectingListId, crmProspect.Id, capitaProspectToUpdate);
            }
        }

        private bool CrmContainsTargetDistrict => _crmProspectOfTargetDistrict != null;
        private NoCrmProspect CrmProspectOfTargetDistrict => _crmRecords.FirstOrDefault(x => x.ProspectingListDistrictName == _targetDistrict);
        private int NumberOfRecordsToInsert => _inserts.Count(x => x.District == _targetDistrict);

        private void CreateAllProspectingLists(int newProspectingListId)
        {
            var bound = (NumberOfRecordsToInsert - 1) / ProspectingListCapacity + 1;
            for (var i = 0; i < bound; i++)
            {
                var newProspectingListTitle = $"{_targetDistrict} {ExtensionMethods.Separator} {newProspectingListId++}";
                var cappedRecordsToInsert = GetCappedRecordsToInsert(ProspectingListCapacity);
                var response = NoCrmCommunication.CreateProspectingList(cappedRecordsToInsert, newProspectingListTitle);
                RemoveFromInserts(cappedRecordsToInsert);
            }
        }
        
        private string AddToProspectingList(int prospectingListId)
        {
            var remainingListCapacity = GetRemainingListCapacity(prospectingListId);
            var cappedRecordsToInsert = GetCappedRecordsToInsert(remainingListCapacity);
            var response =  NoCrmCommunication.AddProspectsToProspectingList(prospectingListId, cappedRecordsToInsert);
            RemoveFromInserts(cappedRecordsToInsert);
            return response;
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
                x.ProspectingListDistrictName == _targetDistrict && x.ProspectingListDistrictNumber > curMax.ProspectingListDistrictNumber ? x : curMax);
            return crmProspectInLastProspectingListForTargetDistrict.ProspectingListId;
        }
        
        private int NumberOfProspectsInList(int prospectingListId)
        {
            return _crmRecords.Select(x => x.ProspectingListId == prospectingListId).Count();
        }
        
        private bool ProspectingListIsFull(int prospectingListId)
        {
            return _crmRecords.Select(x => x.ProspectingListId == prospectingListId).Count() >= ProspectingListCapacity;
        }

        private void RemoveFromInserts(IEnumerable<CapitaProspect> prospectsToRemove)
        {
            var idsToRemove = new HashSet<string>(prospectsToRemove.Select(_ => _.Id));
            _inserts.RemoveAll(_ => idsToRemove.Contains(_.Id));
        }
    }
}