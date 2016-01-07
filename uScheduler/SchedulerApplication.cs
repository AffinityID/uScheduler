using umbraco.businesslogic;
using umbraco.interfaces;
using uScheduler.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uScheduler {
    [Application("uScheduler", "uScheduler", "icon-time", 10)]
    public class SchedulerApplication : ApplicationEventHandler, IApplication {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
            var db = applicationContext.DatabaseContext.Database;
             db.CreateTable<Schedule>(false);
        }
    }
}