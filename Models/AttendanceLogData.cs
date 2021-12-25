using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace FingerPrintAttendance.Models
{
    public class AttendanceLogData
    {
        public enum InOrOur
        {
            In,
            Out
        }

        public int ID { get; set; }
        public int DeviceId { get; set; }
        public int enrollNumber { get; set; }
        public InOrOur inOutMode { get; set; }
        public DateTime dateAndTime { get; set; }

        [ForeignKey("DeviceId")]
        public virtual FPDevice FingerprintDevice { get; set; }
    }
}