using Commonality;
using GenController.Portable.Models;
using System;
using System.Windows.Input;

namespace GenController.Portable.ViewModels
{
    public class EditScheduleViewModel: ViewModelBase
    {
        public EditScheduleViewModel(): base(Service.TryGet<ILogger>())
        {
        }

        public Models.GenPeriod Period { get; private set; } = new Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(13),14.0);

        public Models.GenPeriod Original
        {
            set
            {
                try
                {
                    _Original = value;
                    WillAdd = false;
                    Period = new Models.GenPeriod(_Original.StartAt, _Original.StopAt, _Original.Voltage);
                    base.SetProperty(nameof(Period));
                    base.SetProperty(nameof(WillAdd));
                }
                catch (Exception ex)
                {
                    base.SetError("EV4", ex);
                }
            }
        }
        private Models.GenPeriod _Original;

        public bool WillDelete { get; set; }

        public bool WillAdd { get; set; } = true;

        public ICommand AddCommand => new DelegateCommand((x) =>
        {
            try
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
                    case "V":
                        Period.Voltage += add * 0.2;
                        if (Period.Voltage > 0.0 && Period.Voltage < 1.0)
                            Period.Voltage = 12.0;
                        else if (Period.Voltage < 12.0)
                            Period.Voltage = 0.0;
                        else if (Period.Voltage > 14.0)
                            Period.Voltage = 14.0;
                        break;
                }
                base.SetProperty(nameof(Period));
            }
            catch (Exception ex)
            {
                base.SetError("EV1", ex);
            }
        });

        public ICommand DeleteMeCommand => new DelegateCommand((x) =>
        {
            try
            {
                WillDelete = !WillDelete;
                base.SetProperty(nameof(WillDelete));
            }
            catch (Exception ex)
            {
                base.SetError("EV2", ex);
            }
        });
        
        public void Commit()
        {
            try
            {
                if (WillDelete)
                    Schedule.Remove(_Original);
                else if (WillAdd)
                    Schedule.Add(Period);
                else
                    Schedule.Replace(_Original, Period);
            }
            catch (Exception ex)
            {
                base.SetError("EV3", ex);
            }
        }
        private ISchedule Schedule => Service.Get<ISchedule>();

    }
}
