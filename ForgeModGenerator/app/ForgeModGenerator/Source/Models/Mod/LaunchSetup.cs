using GalaSoft.MvvmLight;

namespace ForgeModGenerator.Models
{
    public class LaunchSetup : ObservableObject
    {
        public LaunchSetup(bool runClient = true, bool runServer = false)
        {
            RunClient = runClient;
            RunServer = runServer;
        }

        private bool runClient;
        public bool RunClient {
            get => runClient;
            set => Set(ref runClient, value);
        }

        private bool runServer;
        public bool RunServer {
            get => runServer;
            set => Set(ref runServer, value);
        }
    }
}
