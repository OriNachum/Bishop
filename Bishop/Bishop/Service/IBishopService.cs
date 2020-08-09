using System;
using System.Threading.Tasks;

namespace Bishop.Service
{
    public interface IBishopService : IDisposable
    {
        public Task StartAsync();
    }
}