using System.ComponentModel;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    public interface IController: INotifyPropertyChanged
    {
        GenStatus Status { get; }

        double Voltage { get; }

        bool Enabled { get; set; }

        Task Start();
        Task Stop();
        void Confirm();
    }
}