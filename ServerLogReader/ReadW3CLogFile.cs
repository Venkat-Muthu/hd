using System;
using System.Threading.Tasks;

namespace UrlRanking.ServerLogReader
{
    public class ReadW3CLogFile
    {
        readonly StreamProcessor _streamProcessor = new StreamProcessor();
        public IObservable<W3CFields> GetW3CFieldsObservable(string fileName)
        {
            var monitorLogFile = _streamProcessor.MonitorLogFileObservable(fileName, DeSerialiseColumnHeader, DeSerialiseValue);
            return monitorLogFile;
        }

        public Task<string[]> DeSerialiseColumnHeader(string line)
        {
            return Task.FromResult(line.TrimStart(Constants.FieldColumnPrefix).Split(' '));
        }

        public Task<W3CFields> DeSerialiseValue(string[] header, string line)
        {
            return Task.Factory.StartNew(() =>
            {
                W3CFields w3CFields = null;
                var value = line.Split(' ');

                var tryParse = DateTime.TryParse($"{value[0]} {value[1]}", out DateTime dateTime);

                if (tryParse)
                {
                    var csUriStemName = nameof(W3CFields.cs_uri_stem).Replace("_", "-");
                    int urlIndex = -1;
                    for (int i = 0; i < header.Length; i++)
                    {
                        if (string.Compare(header[i], csUriStemName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            urlIndex = i;
                            break;
                        }
                    }
                    var url = value[urlIndex];
                    w3CFields = new W3CFields(dateTime, url);
                }
                return w3CFields;
            });
        }
    }
}