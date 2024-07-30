using HtmlAgilityPack;
using Jobby_Me.Data;
using Jobby_Me.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobby_Me.Scrapper
{
    public class JobScrapper
    {
        HtmlWeb web;
        const string SITE_URL = "https://rabota.by/search/vacancy?text=.net&area=1002&page=";
        List<Job> jobList = new List<Job>();
        string[] keyWords = {"junior","trainee","c#",".net"};
        public JobScrapper()
        {
            web = new HtmlWeb();    
        }

        public async Task GetJobsAsync()
        {
            int i = 0;
            while (true)
            {
                var document = web.Load(SITE_URL+i);
                var jobsElements = document.DocumentNode.QuerySelectorAll("div.vacancy-search-item__card");
                if(jobsElements.Count == 0)
                {
                    break;
                }
                foreach( var jobElem in jobsElements )
                {
                    var title = jobElem.QuerySelector("span.serp-item__title-link").InnerText;
                    var url = jobElem.QuerySelector("a.bloko-link").Attributes["href"].Value;
                    var city = jobElem.SelectSingleNode(".//span[contains(@data-qa, 'vacancy-serp__vacancy-address')]").InnerText;
                    var company = jobElem.SelectSingleNode(".//span[contains(@class, 'company-info-text--vgvZouLtf8jwBmaD1xgp')]").InnerText;
                    Job job = new Job() { Title = title, Url = url, City = city, Company = company };
                    jobList.Add(job);
                }
                i++;
            }

        }

        public async Task<List<Job>> GetNewJobsAsync()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Job> resultList = new List<Job>();   
            var previousJobs = await dbContext.Jobs.ToListAsync();
            await GetJobsAsync();
            var newJobs = jobList.Except(previousJobs).ToList();
            foreach(var job in newJobs)
            {
                var contains = keyWords.Any(job.Title.ToLower().Contains);
                if (contains)
                {
                    resultList.Add(job);
                }
            }
            return resultList;
        }

    }
}
