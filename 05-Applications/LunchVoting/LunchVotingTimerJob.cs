using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace LunchVoting {
    public class LunchVotingTimerJob : SPJobDefinition {
        public static void CreateTimerJob(SPSite site) {
            LunchVotingTimerJob timerJob = new LunchVotingTimerJob(site);
            timerJob.Schedule = new SPDailySchedule() {
                BeginHour = 0,
                EndHour = 4
            };
            timerJob.Update();
        }

        public static void DeleteTimerJob(SPSite site) {
            site.WebApplication.JobDefinitions.OfType<LunchVotingTimerJob>()
                .Where(i => string.Equals(i.SiteUrl, site.Url, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(i => i.Delete());
        }

        public LunchVotingTimerJob() : base() { }

        // site - Сайт, содержащий список Lunch Voting, который мы хотим очистить. 
        public LunchVotingTimerJob(SPSite site)
            : base(string.Format("Lunch Voting Timer Job ({0})", site.Url), site.WebApplication, null, SPJobLockType.Job) {
            Title = Name; // 1-й параметр base() - имя Timer Job. 
            SiteUrl = site.Url;
        }

        // Такой способ задания свойства обеспечивает персистентность значений в БД. 
        public string SiteUrl {
            get { return (string)this.Properties["SiteUrl"]; }
            set { this.Properties["SiteUrl"] = value; }
        }

        public override void Execute(Guid targetInstanceId) {
            using (SPSite site = new SPSite(SiteUrl)) {
                SPList list = site.RootWeb.Lists["Lunch Voting"];
                int itemIndex = 0;
                int itemCount = list.Items.Count;

                list.Items.Cast<SPListItem>().ToList().ForEach(item => {
                    item.Delete();
                    System.Threading.Thread.Sleep(1000);
                    itemIndex++;
                    int percentComplete = (itemIndex * 100) / itemCount;
                    if (percentComplete > 100) percentComplete = 100;
                    UpdateProgress(percentComplete);
                });
            }
        }
    }
}