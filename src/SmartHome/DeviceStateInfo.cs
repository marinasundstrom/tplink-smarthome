namespace SmartHome
{
    public class DeviceStateInfo
    {
        public DeviceStateInfo(Device device, DeviceState state)
        {
            Device = device;
            State = state;
        }

        public Device Device { get; }

        public DeviceState State { get; }
    }
}
