using PersonRecordService.Interfaces;
using RecordProcesssor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRecordService
{
    public class DynamicRecordRepository : RecordRepositoryBase, IRecordRepository
    {
        static DynamicRecordRepository()
        {
            _records = new List<PersonRecord>();
        }
    }
}
