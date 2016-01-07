using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace uScheduler.Controllers {
    [Tree("uScheduler", "uScheduler", "uScheduler")]
    [PluginController("uScheduler")]
    public class SchedulerTreeController : TreeController {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings) {
            var nodes = new TreeNodeCollection();
            var activeIcon = "icon-timer color-green";
            var disabledIcon = "icon-timer color-red";

            nodes.Add(CreateTreeNode("0", "-1", queryStrings, "Test", activeIcon));

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings) {
            throw new System.NotImplementedException();
        }
    }
}