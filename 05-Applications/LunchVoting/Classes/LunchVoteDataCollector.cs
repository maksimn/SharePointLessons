using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVoting.Classes {
    // key is Selection name
    public class LunchVoteDataCollector: Dictionary<string, LunchVoteData> {
        public void TallyVote(SPListItem item) {
            if ((DateTime)item["Created"] >= DateTime.Today) {
                string selection = new SPFieldLookupValue(item["Selection"].ToString()).LookupValue;
                bool isProxyVote = item["OnBehalfOf"] != null;
                bool youVoted = new SPFieldUserValue(SPContext.Current.Web, item["Author"].ToString()).User.Name ==
                    SPContext.Current.Web.CurrentUser.Name; // Так сложно, т.к. нас интересует DisplayName.
                bool youVotedByProxy = !isProxyVote ? false :
                    new SPFieldUserValue(SPContext.Current.Web, item["OnBehalfOf"].ToString()).User.Name ==
                    SPContext.Current.Web.CurrentUser.Name;

                LunchVoteData data = null;
                if (!TryGetValue(selection, out data)) {
                    data = new LunchVoteData();
                    this.Add(selection, data);
                    data.Selection = selection;
                }

                data.VoteCount++;
                if (isProxyVote) data.OnBehalfOfCount++;
                data.YouVoted = data.YouVoted || youVoted;
                data.YouVotedByProxy = data.YouVotedByProxy || youVotedByProxy;
            }
        }
    }
}
