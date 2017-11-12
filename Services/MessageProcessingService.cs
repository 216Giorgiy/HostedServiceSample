using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HostedServiceSample.Data;
using HostingSample.Abstractions;

namespace HostingSample.Services
{
    public class MessageProcessingService : BackgroundService
    {
        private readonly AppDbContext _db;

        public MessageProcessingService(AppDbContext db)
        {
            _db = db;
        }

        public CancellationToken StoppingToken { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StoppingToken = stoppingToken;

            Console.WriteLine("MessageProcessingService is starting.");

            stoppingToken.Register(() => Console.WriteLine("MessageProcessingService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("MessageProcessingService is performing background work.");

                foreach (Message message in _db.Messages)
                {
                    if (message.ProcessedText == null)
                    {
                        message.ProcessedText = TranslateSentence(message.Text);
                    }
                }

                await _db.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Console.WriteLine("MessageProcessingService background task is finished.");
        }

        private string TranslateSentence(string message)
        {
            const string vowels = "AEIOUaeiou";
            StringBuilder sb = new StringBuilder();
            var punctuation = message.Substring(message.Length - 1, 1);

            if (!"!.?".Contains(punctuation))
            {
                punctuation = string.Empty;
            }

            var messageWords = Regex.Replace(message.ToLowerInvariant(), @"[^\w\s]", "").Split(' ');

            foreach (string word in messageWords)
            {
                string firstLetter = word.Substring(0, 1);
                string restOfWord = word.Substring(1, word.Length - 1);
                int currentLetter = vowels.IndexOf(firstLetter);

                if (currentLetter == -1)
                {
                    sb.Append($"{restOfWord}{firstLetter}ay ");
                }
                else
                {
                    sb.Append($"{word}way ");
                }
            }

            var returnString = sb.ToString().TrimEnd();

            return $"{returnString.First().ToString().ToUpper()}{returnString.Substring(1)}{punctuation}";
        }
    }
}
