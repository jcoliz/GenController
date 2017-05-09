using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    public interface IController
    {
        GenStatus Status { get; }

        double Voltage { get; }

        Task Start();
        Task Stop();
        void Confirm();
    }
}