using System;
using System.Threading;
using log4net;
using umbraco.businesslogic;
using umbraco.interfaces;
using uScheduler.Models;
using uScheduler.Services;
using uScheduler.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uScheduler {
    [Application("uScheduler", "uScheduler", "icon-time", 10)]
    public class SchedulerApplication : ApplicationEventHandler, IApplication {
        protected static Timer Ticker;

        private readonly ILog _log;
        protected readonly IScheduleRunner Runner;

        public SchedulerApplication() {
            _log = LogManager.GetLogger(GetType());
            Runner = new ScheduleRunner(ApplicationContext.Current);
        }

        protected override void ApplicationStarted(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext
            ) {
            var db = applicationContext.DatabaseContext.Database;
            db.CreateTable<Schedule>(false);
            db.CreateTable<Log>(false);

            _log.Info("Starting ticker.");
            Ticker = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void Tick(object state) {
            try {
                Runner.Run();
                _log.Debug("Tick completed successfully.");
            }
            catch (Exception ex) {
                _log.Error("Tick failed to complete.", ex);
            }
        }
    }
}