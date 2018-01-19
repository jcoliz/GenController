using System.Threading.Tasks;

namespace GenController.Portable.Models
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