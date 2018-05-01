namespace SmartHome.Devices
{
    public struct RequestedBulbState
    {
        public SwitchState? PowerState { get; set; }
        public int? Brightness { get; set; }
        public int? Hue { get; set; }
        public int? Saturation { get; set; }
        public int? ColorTemp { get; set; }
    }

}
