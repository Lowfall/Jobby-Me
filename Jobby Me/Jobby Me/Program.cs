using Jobby_Me.Data;
using Jobby_Me.Scrapper;
using Jobby_Me.Telegram;
using Microsoft.Extensions.Configuration;

namespace Jobby_Me
{
    public class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            JobScrapper jobScrapper = new JobScrapper();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "appconfig.json"), true,true)
                .Build();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            dbContext.Database.EnsureCreated();
            Bot telegramBot = new Bot(configuration,dbContext);
            Console.ReadKey();
        }
    }
}
