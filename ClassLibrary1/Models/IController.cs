using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    public interface IController
    {
        GenStatus Status { get; }
        string FullStatus { get; }

        Task Start();
        Task Stop();
    }
}