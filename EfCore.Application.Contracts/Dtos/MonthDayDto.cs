using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCore.Application.Contracts.Dtos
{
    public class MonthDayDto
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsWeekend { get; set; }
    }
}
