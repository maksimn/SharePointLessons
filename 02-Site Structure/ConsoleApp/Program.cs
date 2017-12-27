using System;
using Microsoft.SharePoint;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {

        }

        static void CreateSiteCollection(SPSite site) {
            site.WebApplication.Sites.Add("sites/Executive", "Executive", 
                "Site Collection for Executives", 1033, "STS#0", "ROGAIKOPYTA\\ostap", "ostap", 
                String.Empty);
        }

        static void CreateTwoSubsites() {
            using (var site = new SPSite("http://maksim/sites/realestate/residential")) {
                using (var web = site.OpenWeb()) {
                    web.Webs.Add("Houses", "Houses", "Subsite for Houses", 1033, "STS#0", false, false);
                    web.Webs.Add("Apartments", "Apartments", "Subsite for Apartments", 1033, "STS#0", false, false);
                }
            }
        }
    }
}
