using Gtk;
using ControllerApp;

class GraphicInterface : Window
{
    public Controller app = new Controller();  
    private VBox vbox;

    public GraphicInterface() : base("Music Library Editor")
    {
        SetDefaultSize(400, 300);
        SetPosition(WindowPosition.Center);
        

        DeleteEvent += delegate { Application.Quit(); };

        Fixed fix = new Fixed();
        Button miningButton = new Button("Start Mining");
        miningButton.Clicked += OnMineClick;
        miningButton.SetSizeRequest(100, 40);
        fix.Put(miningButton, 500, 60);

        Button changeDirButton = new Button("Change");
        changeDirButton.SetSizeRequest(100, 40);
        fix.Put(changeDirButton, 500, 20);

        Button editButton = new Button("Edit");
        editButton.SetSizeRequest(100, 40);
        fix.Put(editButton, 500, 100);

        Button searchButton = new Button("Search"); //insert lupa icon
        searchButton.SetSizeRequest(100, 40);
        fix.Put(searchButton, 500, 160);

        Button burgerButton = new Button("Burger"); //insert burger icon
        burgerButton.SetSizeRequest(100, 40);        
        fix.Put(burgerButton, 500, 200);
    
        vbox = new VBox();
        fix.Put(vbox, 50, 100);  
        Add(fix);
        ShowAll();
    }

    void OnMineClick(object sender, EventArgs args)
    {
        foreach (Widget child in vbox.Children)
        {
            vbox.Remove(child); 
            child.Destroy();    
        }
        app.StartMining(); 
        List<string> rolas = app.GetRolasInfoInPath();
        foreach (string title in rolas)
        {
            Label titleLabel = new Label(title); 
            vbox.PackStart(titleLabel, false, false, 5);
        }
        vbox.ShowAll();  
    }

    public static void Main()
    {
        Application.Init();
        new GraphicInterface();
        Application.Run();
    }
}
