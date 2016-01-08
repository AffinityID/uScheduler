using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uScheduler.Models {
    [TableName(SchedulerConstants.Database.LogTable)]
    public class Log {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Result { get; set; }
    }
}