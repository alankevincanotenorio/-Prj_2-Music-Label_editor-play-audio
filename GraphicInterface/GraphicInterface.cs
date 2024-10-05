using Gtk;
using ControllerApp;

class GraphicInterface : Window
{
    public Controller app = new Controller();  
    private TextView rolasList;
    private Label currentPathLabel;
    CssProvider cssProvider = new CssProvider();

    public GraphicInterface() : base("Music Library Editor")
    {
        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);
        BorderWidth = 10;
        Resizable = true;

        //to change background color
        cssProvider.LoadFromData(@"
            textview {
                background-color: #a9a9a9;
                color: #000000;
            }
            text {
                background-color: #a9a9a9;
                color: #282828;
            }
            window {
                background-color: #999999;
            }
            entry {
                background-color: #d3d3d3;
                color: #000000;
            }
        ");
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);

        //close app
        DeleteEvent += (sender, args) => Application.Quit();

        Box mainBox = new Box(Orientation.Vertical, 10);

        Grid grid = new Grid();
        grid.RowSpacing = 10;
        grid.ColumnSpacing = 10;
        grid.Margin = 10;

        // show current path
        currentPathLabel = new Label($"Current Path: {app.GetCurrentPath()}");
        Frame currentPathFrame = new Frame();
        currentPathFrame.Add(currentPathLabel);
        currentPathFrame.ShadowType = ShadowType.EtchedIn;
        Box pathBox = new Box(Orientation.Horizontal, 10);
        pathBox.PackStart(currentPathFrame, true, true, 5);

        Button changeDirButton = new Button("Change Path");
        changeDirButton.SetSizeRequest(100, 40);
        changeDirButton.Clicked += OnChangeDirClick!;
        pathBox.PackStart(changeDirButton, false, false, 5);
        grid.Attach(pathBox, 0, 0, 4, 1);

        // TextView to show mined rolas inside a ScrolledWindow
        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.Vexpand = true;
        scrolledWindow.Hexpand = true;

        rolasList = new TextView();
        rolasList.StyleContext.AddProvider(cssProvider, uint.MaxValue);
        rolasList.Buffer.Text = "Rolas will be displayed here...";
        rolasList.WrapMode = WrapMode.Word;
        rolasList.Editable = false;

        scrolledWindow.Add(rolasList);
        grid.Attach(scrolledWindow, 0, 1, 3, 4);

        Box buttonBox = new Box(Orientation.Vertical, 10);
        // "Start mining"
        Button miningButton = new Button("Start Mining");
        miningButton.SetSizeRequest(100, 40);
        miningButton.Clicked += OnMineClick!;
        buttonBox.PackStart(miningButton, false, false, 0);
        // "Edit"
        Button editButton = new Button("Edit");
        editButton.SetSizeRequest(100, 40);
        buttonBox.PackStart(editButton, false, false, 0);
        // "Help"
        Button helpButton = new Button("Help");
        helpButton.SetSizeRequest(100, 40);
        buttonBox.PackStart(helpButton, false, false, 0);
        // "Search"
        Button searchButton = new Button();
        Image searchIcon = new Image(Stock.Find, IconSize.Button);
        searchButton.Image = searchIcon;
        searchButton.SetSizeRequest(40, 40);
        buttonBox.PackStart(searchButton, false, false, 0);
        // "Burger"
        Button burgerButton = new Button();
        Image burgerImage = new Image(Stock.Index, IconSize.Button);
        burgerButton.Image = burgerImage;
        burgerButton.SetSizeRequest(40, 40);
        buttonBox.PackStart(burgerButton, false, false, 0);
        
        grid.Attach(buttonBox, 3, 1, 1, 5);

        // add grid
        mainBox.PackStart(grid, true, true, 0);
        Add(mainBox);

        ShowAll();
    }

    // manage miner
    void OnMineClick(object sender, EventArgs args)
    {
        app.StartMining();
        List<string> rolas = app.GetRolasInfoInPath();
        rolasList.Buffer.Text = string.Join("\n", rolas);
    }

    // manage change directory
    void OnChangeDirClick(object sender, EventArgs args)
    {
        Window changePathWindow = new Window("Change Path");
        changePathWindow.SetDefaultSize(300, 100);
        changePathWindow.SetPosition(WindowPosition.Center);
        changePathWindow.StyleContext.AddProvider(cssProvider, 800);

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Insert the new path:");
        vbox.PackStart(instructionLabel, false, false, 5);

        Entry pathEntry = new Entry();
        vbox.PackStart(pathEntry, false, false, 5);
        pathEntry.StyleContext.AddProvider(cssProvider, uint.MaxValue);

        Button confirmButton = new Button("Confirm");
        confirmButton.Clicked += (s, e) => {
            string newPath = pathEntry.Text;
            app.SetCurrentPath(newPath);
            currentPathLabel.Text = $"Current Path: {newPath}";
            changePathWindow.Destroy();
        };
        vbox.PackStart(confirmButton, false, false, 5);

        changePathWindow.Add(vbox);
        changePathWindow.ShowAll();
    }

    public static void Main()
    {
        Application.Init();
        new GraphicInterface();
        Application.Run();
    }
}
