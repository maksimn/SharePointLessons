using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Linq;
using Microsoft.SharePoint;

namespace LunchVoting.Features.LunchVoting {
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("17cd2593-f69d-4e0d-936f-c8cbc8da7602")]
    public class LunchVotingEventReceiver : SPFeatureReceiver {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;
            SPList list = site.RootWeb.Lists["Lunch Voting"];
            Type eventReceiverType = typeof(LunchVotingListEventReceiver);
            string assemblyName = eventReceiverType.Assembly.FullName;
            string className = eventReceiverType.FullName;

            list.EventReceivers.Add(SPEventReceiverType.ItemAdding, assemblyName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemUpdating, assemblyName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, assemblyName, className);
            list.Update();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;
            SPList list = site.RootWeb.Lists["Lunch Voting"];
            Type eventReceiverType = typeof(LunchVotingListEventReceiver);
            string assemblyName = eventReceiverType.Assembly.FullName;
            string className = eventReceiverType.FullName;

            list.EventReceivers.Cast<SPEventReceiverDefinition>()
                .Where(i => i.Assembly == assemblyName && i.Class == className)
                .ToList()
                .ForEach(i => i.Delete());
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
