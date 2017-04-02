using ManiaLabs.Helpers;
using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IotHello.Portable.ViewModels
{
    public class EditScheduleViewModel: ViewModelBase
    {
        public Models.GenPeriod Period { get; private set; } = new Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(13));

        public Models.GenPeriod Original
        {
            //get { return _Original; }
            set
            {
                _Original = value;
                WillAdd = false;
                Period = new Models.GenPeriod(_Original.StartAt, _Original.StopAt);
                base.SetProperty(nameof(Period));
                base.SetProperty(nameof(WillAdd));
            }

        }
        private Models.GenPeriod _Original;

        public bool WillDelete { get; set; }

        public bool WillAdd { get; set; } = true;

        public ICommand AddCommand => new DelegateCommand((x) =>
        {
            string what = x as string;
            char direction = what[0];
            int add = 1;
            if (direction == '-')
                add = -1;
            switch (what.Substring(1))
            {
                case "Hfrom":
                    Period.StartAt = Period.StartAt.Add(TimeSpan.FromHours(add));
                    if (Period.StartAt < TimeSpan.Zero)
                    {
                        Period.StartAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StartAt >= TimeSpan.FromDays(1))
                    {
                        Period.StartAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StopAt = Period.StartAt;
                    }
                    break;
                case "Mfrom":
                    Period.StartAt = Period.StartAt.Add(TimeSpan.FromMinutes(add * 5));
                    if (Period.StartAt < TimeSpan.Zero)
                    {
                        Period.StartAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StartAt >= TimeSpan.FromDays(1))
                    {
                        Period.StartAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StopAt = Period.StartAt;
                    }
                    break;
                case "Hto":
                    Period.StopAt = Period.StopAt.Add(TimeSpan.FromHours(add));
                    if (Period.StopAt < TimeSpan.Zero)
                    {
                        Period.StopAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StopAt >= TimeSpan.FromDays(1))
                    {
                        Period.StopAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StartAt = Period.StopAt;
                    }
                    break;
                case "Mto":
                    Period.StopAt = Period.StopAt.Add(TimeSpan.FromMinutes(add * 5));
                    if (Period.StopAt < TimeSpan.Zero)
                    {
                        Period.StopAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StopAt >= TimeSpan.FromDays(1))
                    {
                        Period.StopAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StartAt = Period.StopAt;
                    }
                    break;
            }
            base.SetProperty(nameof(Period));

        });

        public ICommand DeleteMeCommand => new DelegateCommand((x) =>
        {
            WillDelete = !WillDelete;
            base.SetProperty(nameof(WillDelete));
        });
        
        public void Commit()
        {
            if (WillDelete)
                Models.Schedule.Current.Remove(_Original);
            else if (WillAdd)
                Models.Schedule.Current.Add(Period);
            else
                Models.Schedule.Current.Replace(_Original, Period);
        }
    }
}
