using RecordProcesssor.Model;
using System.Collections.Generic;

namespace PersonRecordService.Interfaces
{
    public interface IRecordRepository
    {
       List<PersonRecord> GetRecords(string orderBy); 
       void SaveRecord(string record); 
    }
}
