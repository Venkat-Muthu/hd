using System;
using System.Threading.Tasks;

namespace UrlRanking.ServerLogSimulator
{
    public interface IGenerateW3CLogFile : IDisposable
    {
        Task StartAsync();
        Task StopAsync();
    }
}