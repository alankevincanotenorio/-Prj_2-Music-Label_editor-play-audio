using Gtk;
using ControllerApp;
using System.Threading.Tasks;

class GraphicInterface : Window
{
    public Controller app = new Controller();  
    private TextView rolasList;
    private Label currentPathLabel;
    private CssProvider cssProvider = new CssProvider();
    private TextView errorLogView;
    private Button changeDirButton;
    private Button miningButton;
    private Button editButton;
    private Button searchButton;
    private Button helpButton;
    private Button burgerButton;

    private ProgressBar progressBar;
    private int totalFiles = 0;


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
                background-color: #ffffff;
                color: #000000;
            }
            frame {
                background-color: #ffffff;
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
        currentPathFrame.StyleContext.AddProvider(cssProvider, uint.MaxValue);
        Box pathBox = new Box(Orientation.Horizontal, 10);
        pathBox.PackStart(currentPathFrame, true, true, 5);

        changeDirButton = new Button("Change Path");
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
        progressBar = new ProgressBar();
        miningButton = new Button("Start Mining");
        miningButton.SetSizeRequest(100, 40);
        miningButton.Clicked += OnStartMiningClick!;
        buttonBox.PackStart(miningButton, false, false, 0);
        buttonBox.PackStart(progressBar, false, false, 0);

        // "Edit"
        editButton = new Button("Edit");
        editButton.SetSizeRequest(100, 40);
        editButton.Clicked += OnEditClick!;
        buttonBox.PackStart(editButton, false, false, 0);

        // "Help"
        helpButton = new Button("Help");
        helpButton.SetSizeRequest(100, 40);
        buttonBox.PackStart(helpButton, false, false, 0);

        // "Search"
        searchButton = new Button();
        Image searchIcon = new Image(Stock.Find, IconSize.Button);
        searchButton.Image = searchIcon;
        searchButton.SetSizeRequest(40, 40);
        buttonBox.PackStart(searchButton, false, false, 0);

        // "Burger"
        burgerButton = new Button();
        Image burgerImage = new Image(Stock.Index, IconSize.Button);
        burgerButton.Image = burgerImage;
        burgerButton.SetSizeRequest(40, 40);
        buttonBox.PackStart(burgerButton, false, false, 0);
    
        // log view
        errorLogView = new TextView();
        errorLogView.Buffer.Text = "Log:\n";
        errorLogView.Editable = false;
        ScrolledWindow errorLogScrolledWindow = new ScrolledWindow();
        errorLogScrolledWindow.Add(errorLogView);
        grid.Attach(errorLogScrolledWindow, 0, 5, 4, 1);

        grid.Attach(buttonBox, 3, 1, 1, 5);

        // add grid
        mainBox.PackStart(grid, true, true, 0);
        Add(mainBox);

        if (app.GetDataBase().IsRolasTableEmpty()) DisableNonMiningActions();
        else 
        {
            List<string> rolas = app.GetRolasInfoInPath();
            rolasList.Buffer.Text = string.Join("\n", rolas);
        }

        ShowAll();
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
            bool isValidPath = app.SetCurrentPath(pathEntry.Text);
            if(!isValidPath)
            {
                changePathWindow.Destroy();
                MessageDialog errorDialog = new MessageDialog(this,DialogFlags.Modal,MessageType.Error,ButtonsType.Ok,"Invalid path. Please enter a valid directory.");
                errorDialog.Run();
                errorDialog.Destroy();
                return;
            }
            currentPathLabel.Text = $"Current Path: {pathEntry.Text}";
            changePathWindow.Destroy();
        };
        vbox.PackStart(confirmButton, false, false, 5);
        changePathWindow.Add(vbox);
        changePathWindow.ShowAll();
    }


    private async void OnStartMiningClick(object sender, EventArgs e)
    {
        miningButton.Sensitive = false;
        totalFiles = app.GetTotalMp3FilesInPath();
        progressBar.Fraction = 0;
        if(totalFiles == 0)
        {
            progressBar.Fraction = 1;
            progressBar.Text = "100%";   
        }
        await Task.Run(() => 
        {
            app.StartMining((processedFiles) =>
            {
                Application.Invoke(delegate
                {
                    float progress = (float)processedFiles / totalFiles;
                    progressBar.Fraction = progress;
                    progressBar.Text = $"{(int)(progress * 100)}%";
                });
            });
        });
        app.SetProcessedFilesNumber(0);
        List<string> rolas = app.GetRolasInfoInPath();
        rolasList.Buffer.Text = string.Join("\n", rolas);
        if (app.GetLog().Count > 0)
            errorLogView.Buffer.Text = "Error Log:\n" + string.Join("\n", app.GetMiner().GetLog());
        miningButton.Sensitive = true;
    }

    void OnEditClick(object sender, EventArgs e)
    {
        Window editWindow = new Window("Edit");
        editWindow.SetDefaultSize(300, 100);
        editWindow.SetPosition(WindowPosition.Center);
        editWindow.StyleContext.AddProvider(cssProvider, 800);

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Select object to edit.");
        vbox.PackStart(instructionLabel, false, false, 5);

        Button editRola = new Button("Edit rola");
        Button editAlbum = new Button("Edit album");
        Button definePerformer = new Button("Define Performer");
        Button addPerson = new Button("Add person to group");
        vbox.PackStart(editRola, false, false, 5);
        vbox.PackStart(editAlbum, false, false, 5);
        vbox.PackStart(definePerformer, false, false, 5);
        vbox.PackStart(addPerson, false, false, 5);

        editRola.Clicked += (s, ev) => EditRola(editWindow);



        editWindow.Add(vbox);
        editWindow.ShowAll();
    }


    void EditRola(Window editWindow)
    {
        editWindow.Destroy();
        Window editRola = new Window("Edit Rola");
        editRola.SetDefaultSize(300, 100);
        editRola.SetPosition(WindowPosition.Center);
        editRola.StyleContext.AddProvider(cssProvider, 800);

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Enter the rola song:");
        vbox.PackStart(instructionLabel, false, false, 5);

        Entry entry = new Entry();
        vbox.PackStart(entry, false, false, 5);
        entry.StyleContext.AddProvider(cssProvider, uint.MaxValue);

        Button confirm = new Button("Confirm");
        vbox.PackStart(confirm, false, false, 5);

        confirm.Clicked += (s, e) => {
            string rolaTitle = entry.Text;
            List<Rola> rolas = app.GetAllRolasInDB();
            Rola retrievedRola = rolas.Find(a => a.GetTitle() == rolaTitle);

            if (retrievedRola == null)
            {
                MessageDialog errorDialog = new MessageDialog(editRola,
                DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                "Rola not found. Please enter a valid title.");
                errorDialog.Run();
                errorDialog.Destroy();
            }
            else
            {
                string rolaDetails = app.ShowRolaDetails(retrievedRola);
                
                Window detailsWindow = new Window("Rola Details");
                detailsWindow.SetDefaultSize(300, 200);
                detailsWindow.SetPosition(WindowPosition.Center);
                detailsWindow.StyleContext.AddProvider(cssProvider, 800);

                Box detailsBox = new Box(Orientation.Vertical, 10);
                Label rolaDetailsLabel = new Label(rolaDetails);
                detailsBox.PackStart(rolaDetailsLabel, false, false, 5);
                    
                Button closeButton = new Button("Close");
                closeButton.Clicked += (closeSender, closeEvent) => detailsWindow.Destroy();
                detailsBox.PackStart(closeButton, false, false, 5);
                    
                detailsWindow.Add(detailsBox);
                detailsWindow.ShowAll();
            }
        };

        editRola.Add(vbox);
        editRola.ShowAll();
    }

    private void DisableNonMiningActions()
    {
        editButton.Sensitive = false;
        searchButton.Sensitive = false;
        helpButton.Sensitive = false;
        burgerButton.Sensitive = false;
    }

    private void AbleNonMiningActions()
    {
        editButton.Sensitive = true;
        searchButton.Sensitive = true;
        helpButton.Sensitive = true;
        burgerButton.Sensitive = true;
    }

    public static void Main()
    {
        Application.Init();
        new GraphicInterface();
        Application.Run();
    }
}