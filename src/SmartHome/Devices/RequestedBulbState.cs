namespace SmartHome.Devices
{
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct RequestedBulbState
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        public SwitchState? PowerState { get; set; }
        public int? Brightness { get; set; }
        public int? Hue { get; set; }
        public int? Saturation { get; set; }
        public int? ColorTemp { get; set; }
    }
}
