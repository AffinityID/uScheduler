using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public DateTime NextRunUtc { get; set; }
        public bool Disabled { get; set; }

        [Column("Headers")]
        [JsonIgnore]
        public string HeadersString { get; set; }
        [Ignore]
        public IDictionary<string, string> Headers {
            get { return HeadersString == null ? null : JsonConvert.DeserializeObject<IDictionary<string, string>>(HeadersString); }
            set { HeadersString = JsonConvert.SerializeObject(value); }
        }
    }
}