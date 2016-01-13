using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uScheduler.Models {
    [TableName(SchedulerConstants.Database.ScheduleTable)]
    public class Schedule {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        [Length(200)]
        public string Name { get; set; }
        [Length(2000)]
        public string Url { get; set; }
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Data { get; set; }
        [Length(100)]
        public string HttpVerb { get; set; }
        [Length(100)]
        public string Frequency { get; set; }
        public DateTime NextRun { get; set; }
        public bool Disabled { get; set; }
    }
}