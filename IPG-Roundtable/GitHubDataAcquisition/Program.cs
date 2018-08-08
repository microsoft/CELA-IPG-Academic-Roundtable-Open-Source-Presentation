/*
   Copyright (c) Microsoft Corporation

   All rights reserved. 

   MIT License

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CELA.GitHubDataAcquisition
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IPG Academic Roundtable-GitHub Data Utility Started");
            //Scrape GitHub data
            GitHubOrgDataProcessor gitHubProcessor = new GitHubOrgDataProcessor();
            gitHubProcessor.generateGitHubEngagementData(@"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\PatentDataAcquisition\Data\github_engagement.csv", @"C:\Users\jabarnwe\Source\Repos\OSSCELA-IPG-Roundtable\IPG-Roundtable\GitHubDataAcquisition\auth.json");

            Console.ReadLine();
        }
    }

    class GitHubOrgDataProcessor
    {
        public GitHubOrgDataProcessor()
        {
        }

        public List<Organization> generateGitHubEngagementData(string pathToOutputFile, string authFilePath)
        {
            Console.WriteLine("Generating GitHub Engagement Data");
            var orgs = generateOrganizations();
            //iterate over the top level organizations
            foreach (var organization in orgs)
            {
                Console.WriteLine("Evaluating Top Level Organization:" + organization.name);
                //iterate over the allied organizations
                foreach (var alliedOrg in organization.alliedOrganizationGitHubHandles)
                {
                    Console.WriteLine("Augmenting data for Allied Organization: " + alliedOrg);
                    //Get the repos for the specific allied repository
                    var repos = getOrgRespositoriesForOrg(alliedOrg, new List<Repository>(), authFilePath);
                    //Add the organization to the list for the top level organization                    
                    organization.organizations.Add(repos.First().owner);

                    Console.WriteLine("Evaluating Repos for Allied Organization: " + alliedOrg);
                    foreach (var repository in repos)
                    {
                        Console.WriteLine("Adding " + repository.name + " to " + organization.name);
                        organization.repositories.Add(repository);
                        //populate the dictionary that links orgs and repos
                        organization.orgsByRepo.Add(repository, repository.owner);
                    }
                    //for testing only evalute the first allied organization
                    //break;
                }
                //Add a delay to deal with GitHub rate throttling
                Console.WriteLine("Pausing to deal with throttle limits");
                Thread.Sleep(10000);
                Console.WriteLine("Restarting after delay");
                //for testing only evaluate the first organization
                //break;
            }
            writeGitHubEngagementDataToCSV(orgs, pathToOutputFile);
            return orgs;
        }

        public List<Repository> getOrgRespositoriesForOrg(string gitHubOrgName, List<Repository> orgRepos, string authFilePath)
        {
            var pageSize = 100;
            var page = 1;
            var client = new RestClient("https://api.github.com");
            //auth.json file takes the form of 
            //{ "user":"microsoftopensourcelegal","password":"m$f7Paddw04D"}            
            var auth = JsonConvert.DeserializeObject<Auth>(File.ReadAllText(authFilePath));
            client.Authenticator = new HttpBasicAuthenticator(auth.user, auth.password);

            IRestResponse<List<Repository>> response = null;
            do
            {
                var request = new RestRequest("orgs/" + gitHubOrgName + "/repos");
                request.AddParameter("page", page);
                request.AddParameter("per_page", pageSize);
                response = client.Execute<List<Repository>>(request);
                response.Data.ForEach(r => orgRepos.Add(r));
                page++;
            } while (response.Data.Count == pageSize);

            return orgRepos;
        }

        public bool writeGitHubEngagementDataToCSV(List<Organization> orgs, string fileOutputPath)
        {
            Console.WriteLine("Writing engagement data to " + fileOutputPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("OrganizationName,PatentFilerName,LinuxFoundationMembership,RepositoryFullName,RepositoryURL,RepositoryForks,RepositoryWatchers,RepositorySizeInKiloBytes,RepositoryName,GitHubOrgName,GitHubOrgURL,RespositoryDescription");
            //iterate over the organizations (which is the top level umbrella organization that aggregates the applicable GH orgs)
            foreach (var org in orgs)
            {
                foreach (var repo in org.repositories)
                {
                    var gitHubOrgForRepo = org.orgsByRepo[repo];
                    sb.AppendLine(StringToCSVCell(org.name) + "," + StringToCSVCell(org.PatentFilerName) + "," + StringToCSVCell(org.LinxuFoundationMembership) + "," + StringToCSVCell(repo.full_name) + "," + StringToCSVCell(repo.url) + "," + repo.forks_count + "," + repo.watchers_count + "," + repo.size + "," + StringToCSVCell(repo.name) + "," + StringToCSVCell(gitHubOrgForRepo.login) + "," + gitHubOrgForRepo.html_url + "," + StringToCSVCell(repo.description));
                }
            }
            File.WriteAllText(fileOutputPath, sb.ToString());
            return true;
        }

        //From: http://stackoverflow.com/questions/6377454/escaping-tricky-string-to-csv-format
        //Note that no relicensing has been requested from the author so this is licensed under CC-SA unless other permission granted
        public static string StringToCSVCell(string str)
        {
            if (str == null || str.Length < 1)
            {
                return "";
            }

            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }

        public List<Organization> generateOrganizations()
        {
            List<Organization> orgs = new List<Organization>();

            //https://amzn.github.io/
            Organization amazon = new Organization();
            orgs.Add(amazon);
            amazon.name = "Amazon";
            amazon.PatentFilerName = "AMAZON TECHNOLOGIES, INC.";
            amazon.LinxuFoundationMembership = "None";
            amazon.alliedOrganizationGitHubHandles.Add("aws");
            amazon.alliedOrganizationGitHubHandles.Add("amazonwebservices");
            amazon.alliedOrganizationGitHubHandles.Add("amzn");
            amazon.alliedOrganizationGitHubHandles.Add("amznlabs");
            amazon.alliedOrganizationGitHubHandles.Add("alexa");
            amazon.alliedOrganizationGitHubHandles.Add("awsdocs");
            amazon.alliedOrganizationGitHubHandles.Add("awslabs");
            amazon.alliedOrganizationGitHubHandles.Add("aws-quickstart");
            amazon.alliedOrganizationGitHubHandles.Add("blindsightcorp");
            amazon.alliedOrganizationGitHubHandles.Add("Carbonado");
            amazon.alliedOrganizationGitHubHandles.Add("ajaxorg");
            amazon.alliedOrganizationGitHubHandles.Add("c9");
            amazon.alliedOrganizationGitHubHandles.Add("goodreads");
            amazon.alliedOrganizationGitHubHandles.Add("IvonaSoftware");
            amazon.alliedOrganizationGitHubHandles.Add("twitchtv");
            amazon.alliedOrganizationGitHubHandles.Add("TwitchScience");
            amazon.alliedOrganizationGitHubHandles.Add("justintv");
            amazon.alliedOrganizationGitHubHandles.Add("Zappos");

            Organization apple = new Organization();
            orgs.Add(apple);
            apple.name = "Apple";
            apple.PatentFilerName = "APPLE, INC.";
            apple.LinxuFoundationMembership = "None";
            apple.alliedOrganizationGitHubHandles.Add("apple");

            Organization cisco = new Organization();
            orgs.Add(cisco);
            cisco.name = "Cisco";
            cisco.PatentFilerName = "CISCO TECHNOLOGY, INC.";
            cisco.LinxuFoundationMembership = "Platinum";
            cisco.alliedOrganizationGitHubHandles.Add("cisco");
            cisco.alliedOrganizationGitHubHandles.Add("CiscoCloud");
            cisco.alliedOrganizationGitHubHandles.Add("CiscoSystems");
            cisco.alliedOrganizationGitHubHandles.Add("cisco-system-traffic-generator");
            cisco.alliedOrganizationGitHubHandles.Add("contiv");
            cisco.alliedOrganizationGitHubHandles.Add("datacenter");

            //https://code.facebook.com/
            Organization facebook = new Organization();
            orgs.Add(facebook);
            facebook.name = "Facebook";
            facebook.PatentFilerName = "FACEBOOK, INC.";
            facebook.LinxuFoundationMembership = "Gold";
            facebook.alliedOrganizationGitHubHandles.Add("facebook");
            facebook.alliedOrganizationGitHubHandles.Add("OculusVR");
            facebook.alliedOrganizationGitHubHandles.Add("Instagram");

            Organization fujitsu = new Organization();
            orgs.Add(fujitsu);
            fujitsu.name = "Fujitsu";
            fujitsu.PatentFilerName = "FUJITSU LIMITED";
            fujitsu.LinxuFoundationMembership = "Platinum";
            fujitsu.alliedOrganizationGitHubHandles.Add("fujitsu-pio");
            fujitsu.alliedOrganizationGitHubHandles.Add("FujitsuEnablingSoftwareTechnologyGmbH");

            Organization google = new Organization();
            orgs.Add(google);
            google.name = "Google";
            google.PatentFilerName = "GOOGLE, INC.";
            google.LinxuFoundationMembership = "Gold";
            google.alliedOrganizationGitHubHandles.Add("google");
            google.alliedOrganizationGitHubHandles.Add("googlemaps");
            google.alliedOrganizationGitHubHandles.Add("googlevr");
            google.alliedOrganizationGitHubHandles.Add("youtube");
            google.alliedOrganizationGitHubHandles.Add("angular");

            Organization huawei = new Organization();
            orgs.Add(huawei);
            huawei.name = "Huawei";
            huawei.PatentFilerName = "HUAWEI TECHNOLOGIES CO., LTD.";
            huawei.LinxuFoundationMembership = "Platinum";
            huawei.alliedOrganizationGitHubHandles.Add("Huawei");

            Organization ibm = new Organization();
            orgs.Add(ibm);
            ibm.name = "IBM";
            ibm.PatentFilerName = "INTERNATIONAL BUSINESS MACHINES CORPORATION";
            ibm.LinxuFoundationMembership = "Platinum";
            ibm.alliedOrganizationGitHubHandles.Add("ibm");
            ibm.alliedOrganizationGitHubHandles.Add("IBM-Blockchain");
            ibm.alliedOrganizationGitHubHandles.Add("IBM-Bluemix");
            ibm.alliedOrganizationGitHubHandles.Add("IBM-MIL");
            ibm.alliedOrganizationGitHubHandles.Add("IBM-Design");
            ibm.alliedOrganizationGitHubHandles.Add("IBM-Watson");
            ibm.alliedOrganizationGitHubHandles.Add("ibm-messaging");

            Organization intel = new Organization();
            orgs.Add(intel);
            intel.name = "Intel";
            intel.PatentFilerName = "INTEL CORPORATION";
            intel.LinxuFoundationMembership = "Platium";
            intel.alliedOrganizationGitHubHandles.Add("intel");
            intel.alliedOrganizationGitHubHandles.Add("intel-iot-devkit");
            intel.alliedOrganizationGitHubHandles.Add("IntelRealSense");
            intel.alliedOrganizationGitHubHandles.Add("intel-hadoop");
            //Turns out this is not an org
            //intel.alliedOrganizationGitHubHandles.Add("ispc");

            Organization linkedin = new Organization();
            orgs.Add(linkedin);
            linkedin.name = "LinkedIn";
            linkedin.PatentFilerName = "Linkedin Corporation";
            linkedin.LinxuFoundationMembership = "None";
            linkedin.alliedOrganizationGitHubHandles.Add("linkedin");

            Organization microsoft = new Organization();
            orgs.Add(microsoft);
            microsoft.name = "Microsoft";
            microsoft.PatentFilerName = "MICROSOFT TECHNOLOGY LICENSING, LLC.";
            microsoft.LinxuFoundationMembership = "None";
            microsoft.alliedOrganizationGitHubHandles.Add("microsoft");
            //Note that it may be a distortion to aggregate this into this organization
            microsoft.alliedOrganizationGitHubHandles.Add("dotnet");
            microsoft.alliedOrganizationGitHubHandles.Add("MSOpenTech");
            microsoft.alliedOrganizationGitHubHandles.Add("Azure");
            microsoft.alliedOrganizationGitHubHandles.Add("aspnet");
            microsoft.alliedOrganizationGitHubHandles.Add("MicrosoftLearning");
            microsoft.alliedOrganizationGitHubHandles.Add("MicrosoftGenomics");
            microsoft.alliedOrganizationGitHubHandles.Add("MicrosoftEdge");
            microsoft.alliedOrganizationGitHubHandles.Add("Azure-Readiness");
            microsoft.alliedOrganizationGitHubHandles.Add("microsoftgraph");
            microsoft.alliedOrganizationGitHubHandles.Add("MicrosoftDX");

            Organization oracle = new Organization();
            orgs.Add(oracle);
            oracle.name = "Oracle";
            oracle.PatentFilerName = "ORACLE INTERNATIONAL CORPORATION";
            oracle.LinxuFoundationMembership = "Platinum";
            oracle.alliedOrganizationGitHubHandles.Add("oracle");
            oracle.alliedOrganizationGitHubHandles.Add("mysql");

            Organization qualcomm = new Organization();
            orgs.Add(qualcomm);
            qualcomm.name = "Qualcomm";
            qualcomm.PatentFilerName = "ORACLE INTERNATIONAL CORPORATION";
            qualcomm.LinxuFoundationMembership = "Platinum";
            qualcomm.alliedOrganizationGitHubHandles.Add("Qualcomm-msm");

            //https://github.com/Samsung
            Organization samsung = new Organization();
            orgs.Add(samsung);
            samsung.name = "Samsung";
            samsung.PatentFilerName = "SAMSUNG ELECTRONICS CO., LTD.";
            samsung.LinxuFoundationMembership = "Platinum";
            samsung.alliedOrganizationGitHubHandles.Add("samsung");

            return orgs;
        }
    }

    public class Organization
    {
        public string name { get; set; }
        public List<Owner> organizations { get; set; }
        public List<Repository> repositories { get; set; }
        public List<string> alliedOrganizationGitHubHandles { get; set; }
        public string LinxuFoundationMembership { get; set; }
        public string PatentFilerName { get; set; }
        public Dictionary<Repository, Owner> orgsByRepo { get; set; }

        public Organization()
        {
            organizations = new List<Owner>();
            repositories = new List<Repository>();
            alliedOrganizationGitHubHandles = new List<string>();
            orgsByRepo = new Dictionary<Repository, Owner>();
        }
    }



    public class Owner
    {
        public string login { get; set; }
        public int id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    public class Permissions
    {
        public bool admin { get; set; }
        public bool push { get; set; }
        public bool pull { get; set; }
    }

    public class Repository
    {
        public int id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public Owner owner { get; set; }
        public bool @private { get; set; }
        public string html_url { get; set; }
        public string description { get; set; }
        public bool fork { get; set; }
        public string url { get; set; }
        public string forks_url { get; set; }
        public string keys_url { get; set; }
        public string collaborators_url { get; set; }
        public string teams_url { get; set; }
        public string hooks_url { get; set; }
        public string issue_events_url { get; set; }
        public string events_url { get; set; }
        public string assignees_url { get; set; }
        public string branches_url { get; set; }
        public string tags_url { get; set; }
        public string blobs_url { get; set; }
        public string git_tags_url { get; set; }
        public string git_refs_url { get; set; }
        public string trees_url { get; set; }
        public string statuses_url { get; set; }
        public string languages_url { get; set; }
        public string stargazers_url { get; set; }
        public string contributors_url { get; set; }
        public string subscribers_url { get; set; }
        public string subscription_url { get; set; }
        public string commits_url { get; set; }
        public string git_commits_url { get; set; }
        public string comments_url { get; set; }
        public string issue_comment_url { get; set; }
        public string contents_url { get; set; }
        public string compare_url { get; set; }
        public string merges_url { get; set; }
        public string archive_url { get; set; }
        public string downloads_url { get; set; }
        public string issues_url { get; set; }
        public string pulls_url { get; set; }
        public string milestones_url { get; set; }
        public string notifications_url { get; set; }
        public string labels_url { get; set; }
        public string releases_url { get; set; }
        public string deployments_url { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string pushed_at { get; set; }
        public string git_url { get; set; }
        public string ssh_url { get; set; }
        public string clone_url { get; set; }
        public string svn_url { get; set; }
        public string homepage { get; set; }
        public int size { get; set; }
        public int stargazers_count { get; set; }
        public int watchers_count { get; set; }
        public string language { get; set; }
        public bool has_issues { get; set; }
        public bool has_downloads { get; set; }
        public bool has_wiki { get; set; }
        public bool has_pages { get; set; }
        public int forks_count { get; set; }
        public object mirror_url { get; set; }
        public int open_issues_count { get; set; }
        public int forks { get; set; }
        public int open_issues { get; set; }
        public int watchers { get; set; }
        public string default_branch { get; set; }
        public Permissions permissions { get; set; }
    }

    class Auth
    {
        public string user { get; set; }
        public string password { get; set; }
    }


}
