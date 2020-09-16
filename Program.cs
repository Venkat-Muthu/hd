using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrlRanking.ServerLogReader;
using UrlRanking.ServerLogSimulator;

namespace UrlRanking
{
    class Program
    {
        static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        static readonly ConcurrentDictionary<string, int> ConcurrentDictionary = new ConcurrentDictionary<string, int>();
        private static Timer timer;
        static void Main(string[] args)
        {
            string fileName = @".\file01.txt";
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(200);
            IGenerateW3CLogFile generateW3CLogFile =
                new GenerateW3CLogFile(fileName, timeSpan, CancellationTokenSource.Token);
            generateW3CLogFile.StartAsync().ConfigureAwait(false);

            var readW3CLogFile = new ReadW3CLogFile();
            var w3CFieldsObservable = readW3CLogFile.GetW3CFieldsObservable(fileName);

            var disposable = w3CFieldsObservable.Subscribe(OnNext, OnError, OnCompleted);

            timer = new Timer(Callback);
            timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            Console.WriteLine("Press any key to terminate...");
            Console.ReadLine();
            CancellationTokenSource.Cancel();
            timer.Dispose();
            disposable.Dispose();
        }

        private static void Callback(object? state)
        {
            //Console.Clear();

            var keyValuePairs = ConcurrentDictionary.ToList();
            var top5VisitedUrl = keyValuePairs.OrderByDescending(pair => pair.Value).Take(5);

            Console.WriteLine($"Top 5 visited URL at {DateTime.UtcNow:G} :");
            foreach (var visitedUrl in top5VisitedUrl)
            {
                Console.WriteLine($"Url : {visitedUrl.Key}, Visits : {visitedUrl.Value}");
            }
        }

        private static void OnCompleted()
        {
            Console.WriteLine("No more stream to process.");
        }

        private static void OnError(Exception obj)
        {
            Console.WriteLine($"Unexpected error in processing log stream. {obj.GetBaseException().Message}");
        }

        private static void OnNext(W3CFields obj)
        {
            ConcurrentDictionary.AddOrUpdate(obj.cs_uri_stem, 1, (s, i) => i + 1);
        }
    }
}
