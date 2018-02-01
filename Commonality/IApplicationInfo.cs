namespace Commonality
{
    /// <summary>
    /// Provides information about the application to components who might like to discover it
    /// </summary>
    public interface IApplicationInfo
    {
        string Title { get; }
        string Version { get; }
    }
}
