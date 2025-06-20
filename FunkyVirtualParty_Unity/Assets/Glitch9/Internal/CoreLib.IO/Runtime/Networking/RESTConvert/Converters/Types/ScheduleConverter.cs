namespace Glitch9.IO.Networking
{
    public class ScheduleConverter : RESTConveter<Schedule>
    {
        public override Schedule ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = RESTConvertUtil.SafeConvertToString(propertyValue);
            return stringValue == null ? new Schedule() : Schedule.Create(stringValue);
        }

        public override object ToCloudFormat(Schedule propertyValue)
        {
            return propertyValue.ToString();
        }
    }
}