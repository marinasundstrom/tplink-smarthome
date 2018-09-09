namespace SmartHome
{
    public sealed class NotificationBus
    {
        private static NotificationBus s_instance;

        private NotificationBus()
        {

        }

        public static NotificationBus Instance
        {
            get
            {
                return s_instance ?? (s_instance = new NotificationBus());
            }
        }
    }
}
