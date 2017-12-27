using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {
            // ListsExample1();
            // ListItemExample();
            // FieldsExample();
            // ManagedMetadataExample();
            // SiteColumnExample();
            // ContentTypesExample();
            DocumentLibrariesFilesFoldersExample();
        }

        static void DocumentLibrariesFilesFoldersExample() {
            using (var site = new SPSite("http://maksim")) {
                // CreateDocumentLibrary(site.RootWeb);
                // UploadFile(site.RootWeb.Lists["Contracts"], 
                //    @"C:\SP Live Lesson Demo\03-Content\Documents\Fowler. UML Basics.pdf");
                //DownloadFile(site.RootWeb, @"http://maksim/Contracts/Fowler.%20UML%20Basics.pdf",
                //    @"C:\SP Live Lesson Demo\03-Content\Documents\Downloads\");
                //ShowVersions(site.RootWeb, @"http://maksim/Contracts/Fowler.%20UML%20Basics.pdf");
                //CheckoutDemo(site.RootWeb, @"http://maksim/Contracts/Fowler.%20UML%20Basics.pdf");
                EnsureFolders(site.RootWeb.Lists["Contracts"], "Abdulov\\Ozon 666\\Biboran");
            }
        }

        static void EnsureFolders(SPList list, string folderPath) {
            var folders = folderPath.Split('\\');
            var currentFolder = list.RootFolder;
            foreach(var folder in folders) {
                var nextFolder = (from SPFolder f in currentFolder.SubFolders
                                  where string.Equals(f.Name, folder, 
                                                      StringComparison.InvariantCultureIgnoreCase)
                                  select f).FirstOrDefault();
                if (nextFolder == null) {
                    nextFolder = currentFolder.SubFolders.Add(folder);
                    nextFolder.Item["ContentTypeId"] = list.ContentTypes["SharePoint Realty Folder"].Id;
                    nextFolder.Item["SPRDepartment"] = "Marketing";
                    nextFolder.Item.Update();
                }
                currentFolder = nextFolder;
            }
        }

        static void CheckoutDemo(SPWeb web, string itemUrl) {
            SPFile file = web.GetFile(itemUrl);
            file.CheckOut();

            Console.WriteLine("Press <Enter> to check the file in ...");
            Console.ReadLine();

            file.CheckIn("Checkin from demo code.");
        }

        static void ShowVersions(SPWeb web, string itemUrl) {
            SPFile file = web.GetFile(itemUrl);
            Console.WriteLine("Versions for {0}", file.Url);
            foreach(SPFileVersion version in file.Versions) {
                Console.WriteLine("{0} - {1} - {2}", version.VersionLabel, version.Size, version.CheckInComment);
                // version.File.OpenBinaryStream()
            }
        }

        static void DownloadFile(SPWeb web, string itemUrl, string downloadDirectory) {
            SPFile file = web.GetFile(itemUrl);
            using(var stream = file.OpenBinaryStream()) {
                string fileName = downloadDirectory + file.Name;
                using(var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)) {
                    var buffer = new byte[10000];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        static void UploadFile(SPList list, string pathToFile) {
            var fileName = Path.GetFileName(pathToFile);
            using (var stream = File.OpenRead(pathToFile)) {
                var file = list.RootFolder.Files.Add(fileName, stream, true);
                file.Item["Title"] = "FileName: " + fileName;
                file.Item.Update();
            }
        }

        static void CreateDocumentLibrary(SPWeb web) {
            web.Lists.Add("Contracts", "A Document library for contracts", SPListTemplateType.DocumentLibrary);
        }

        static void ContentTypesExample() {
            using (var site = new SPSite("http://maksim")) {
                // CreateListingContentType(site.RootWeb);
                // CreateRentalListingContentType(site.RootWeb);
                // CreateListingsList(site.RootWeb);
                AddListing(site.RootWeb.Lists["Listings"]);
            }
        }

        static void AddListing(SPList list) {
            var listing = list.AddItem();
            listing["ContentTypeId"] = list.ContentTypes["Listing"].Id;
            listing["FullName"] = "Dave Smith";
            listing["WorkPhone"] = "555-555-5555";
            listing["EMail"] = "ds@nn.com";
            listing["WorkAddress"] = "777 smwhre street";
            listing["WorkCity"] = "Plano";
            listing["WorkState"] = "TX";
            listing["WorkZip"] = "75003";
            listing.Update();
        }

        static void CreateListingsList(SPWeb web) {
            SPListTemplate template = web.ListTemplates["Custom List"];
            var listId = web.Lists.Add("Listings", "Contains a list of property listings.", template);
            var list = web.Lists[listId];
            list.ContentTypesEnabled = true;
            list.ContentTypes.Add(web.AvailableContentTypes["Listing"]);
            list.ContentTypes.Add(web.AvailableContentTypes["Rental Listing"]);
            list.ContentTypes.Delete(list.ContentTypes["Item"].Id);
            list.Update();
        }

        static void CreateListingContentType(SPWeb web) {
            var parentContentType = web.AvailableContentTypes["Item"];
            var contentType = new SPContentType(parentContentType, web.ContentTypes, "Listing");
            contentType.Group = "SharePoint Realty Content Types";
            contentType.FieldLinks["Title"].Hidden = true;
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("FullName")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("WorkPhone")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("EMail")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("WorkAddress")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("WorkCity")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("WorkState")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("WorkZip")));
            web.ContentTypes.Add(contentType);
            contentType.Update();
        }

        static void CreateRentalListingContentType(SPWeb web) {
            var parentContentType = web.AvailableContentTypes["Listing"];
            var contentType = new SPContentType(parentContentType, web.ContentTypes, "Rental Listing");
            contentType.Group = "SharePoint Realty Content Types";
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("SPRAllowsPets")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("SPRDepositAmount")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("SPRAllowsSmoking")));
            contentType.FieldLinks.Add(new SPFieldLink(web.AvailableFields.GetFieldByInternalName("SPRRentalAmount")));
            web.ContentTypes.Add(contentType);
            contentType.Update();
        }

        static void SiteColumnExample() {
            using (var site = new SPSite("http://maksim")) {
                var list = site.RootWeb.Lists["Corporate Directory"];
                // CreateDepartmentSiteColumn(site.RootWeb);
                AddSiteColumnToList(site.RootWeb, list, "SPRDepartment");
            }
        }

        static void AddSiteColumnToList(SPWeb web, SPList list, string siteColumnName) {
            list.Fields.Add(web.AvailableFields.GetFieldByInternalName(siteColumnName));
            list.Update();
        }

        static void CreateDepartmentSiteColumn(SPWeb web) {
            web.Fields.Add(web.Fields.CreateNewField(SPFieldType.Choice.ToString(), "SPRDepartment"));
            web.Update();

            var newField = (SPFieldChoice)web.Fields["SPRDepartment"];
            newField.Title = "Department";
            newField.Group = "SharePoint Realty Site Columns";
            newField.Required = true;
            newField.Description = "Denotes the department the employee works in";
            newField.Choices.Add("Real Estate");
            newField.Choices.Add("Lending");
            newField.Choices.Add("Marketing");
            newField.Choices.Add("HR");
            newField.Choices.Add("IT");
            newField.Choices.Add("Executive");
            newField.EditFormat = SPChoiceFormatType.Dropdown;
            newField.FillInChoice = false;
            newField.DefaultValue = "HR";
            newField.Update();

        }

        static void ManagedMetadataExample() {
            using (var site = new SPSite("http://maksim")) {
                var list = site.RootWeb.Lists["Corporate Directory"];
                var session = new TaxonomySession(site);
                var termStore = session.DefaultSiteCollectionTermStore;

                //var group = CreateGroup(termStore, "SharePoint Realty");
                //var termSet = CreateTermSet(group, "Ask Me About");
                //CreateTerm(termSet, "Homes");
                //CreateTerm(termSet, "Apartments");
                //CreateTerm(termSet, "Contracts");
                //var insuranse = CreateTerm(termSet, "Insurance");
                //CreateTerm(insuranse, "Renters");
                //CreateTerm(insuranse, "Home Owners");
                //CreateTerm(insuranse, "Liability");

                // ************ Get Info about terms **********************************************
                var group = termStore.Groups["SharePoint Realty"];
                var termSet = group.TermSets["Ask Me About"];
                var term = FindTerm(termSet, "Insurance/Home Owners");

                //if (term != null) {
                //    Console.WriteLine(term.Id);
                //}
                var listItem = list.GetItemById(1);
                // PrintAskMeAboutTerms(listItem);
                AddAskMeAboutTerm(listItem, termSet, "Contracts");
                AddAskMeAboutTerm(listItem, termSet, "Insurance/Liability");
            }
        }

        static void AddAskMeAboutTerm(SPListItem item, TermSet termSet, string termPath) {
            var term = FindTerm(termSet, termPath);
            if (term != null) {
                var currentValues = (TaxonomyFieldValueCollection)item["AskMeAbout"];
                var newValue = new TaxonomyFieldValue(item.Fields.GetFieldByInternalName("AskMeAbout")) {
                    TermGuid = term.Id.ToString(),
                    Label = term.Name
                };
                currentValues.Add(newValue);
                item["AskMeAbout"] = currentValues;
                item.Update();
                Console.WriteLine("Updated the item.");
            }

        }

        static void PrintAskMeAboutTerms(SPListItem item) {
            var currentValues = (TaxonomyFieldValueCollection)item["AskMeAbout"];
            foreach (var value in currentValues) {
                Console.WriteLine(value.Label);
            }
        }

        static TermSetItem FindTerm(TermSetItem termSetItem, string path) {
            var pathParts = path.Split('/');
            var currentIndex = 0;
            while (termSetItem != null && currentIndex < pathParts.Length) {
                termSetItem = termSetItem.Terms.FirstOrDefault(term => 
                    string.Equals(term.Name, pathParts[currentIndex], StringComparison.InvariantCultureIgnoreCase)
                );
                currentIndex++;
            }
            return termSetItem;
        }

        static Group CreateGroup(TermStore termStore, string groupName) {
            var group = termStore.CreateGroup(groupName);
            termStore.CommitAll();
            return group;
        }

        static TermSet CreateTermSet(Group group, string termSetName) {
            var termSet = group.CreateTermSet(termSetName);
            group.TermStore.CommitAll();
            return termSet;
        }

        static Term CreateTerm(TermSetItem termSetItem, string termName) {
            var term = termSetItem.CreateTerm(termName, 1033);
            termSetItem.TermStore.CommitAll();
            return term;
        }

        static void FieldsExample() {
            using (var site = new SPSite("http://maksim")) {
                var list = site.RootWeb.Lists["Corporate Directory"];
                
                // Где есть поля
                //list.Fields;
                //list.Items[0].Fields;
                //site.RootWeb.Fields;
                //site.RootWeb.ContentTypes[0].Fields;

                // ShowFields(list.Fields);
                // CreateTextField(list);
                // CreateChoiceField(list);
                // CreateLookupField(list);
            }
        }

        static void CreateLookupField(SPList list) {
            list.Fields.Add(list.Fields.CreateNewField(SPFieldType.Lookup.ToString(), "Licenses"));
            list.Update();

            list = list.ParentWeb.Lists[list.ID];
            var newField = (SPFieldLookup)list.Fields["Licenses"];
            newField.Description = "Shows a list of the licenses the employee has obtained";
            newField.Required = false;
            newField.EnforceUniqueValues = false;
            newField.LookupList = "e7cb9558-397b-4534-a6dd-ff797ad73a26";
            newField.LookupField = "Title";
            newField.AllowMultipleValues = true;
            newField.Update();
        }

        static void CreateChoiceField(SPList list) {
            list.Fields.Add(list.Fields.CreateNewField(SPFieldType.Choice.ToString(), "Department"));
            list.Update();

            var newField = (SPFieldChoice)list.Fields["Department"];
            newField.Required = true;
            newField.Description = "Denotes the department the employee works in";
            newField.Choices.Add("Real Estate");
            newField.Choices.Add("Lending");
            newField.Choices.Add("Marketing");
            newField.Choices.Add("HR");
            newField.Choices.Add("IT");
            newField.Choices.Add("Executive");
            newField.EditFormat = SPChoiceFormatType.Dropdown;
            newField.FillInChoice = false;
            newField.DefaultValue = "HR";
            newField.Update();
        }

        static void CreateTextField(SPList list) {
            list.Fields.Add(list.Fields.CreateNewField(SPFieldType.Text.ToString(), "LocationsServiced"));
            list.Update();

            SPFieldText newField = (SPFieldText)list.Fields["LocationsServiced"];
            newField.Title = "Locations Serviced";
            newField.Required = false;
            newField.ShowInDisplayForm = true;
            newField.ShowInEditForm = true;
            newField.ShowInNewForm = true;
            newField.ShowInListSettings = true;
            newField.Description = "This allows you to enter which cities you service.";
            newField.MaxLength = 255;
            newField.Update();
        }

        private static void ShowFields(SPFieldCollection fields) {
            foreach (SPField field in fields) {
                if (!field.Hidden) {
                    Console.WriteLine("=============================================================");
                    // Дружественное имя столбца. МБ изменено
                    Console.WriteLine("Title        : {0}", field.Title); 
                    // Задаются SP. Обычно одинаковые; InternalName - уникальное.
                    // По возможности для ссылок в коде используйте InternalName
                    Console.WriteLine("Static Name  : {0}", field.StaticName);
                    Console.WriteLine("Internal Name: {0}", field.InternalName);
                    // Имя типа данных поля
                    Console.WriteLine("Field Type   : {0}", field.TypeDisplayName);
                    Console.WriteLine("Description  : {0}", field.Description);
                    Console.WriteLine("Required     : {0}", field.Required);
                    Console.WriteLine("Unique Values: {0}", field.EnforceUniqueValues);
                    Console.WriteLine("New Form     : {0}", field.ShowInNewForm);
                    Console.WriteLine("Edit Form    : {0}", field.ShowInEditForm);
                    Console.WriteLine("Display Form : {0}", field.ShowInDisplayForm);
                    Console.WriteLine("List Settings: {0}", field.ShowInListSettings);
                }
            }
        }

        static void ListItemExample() {
            using (var site = new SPSite("http://maksim")) {
                // CreateCorporateDirectory(site.RootWeb);
                var list = site.RootWeb.Lists["Corporate Directory"];
                // AddDirectoryEntry(list, "Doe", "Jane", "SharePoint Realty", "444-444-4444", 
                //    "Jane.Doe@SharePointRealty.com");
                GetListItemDemo(list);
            }
        }

        static void GetListItemDemo(SPList list) {
            var item = list.GetItemById(1);
            Console.WriteLine("    Unique: {0}", item.UniqueId);
            // a0cee7d4-53f3-4deb-be8e-6698bb522e36
        }

        static SPListItem AddDirectoryEntry(SPList list, string lastName, string firstname, 
                string company, string workphone, string email) {
            var entry = list.AddItem();
            // Здесь используются внутренние имена для списков SP
            entry["Title"] = lastName;
            entry["FirstName"] = firstname;
            entry["Company"] = company;
            entry["WorkPhone"] = workphone;
            entry["Email"] = email;
            entry.Update();
            Console.WriteLine("Added {0} {1} to the Directory", firstname, lastName);
            return entry;
        }

        static void ListsExample1() {
            using (var site = new SPSite("http://maksim")) {
                // CreateCorporateDirectory(site.RootWeb);
                var list = site.RootWeb.Lists["Corporate Directory"];
                CreateView(list);
            }
        }


        static void CreateView(SPList list) {
            var viewFields = new StringCollection() { "Attachments", "FirstName", "LinkTitle", "Company", "WorkPhone", "HomePhone", "Email"};
            string query = "<OrderBy><FieldRef Name='FirstName' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy>";
            var newView = list.Views.Add("First Name First", viewFields, query, 5000, true, false);
            newView.Update();
        }

        static SPList CreateCorporateDirectory(SPWeb web) {
            Console.Write("Creating Directory ... ");

            SPListTemplate template = web.ListTemplates["Contacts"];

            var listId = web.Lists.Add("Corporate Directory", 
                "Contains phone numbers for all employees", template);
            Console.WriteLine("Done");
            return web.Lists[listId];
        }
    }
}