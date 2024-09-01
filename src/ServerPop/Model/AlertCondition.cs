using serverpop.Enum;

namespace serverpop.Model
{
    public class AlertCondition
    {
        public string ServerName { get; set; }

        public AlertOperator AlertOperator { get; set; }

        public int Volume { get; set; }

        public AlertCondition() { }

        public AlertCondition(string serverName, AlertOperator op, int volume)
        {
            ServerName = serverName; 
            AlertOperator = op; 
            Volume = volume;
        }
    }
}
