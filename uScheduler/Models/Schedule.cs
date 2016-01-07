using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uScheduler.Models {
    [TableName("uScheduler_Schedules")]
    public class Schedule {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        public string HttpVerb { get; set; }
        public string Frequency { get; set; }
        public DateTime NextRun { get; set; }
        public bool Disabled { get; set; }
    }
}