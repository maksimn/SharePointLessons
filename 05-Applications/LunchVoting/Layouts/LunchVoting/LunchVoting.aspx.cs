using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;

namespace LunchVoting.Layouts.LunchVoting {
    public partial class LunchVoting : LayoutsPageBase {
        protected void Page_Load(object sender, EventArgs e) {
            if (IsVotingClosed()) {
                VotingClosedPanel.Visible = true;
            } else {
                InputPanel.Visible = true;
                Vote.Click += Vote_Click;
                AddLocation.NavigateUrl = "~/Lists/LunchLocations/NewForm.aspx?Source=" +
                    Server.UrlEncode(Request.RawUrl);
                if(!IsPostBack) {
                    PopulateSelectionList();
                }
            }
        }

        private void PopulateSelectionList() {
            var lookupList = SPContext.Current.Web.Lists["Lunch Locations"];
            var query = new SPQuery();
            query.Query = "<OrderBy><FieldRef Name='Title' Ascending='false' /></OrderBy>";
            foreach(SPListItem result in lookupList.GetItems(query)) {
                Selection.Items.Add(new ListItem(result["Title"].ToString(), result.ID.ToString()));
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

        private void ShowMessage(string message) {
            InputPanel.Visible = false;
            MessagePanel.Visible = true;
            Message.Text = message;
        }

        void Vote_Click(object sender, EventArgs e) {
            var list = SPContext.Current.Web.Lists["Lunch Voting"];
            var currentUser = SPContext.Current.Web.CurrentUser.Name;
            var votingUser = OnBehalfOf.AllEntities.Count > 0 ? OnBehalfOf.AllEntities[0].DisplayText : currentUser;
            var directVote = GetDirectVote(list, votingUser);
            var onBehalfVote = GetOnBehalfVote(list, votingUser);
            var existingVote = directVote != null || onBehalfVote != null;
            var isOnBehalfVote = !string.Equals(currentUser, votingUser, StringComparison.InvariantCultureIgnoreCase);

            if(isOnBehalfVote && directVote != null) {
                ShowMessage(string.Format("You cannot cast a vote for {0} because they have already voted.", votingUser));
            } else {
                if (directVote != null) directVote.Delete();
                if (onBehalfVote != null) onBehalfVote.Delete();
                var vote = list.Items.Add();
                vote["Selection"] = new SPFieldLookupValue(int.Parse(Selection.SelectedItem.Value), Selection.SelectedItem.Text);
                if (isOnBehalfVote) {
                    // Если в поле OnBehalfOf ввести неправильное имя, то здесь возникнет необработанное исключение
                    // SPException: The specified user We couldn't find an exact match. could not be found.
                    vote["OnBehalfOf"] = SPContext.Current.Web.EnsureUser(OnBehalfOf.AllEntities[0].Description);
                }
                vote.Update();

                if (existingVote) {
                    if (isOnBehalfVote) {
                        ShowMessage(string.Format("{0}'s vote has been recast for {1}", votingUser, Selection.SelectedItem.Text));
                    } else {
                        ShowMessage(string.Format("Your vote has been recast for {0}", Selection.SelectedItem.Text));
                    }
                } else {
                    if (isOnBehalfVote) {
                        ShowMessage(string.Format("{0}'s vote has been cast for {1}", votingUser, Selection.SelectedItem.Text));
                    } else {
                        ShowMessage(string.Format("Your vote has been cast for {0}", Selection.SelectedItem.Text));
                    }
                }
            }
        }

        private bool IsVotingClosed() {
            var current = DateTime.Now;
            if (!string.IsNullOrEmpty(Request.QueryString["time"])) {
                DateTime.TryParse(Request.QueryString["time"], out current);
            }
            return current.Hour > 10;
        }
    }
}
