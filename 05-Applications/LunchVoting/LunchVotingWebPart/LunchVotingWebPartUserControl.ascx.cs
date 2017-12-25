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
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
                LunchVoteData dataItem = e.Item.DataItem as LunchVoteData;

                Literal selection = (Literal)e.Item.FindControl("Selection");
                if (selection != null) selection.Text = dataItem.Selection;

                Literal voteCount = (Literal)e.Item.FindControl("VoteCount");
                if (voteCount != null) voteCount.Text = dataItem.VoteCount.ToString();

                Literal votesOnBehalf = (Literal)e.Item.FindControl("VotesOnBehalf");
                if (votesOnBehalf != null) votesOnBehalf.Text = dataItem.OnBehalfOfCount.ToString();

                Literal yourVote = (Literal)e.Item.FindControl("YourVote");
                if (yourVote != null) yourVote.Visible = dataItem.YouVoted;

                Literal yourVoteProxy = (Literal)e.Item.FindControl("YourVoteProxy");
                if (yourVoteProxy != null) yourVoteProxy.Visible = dataItem.YouVotedByProxy;
            }
        }

        private IList<LunchVoteData> GetLunchVoteData() {
            SPList list = SPContext.Current.Web.Lists["Lunch Voting"];
            LunchVoteDataCollector dataCollector = new LunchVoteDataCollector();
            foreach (SPListItem item in list.GetItems(list.DefaultView)) {
                dataCollector.TallyVote(item);
            }
            return dataCollector
                .OrderByDescending(i => i.Value.VoteCount)
                .ThenBy(i => i.Value.Selection)
                .Take(_parent.ResultCount)
                .Select(i => i.Value)
                .ToList();
        }
    }
}
