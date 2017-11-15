namespace CalDav.Outlook
{
    public interface ISynchronizer
    {
        void Connect();
        void Prepare();
        void Sync();
        void Start();

        IController Controller { get; }
        bool Autostart { get; set; }
    }
}
