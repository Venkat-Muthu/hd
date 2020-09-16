using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UrlRanking.ServerLogSimulator
{
    public class GenerateW3CLogFile : IGenerateW3CLogFile
    {
        private readonly string _fullPath;
        private readonly TimeSpan _updateFrequency;
        private readonly CancellationToken _token;
        private readonly Timer _timer;
        private readonly object _lockObject = new object();
        private bool _isTimerOn;
        private readonly StreamWriter _streamWriter;
        private bool _disposed;

        public GenerateW3CLogFile(string fileName, TimeSpan updateFrequency, CancellationToken token)
        {
            _updateFrequency = updateFrequency;
            _token = token;
            _fullPath = Path.GetFullPath(fileName);
            Console.WriteLine($"Generating file : {_fullPath}");
            _streamWriter = new StreamWriter(_fullPath) { AutoFlush = true };
            _timer = new Timer(Callback, _timer, Timeout.Infinite, Timeout.Infinite);
        }

        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                if (!_isTimerOn)
                {
                    lock (_lockObject)
                    {
                        if (!_isTimerOn)
                        {
                            _isTimerOn = true;
                            _timer.Change(_updateFrequency, TimeSpan.FromMilliseconds(-1));
                            Console.WriteLine($"Log file : {_fullPath} generation started");
                        }
                    }
                }
            });
        }

        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _isTimerOn = false;
                }
            });
        }

        private async void Callback(object? state)
        {
            try
            {
                if (_token.IsCancellationRequested)
                {
                    Console.WriteLine($"SLM : {DateTime.UtcNow}, Cancellation requested.");
                    Dispose();
                    Console.WriteLine($"SLM : {DateTime.UtcNow}, Cancellation processed.");
                    return;
                }
                await _streamWriter.WriteLineAsync($"{Constants.FieldColumnPrefix}date time cs-uri-stem");
                var url = $"/images/picture{DateTime.UtcNow.Ticks % 10}.jpg";
                await _streamWriter.WriteLineAsync($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} {url}");
                await _streamWriter.FlushAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetBaseException().Message);
            }
            finally
            {
                _timer.Change(_isTimerOn ? _updateFrequency : TimeSpan.FromMilliseconds(-1),
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            File.Delete(_fullPath);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Console.WriteLine($"SLM : {DateTime.UtcNow}, timer disposed.");
                _timer?.Dispose();

                Console.WriteLine($"SLM : {DateTime.UtcNow}, Stream Writer disposed.");
                _streamWriter?.Dispose();
            }

            _disposed = true;
        }

        ~GenerateW3CLogFile()
        {
            Dispose(false);
        }
    }
}