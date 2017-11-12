using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HostedServiceSample.Data;
using HostingSample.Abstractions;

namespace HostingSample.Services
{
    public class WordCountingService : BackgroundService
    {
        private readonly AppDbContext _db;

        public WordCountingService(AppDbContext db)
        {
            _db = db;
        }

        public CancellationToken StoppingToken { get; private set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StoppingToken = stoppingToken;

            Console.WriteLine("WordCountingService is starting.");

            stoppingToken.Register(() => Console.WriteLine("WordCountingService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("WordCountingService is performing background work.");

                foreach (Word word in _db.Words)
                {
                    _db.Words.Remove(word);
                }

                await _db.SaveChangesAsync();

                var words = new Dictionary<string, int>();

                foreach (Message message in _db.Messages)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var messageWords = Regex.Replace(message.Text.ToLowerInvariant(), @"[^\w\s]", "").Split(' ');

                    foreach (var word in messageWords)
                    {
                        if (words.TryGetValue(word, out var count))
                        {
                            words[word] = count + 1;
                        }
                        else
                        {
                            words.Add(word, 1);
                        }
                    }
                }

                foreach (var word in words)
                {
                    _db.Words.Add(new Word() { Text = word.Key, Count = word.Value });
                }

                await _db.SaveChangesAsync();

                DateTime releaseWait = DateTime.Now.AddSeconds(30);

                while (DateTime.Now < releaseWait && !stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            Console.WriteLine("WordCountingService background task is finished.");
        }
    }
}
