using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace uScheduler.Controllers {
    [Tree(SchedulerConstants.Application.Name, SchedulerConstants.Application.Name, SchedulerConstants.Application.Name)]
    [PluginController(SchedulerConstants.Application.Name)]
    public class SchedulerTreeController : TreeController {
        private readonly ILocalizedTextService _textService;
        private readonly SchedulerApiController _schedulerApi;

        public SchedulerTreeController() {
            _textService = ApplicationContext.Services.TextService;
            _schedulerApi = new SchedulerApiController();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings) {
            var nodes = new TreeNodeCollection();
            const string activeIcon = "icon-timer color-green";
            const string disabledIcon = "icon-timer color-red";

            var schedules = _schedulerApi.GetAllSchedules();
            nodes.AddRange(
                schedules.Select(s => 
                    CreateTreeNode(
                        s.Id.ToString(), 
                        Constants.System.Root.ToString(), 
                        queryStrings,
                        s.Name,
                        s.Disabled ? disabledIcon : activeIcon)));

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings) {
            var menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToString()) {
                menu.Items.Add<CreateChildEntity, ActionNew>(GetLocalizedText("actions", ActionNew.Instance.Alias));
                menu.Items.Add<RefreshNode, ActionRefresh>(GetLocalizedText("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }

            menu.Items.Add<ActionDelete>(GetLocalizedText("actions", ActionDelete.Instance.Alias));
            return menu;
        }

        private string GetLocalizedText(string area, string key) {
            return _textService.Localize($"{area}/{key}", CultureInfo.CurrentCulture);
        }
    }
}