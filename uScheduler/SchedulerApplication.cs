using System;
using System.Threading;
using System.Web;
using System.Xml;
using log4net;
using umbraco;
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

        protected override void ApplicationInitialized(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext
            ) {
            UpdateLocalization("en.xml", "sections", "uScheduler", "uScheduler");
            UpdateLocalization("en_us.xml", "sections", "uScheduler", "uScheduler");
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

        private void UpdateLocalization(string languageFile, string area, string key, string value) {
            var umbracoPath = GlobalSettings.Path;
            var doc = XmlHelper.OpenAsXmlDocument($"{umbracoPath}/config/lang/{languageFile}");

            var areaNode = doc.SelectSingleNode($"//area[@alias='{area}']");
            if (areaNode == null || areaNode.SelectSingleNode($"key[@alias='{key}']") != null) return;

            var node = areaNode.AppendChild(doc.CreateElement("key"));
            if (node.Attributes == null)
                throw new XmlException($"Error creating the key element in {languageFile} -> {area}");

            var attr = node.Attributes.Append(doc.CreateAttribute("alias"));
            attr.InnerText = key;
            node.InnerText = value;

            doc.Save(HttpContext.Current.Server.MapPath($"{umbracoPath}/config/lang/{languageFile}"));
            _log.Info($"Successfully update language file {languageFile} with area: {area}, key: {key}, value: {value}");
        }
    }
}