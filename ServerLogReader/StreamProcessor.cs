using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace UrlRanking.ServerLogReader
{
    public class StreamProcessor //TO DO : Extract interface and use IoC container to map interface and instance
    {
        public IObservable<W3CFields> MonitorLogFileObservable(string fileName, Func<string, Task<string[]>> header, Func<string[], string, Task<W3CFields>> processor, IScheduler scheduler = null)
        {
            return Observable.Create<W3CFields>(async obs =>
            {
                var fullPath = Path.GetFullPath(fileName);
                Console.WriteLine($"Reading file : {fullPath}");
                var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite);
                using var streamReader = new StreamReader(fs);
                bool isPrevLineFieldHeader = false;
                string[] headers = null;
                do
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        if (line.StartsWith(Constants.FieldColumnPrefix))
                        {
                            isPrevLineFieldHeader = true;
                            headers = await header(line);
                        }
                        else if (isPrevLineFieldHeader)
                        {
                            var w3CFields = await processor(headers, line).ConfigureAwait(false);
                            obs.OnNext(w3CFields);
                            isPrevLineFieldHeader = false;
                        }

                        if (streamReader.EndOfStream)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }
                } while (IsFileLocked(fullPath));
                obs.OnCompleted();
                return Disposable.Empty;
            });

        }

        public bool IsFileLocked(string filename)
        {
            bool locked = false;
            try
            {
                using FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                        FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                locked = true;
            }
            return locked;
        }
    }
}