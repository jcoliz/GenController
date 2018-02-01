namespace GenController.Portable.Models
{
    /// <summary>
    /// Defines a platform-dependent service to retrieve the current system voltage
    /// </summary>
    public interface IVoltage
    {
        double Voltage { get; }
    }
}
