using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace LunchVoting {
    /// <summary>
    /// List Item Events
    /// </summary>
    public class LunchVotingListEventReceiver : SPItemEventReceiver {
        public override void ItemAdding(SPItemEventProperties properties) {
            object onBehalfOf = properties.AfterProperties["OnBehalfOf"]; // Значение поля после события
            // properties.BeforeProperties["field"] - до события.
            SPFieldUserValue onBehalfUser = onBehalfOf == null || String.IsNullOrEmpty(onBehalfOf.ToString()) ?
                null : new SPFieldUserValue(properties.Web, onBehalfOf.ToString());
            SPUser onBehalfUserSP = onBehalfUser != null ? properties.Web.EnsureUser(onBehalfUser.LookupValue) : null;
            string votingUser = onBehalfUserSP != null ? onBehalfUserSP.Name : properties.Web.CurrentUser.Name;
            SPListItem directVote = GetDirectVote(properties.List, votingUser);
            SPListItem onBehalfVote = GetOnBehalfVote(properties.List, votingUser);

            if (directVote != null || onBehalfVote != null) {
                properties.Cancel = true;
                properties.ErrorMessage = "Vote for this user already exists. You cannot vote twice in one day.";
            }

        }

        public override void ItemUpdating(SPItemEventProperties properties) {
            if (properties.UserDisplayName != 
                    new SPFieldUserValue(properties.Web, properties.ListItem["Author"].ToString()).LookupValue) {
                properties.Cancel = true;
                properties.ErrorMessage = "You cannot change a vote that you did not create. Please user the voting page.";
            }
        }

        public override void ItemDeleting(SPItemEventProperties properties) {
            if (properties.UserDisplayName !=
                    new SPFieldUserValue(properties.Web, properties.ListItem["Author"].ToString()).LookupValue) {
                properties.Cancel = true;
                properties.ErrorMessage = "You cannot delete a vote that you did not create. Please user the voting page.";
            }
        }

        private SPListItem GetDirectVote(SPList list, string user) {
            var query = new SPQuery() {
                Query = String.Format(
                    @"<Where>
                          <And>
                             <Geq>
                                <FieldRef Name='Created' />
                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{0}</Value>
                             </Geq>
                             <And>
                                <Eq>
                                   <FieldRef Name='Author' />
                                   <Value Type='User'>{1}</Value>
                                </Eq>
                                <IsNull>
                                   <FieldRef Name='OnBehalfOf' />
                                </IsNull>
                             </And>
                          </And>
                       </Where>", SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.Date), user
                ),
                ViewFields = "<FieldRef Name='Selection' /><FieldRef Name='Author' /><FieldRef Name='OnBehalfOf' />",
                ViewFieldsOnly = true
            };

            var itemList = list.GetItems(query);
            return itemList.Count > 0 ? itemList[0] : null;
        }

        private SPListItem GetOnBehalfVote(SPList list, string user) {
            var query = new SPQuery() {
                Query = String.Format(@"<Where>
                      <And>
                         <Geq>
                            <FieldRef Name='Created' />
                            <Value IncludeTimeValue='TRUE' Type='DateTime'>{0}</Value>
                         </Geq>
                         <Eq>
                            <FieldRef Name='OnBehalfOf' />
                            <Value Type='User'>{1}</Value>
                         </Eq>
                      </And>
                   </Where>", SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.Date), user),
                ViewFields = "<FieldRef Name='Selection' /><FieldRef Name='Author' /><FieldRef Name='OnBehalfOf' />",
                ViewFieldsOnly = true
            };

            var itemList = list.GetItems(query);
            return itemList.Count > 0 ? itemList[0] : null;
        }

    }
}