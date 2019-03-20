using Prism.Mvvm;

namespace ForgeModGenerator.Models
{
    public class LaunchSetup : BindableBase
    {
        public LaunchSetup(bool runClient = true, bool runServer = false)
        {
            RunClient = runClient;
            RunServer = runServer;
        }

        private bool runClient;
        public bool RunClient {
            get => runClient;
            set => SetProperty(ref runClient, value);
        }

        private bool runServer;
        public bool RunServer {
            get => runServer;
            set => SetProperty(ref runServer, value);
        }
    }
}
