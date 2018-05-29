using Commonality;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    public interface ISchedule
    {
        IObservableCollection<GenPeriod> Periods { get; }
        bool Enabled { get; set; }
        void Add(GenPeriod item);
        void Load();
        void Override();
        void Remove(GenPeriod old);
        void Replace(GenPeriod old, GenPeriod replacement);
        Task Tick();
    }
}