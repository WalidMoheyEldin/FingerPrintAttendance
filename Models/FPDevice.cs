using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using zkemkeeper;

namespace FingerPrintAttendance.Models
{
    public class FPDevice
    {
        public FPDevice(int machineNumber, string name, string ip, int port)
        {
            this.MachineNumber = machineNumber;
            this.Name = name;
            this.IP = ip;
            this.Port = port;
        }

        public int ID { get; set; }

        [Display(Name = "Machine Number")]
        public int MachineNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, RegularExpression(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$", ErrorMessage = "Please insert valid IP!")]
        public string IP { get; set; }

        [Required]
        public int Port { get; set; }

        private readonly CZKEMClass fpSDK = new CZKEMClass();

        public bool isConnected()
        {
            return fpSDK.Connect_Net(this.IP, this.Port);
        }

        public DateTime? getDeviceTime()
        {
            if (isConnected())
            {
                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;
                int minute = 0;
                int second = 0;
                fpSDK.GetDeviceTime(1, ref year, ref month, ref day, ref hour, ref minute, ref second);
                DateTime time = new DateTime(year, month, day, hour, minute, second);
                return time;
            }
            else
            {
                return null;
            }
        }

        public List<AttendanceLogData> GetLiveAttendanceLogData()
        {
            List<AttendanceLogData> logs = new List<AttendanceLogData>();

            fpSDK.Disconnect();

            if (isConnected())
            {
                int enrollNumber = 0;
                int verifyMode = 0;
                int inOutMode = 0;
                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;
                int minute = 0;
                int tMachineNumber = 0;
                int eMachineNumber = 0;
                fpSDK.EnableDevice(1, false);

                if (fpSDK.ReadGeneralLogData(1))
                {
                    while (fpSDK.GetGeneralLogData(1, ref tMachineNumber, ref enrollNumber, ref eMachineNumber, ref verifyMode, ref inOutMode, ref year, ref month, ref day, ref hour, ref minute))
                    {
                        logs.Add(new AttendanceLogData()
                        {
                            enrollNumber = enrollNumber,
                            inOutMode = (AttendanceLogData.InOrOur)inOutMode,
                            dateAndTime = new DateTime(year, month, day, hour, minute, 0),
                            DeviceId = ID
                        });
                    }
                }

                fpSDK.EnableDevice(1, true);
            }

            fpSDK.Disconnect();

            return logs;
        }
    }
}