using Jobby_Me.Data;
using Jobby_Me.Scrapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Jobby_Me.Telegram
{
    public class Bot
    {
        TelegramBotClient client;

        public Bot(IConfiguration configuration,ApplicationDbContext dbContext)
        {
            client = new TelegramBotClient(configuration.GetSection("BotSettings")["KeyAPI"]);
            client.StartReceiving(UpdateHandler, ErrorHandler);
            Console.WriteLine("Bot successfully started");
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if(update.Message.Chat.Type == ChatType.Group)
            {
                return;
            }
            ApplicationDbContext dbContext = new ApplicationDbContext();
            if (IsFirstMessage(update.Message.Chat.Id))
            {
                dbContext.Users.Add(update.Message.From);
                dbContext.SaveChanges();
            }
            if (update.Message.Text == "/jobs")
            {
                JobScrapper jobScrapper = new JobScrapper();
                var jobList = await jobScrapper.GetNewJobsAsync();
                StringBuilder stringBuilder = new StringBuilder();  
                foreach (var job in jobList)
                {
                    stringBuilder.AppendLine(job.Title);
                    stringBuilder.AppendLine(job.City);
                    stringBuilder.AppendLine(job.Company);
                    stringBuilder.AppendLine(job.Url);
                    stringBuilder.AppendLine();
                }
                await client.SendTextMessageAsync(update.Message.Chat.Id, stringBuilder.ToString());
            }

        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }

        private bool IsFirstMessage(long id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var user =  dbContext.Users.FirstOrDefault(u => u.Id == id);
            return user == null ? true : false;
        }

    }
}
