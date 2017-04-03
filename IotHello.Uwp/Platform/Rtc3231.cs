using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace IotHello.Iot.Platform
{
    // http://stackoverflow.com/questions/34964682/windows-iot-and-ds3231-rtc-clock
    public class Rtc3231: IClock
    {
        public DateTime Now { get; set; }

        public async Task Update()
        {
            await Init();

            using (I2cDevice Device = await I2cDevice.FromIdAsync(DIS[0].Id, Settings))
            {
                byte[] writeBuf = { 0x00 };
                Device.Write(writeBuf);
                byte[] readBuf = new byte[7];
                Device.Read(readBuf);
                byte second = bcdToDec((byte)(readBuf[0] & 0x7f));
                byte minute = bcdToDec(readBuf[1]);
                byte hour = bcdToDec((byte)(readBuf[2] & 0x3f));
                byte dayOfWeek = bcdToDec(readBuf[3]);
                byte dayOfMonth = bcdToDec(readBuf[4]);
                byte month = bcdToDec(readBuf[5]);
                byte year = bcdToDec(readBuf[6]);

                Now = new DateTime(year, month, dayOfMonth, hour, minute, second);
            }
        }

        public async Task Set(DateTime value)
        {
            await Init();

            using (I2cDevice Device = await I2cDevice.FromIdAsync(DIS[0].Id, Settings))
            {
                byte write_seconds = decToBcd((byte)value.Second);
                byte write_minutes = decToBcd((byte)value.Minute);
                byte write_hours = decToBcd((byte)value.Hour);
                byte write_dayofweek = decToBcd((byte)value.DayOfWeek);
                byte write_day = decToBcd((byte)value.Day);
                byte write_month = decToBcd((byte)value.Month);
                byte write_year = decToBcd((byte)value.Year);

                byte[] write_time = { 0x00, write_seconds, write_minutes, write_hours, write_dayofweek, write_day, write_month, write_year };

                Device.Write(write_time);
            }
        }

        private string AQS;
        private DeviceInformationCollection DIS;
        private I2cConnectionSettings Settings;

        /* DS3231 I2C SLAVE address */
        const int SlaveAddress = 0x68;

        private async Task Init()
        {
            // Initialize I2C
            Settings = new I2cConnectionSettings(SlaveAddress);
            Settings.BusSpeed = I2cBusSpeed.StandardMode;

            if (AQS == null || DIS == null)
            {
                AQS = I2cDevice.GetDeviceSelector("I2C1");
                DIS = await DeviceInformation.FindAllAsync(AQS);
            }
        }

        // Convert normal decimal numbers to binary coded decimal
        private static byte decToBcd(byte val)
        {
            return (byte)(((int)val / 10 * 16) + ((int)val % 10));
        }

        private static byte bcdToDec(byte val)
        {
            return (byte)(((int)val / 16 * 10) + ((int)val % 16));
        }

        public Task Delay(TimeSpan t)
        {
            throw new NotImplementedException();
        }
    }
}
