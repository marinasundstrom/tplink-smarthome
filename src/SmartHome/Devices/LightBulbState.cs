namespace SmartHome.Devices
{
    public class LightBulbState
    {
        public string Mode { get; internal set; }
        public SwitchState PowerState { get; internal set; }
        public int Hue { get; internal set; }
        public int Saturation { get; internal set; }
        public int ColorTemp { get; internal set; }
        public int Brightness { get; internal set; }
    }
}
