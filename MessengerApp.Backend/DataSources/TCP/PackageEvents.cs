namespace MessengerApp.Backend.TCP;
public class PackageEvents {
    // main class for handling receing new data from socket
    // a little more readable than any other way
    // may get combined into the channel events
    public event EventHandler<PackageReceivedEventArgs> PackageReceivedEvent = delegate {};
    public void ReceivedPackage(PackageData package) {
        OnNewPackageReceived(new (){package = package});
    }
    protected virtual void OnNewPackageReceived(PackageReceivedEventArgs e) {
        PackageReceivedEvent.Invoke(this,e);
    }
}
public class PackageReceivedEventArgs : EventArgs {
    public PackageData package {get; set;} = null!;
}