using Gtk;
 
class SharpApp : Window {
 
    public SharpApp() : base("MusicLabelEditor")
    {
        SetDefaultSize(250, 200);
        SetPosition(WindowPosition.Center);
        DeleteEvent += delegate { Application.Quit(); }; //x or alt+f4 to close
        Show();
    }
    
    public static void Main()
    {
        Application.Init();
        new SharpApp();
        Application.Run();
    }
}