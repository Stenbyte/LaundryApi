using LaundryApi.Exceptions;

namespace LaundryApi.Validators
{
    public class TimeSlotValidator
    {
        public static bool IsTimeSlotInThePast(List<string> timeSlot)
        {

            var nowUtc = DateTime.UtcNow;
            string[] timeParts = timeSlot[0].Split("-");
            var endTime = timeParts[1];
            var endHour = int.Parse(endTime.Split(":")[0]);
            var endMinute = int.Parse(endTime.Split(":")[1]);

            var slotEndUtc = DateTime.UtcNow.AddHours(endHour).AddMinutes(endMinute);
           return slotEndUtc < nowUtc;
        }
    }
}