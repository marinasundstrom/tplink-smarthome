using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    public sealed class NotificationBus
    {
        private static NotificationBus instance;

        protected NotificationBus()
        {

        }

        public void NotifyDeviceAdded(Device device)
        {

        }

        public void NotifyDeviceUpdated(Device device)
        {

        }

        public static NotificationBus Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new NotificationBus();
                }
                return instance;
            }
        }
    }
}
