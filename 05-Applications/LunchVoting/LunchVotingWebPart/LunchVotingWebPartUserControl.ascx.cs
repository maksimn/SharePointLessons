using LunchVoting.Classes;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace LunchVoting.LunchVotingWebPart {
    public partial class LunchVotingWebPartUserControl : UserControl {
        LunchVotingWebPart _parent;

        protected void Page_Load(object sender, EventArgs e) {
            _parent = this.Parent as LunchVotingWebPart;

            GoVoteLink.NavigateUrl = SPContext.Current.Site.Url + "/_layouts/15/LunchVoting/LunchVoting.aspx";
            IList<LunchVoteData> voteData = GetLunchVoteData();

            if (voteData.Count == 0) {
                NoResultsPanel.Visible = true;
                TopLunchPicks.Visible = false;
            } else {
                TopLunchPicks.ItemDataBound += TopLunchPicks_ItemDataBound;
                TopLunchPicks.DataSource = voteData;
                TopLunchPicks.DataBind();
            }
        }

        private void TopLunchPicks_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            
        }

        private IList<LunchVoteData> GetLunchVoteData() {
            SPList list = SPContext.Current.Web.Lists["Lunch Voting"];
            LunchVoteDataCollector dataCollector = new LunchVoteDataCollector();
            foreach (SPListItem item in list.GetItems(list.DefaultView)) {
                dataCollector.TallyVote(item);
            }
            return dataCollector
                .OrderByDescending(i => i.Value.VoteCount)
                .OrderBy(i => i.Value.Selection)
                .Take(_parent.ResultCount)
                .Select(i => i.Value)
                .ToList();
        }
    }
}
