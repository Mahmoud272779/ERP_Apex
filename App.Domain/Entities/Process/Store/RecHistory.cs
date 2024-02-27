using System;

namespace App.Domain.Entities.Process
{
    public class RecHistory
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Time { get; set; }
        public int EmployeeId { get; set; }
        public string Details { get; set; }
        public string Version { get; set; }
        public string MacAdress { get; set; }
        public string OS { get; set; }
        public int HistoryType { get; set; }
    }
}
