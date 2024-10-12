using Gtk;
using ControllerApp;
using System.Threading.Tasks;

class GraphicInterface : Window
{
    public Controller app = new Controller();  
    private Label currentPathLabel;
    private CssProvider cssProvider = new CssProvider();
    private TextView errorLogView;
    private Button changeDirButton;
    private Button miningButton;
    private Button editRolaButton;
    private Button editAlbumButton;
    private Button definePerformerButton;
    private Button addPersonButton;
    private Button searchButton;
    private Button helpButton;
    private ProgressBar progressBar;
    private int totalFiles = 0;
    private Grid grid;
    private ScrolledWindow rolasScrolledWindow;
    private Box rolasBox;

    public GraphicInterface() : base("Music Library Editor")
    {
        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);
        BorderWidth = 10;
        Resizable = true;

        cssProvider.LoadFromData(@"
            textview {
                background-color: #2e2e2e;
                color: #d3d3d3;
            }
            text {
                background-color: #3b3b3b;
                color: #d3d3d3;
            }
            window {
                background-color: #2e2e2e;
            }
            entry {
                background-color: #3b3b3b;
                color: #d3d3d3;
                caret-color: #ffffff;
            }
            frame {
                background-color: #3b3b3b;
                color: #d3d3d3;
                border-radius: 12px;
                border-width: 1px;
                border-color: #444444;
            }
            label.Child-label {
                color : #d3d3d3;
            }
        ");
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);

        //close app
        DeleteEvent += (sender, args) => Application.Quit();

        Box mainBox = new Box(Orientation.Vertical, 10);

        grid = new Grid();
        grid.RowSpacing = 10;
        grid.ColumnSpacing = 10;
        grid.Margin = 10;

        // show current path
        currentPathLabel = new Label($"Current Path: {app.GetCurrentPath()}");
        Frame currentPathFrame = new Frame();
        currentPathFrame.Add(currentPathLabel);
        currentPathFrame.ShadowType = ShadowType.None;
        currentPathFrame.StyleContext.AddProvider(cssProvider, uint.MaxValue);
        Box pathBox = new Box(Orientation.Horizontal, 10);
        pathBox.PackStart(currentPathFrame, true, true, 5);

        changeDirButton = new Button("Change Path");
        changeDirButton.SetSizeRequest(100, 40);
        changeDirButton.Clicked += OnChangeDirClick!;
        pathBox.PackStart(changeDirButton, false, false, 5);
        grid.Attach(pathBox, 0, 0, 4, 1);

        Box buttonBox = new Box(Orientation.Vertical, 10);

        // "Start mining"
        progressBar = new ProgressBar();
        miningButton = new Button("Start Mining");
        miningButton.SetSizeRequest(100, 40);
        miningButton.Clicked += OnStartMiningClick!;
        buttonBox.PackStart(miningButton, false, false, 0);
        buttonBox.PackStart(progressBar, false, false, 0);

        // "Edit Rola"
        editRolaButton = new Button("Edit rola");
        editRolaButton.SetSizeRequest(100, 40);
        editRolaButton.Clicked += OnEditRolaClick!;
        buttonBox.PackStart(editRolaButton, false, false, 0);

        // "Edit album"
        editAlbumButton = new Button("Edit album");
        editAlbumButton.SetSizeRequest(100, 40);
        editAlbumButton.Clicked += OnEditAlbumButton!;
        buttonBox.PackStart(editAlbumButton, false, false, 0);

        // "Define performer"
        definePerformerButton = new Button("Define Performer");
        definePerformerButton.SetSizeRequest(100, 40);
        definePerformerButton.Clicked += OnDefinePerformerButton!;
        buttonBox.PackStart(definePerformerButton, false, false, 0);

        // "Add person to group"
        addPersonButton = new Button("Add person to group");
        addPersonButton.SetSizeRequest(100, 40);
        addPersonButton.Clicked += OnAddPersonGroup!;
        buttonBox.PackStart(addPersonButton, false, false, 0);

        // "Search"
        searchButton = new Button();
        Image searchIcon = new Image(Stock.Find, IconSize.Button);
        searchButton.Image = searchIcon;
        searchButton.Clicked += OnSearchButton!;
        searchButton.SetSizeRequest(40, 40);
        buttonBox.PackStart(searchButton, false, false, 0);

        // "Help"
        helpButton = new Button("Help");
        helpButton.SetSizeRequest(100, 40);
        buttonBox.PackStart(helpButton, false, false, 0);
    
        // log view
        errorLogView = new TextView();
        errorLogView.Buffer.Text = "Log:\n";
        errorLogView.Editable = false;
        ScrolledWindow errorLogScrolledWindow = new ScrolledWindow();
        errorLogScrolledWindow.Add(errorLogView);
        grid.Attach(errorLogScrolledWindow, 0, 5, 4, 1);

        grid.Attach(buttonBox, 3, 1, 1, 5);

        rolasScrolledWindow = new ScrolledWindow{Vexpand = true, Hexpand = true};
        rolasBox = new Box(Orientation.Vertical, 10);        

        if(app.AreRolasInDatabase())
        {
            Label placeholderLabel = new Label("Aquí aparecerán las rolas...");
            rolasBox.PackStart(placeholderLabel, false, false, 10);
            DisableNonMiningActions();
        }
        else ShowRolasWithCoverArt();
        rolasScrolledWindow.Add(rolasBox);
        grid.Attach(rolasScrolledWindow, 0, 1, 3, 4);
        mainBox.PackStart(grid, true, true, 0);
        Add(mainBox);
        ShowAll();
    }

    // ready
    void OnChangeDirClick(object sender, EventArgs args)
    {
        Window changePathWindow = new Window("Change Path");
        changePathWindow.SetDefaultSize(600, 100);
        changePathWindow.SetPosition(WindowPosition.Center);
        changePathWindow.TransientFor = this;
        changePathWindow.Modal = true;

        changePathWindow.Resizable = false;
    
        changePathWindow.StyleContext.AddProvider(cssProvider, 800);

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Insert the new path:");
        instructionLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(instructionLabel, false, false, 5);

        Entry pathEntry = new Entry();
        vbox.PackStart(pathEntry, false, false, 5);
        pathEntry.StyleContext.AddProvider(cssProvider, uint.MaxValue);

        Button confirmButton = new Button("Confirm");
        confirmButton.Clicked += (s, e) => {            
            bool isValidPath = app.SetCurrentPath(pathEntry.Text);
            if(!isValidPath)
            {
                MessageDialog errorDialog = new MessageDialog(this,DialogFlags.Modal,MessageType.Error,ButtonsType.Ok,"Invalid path. Please enter a valid directory.");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
            }
            else
            {
                currentPathLabel.Text = $"Current Path: {pathEntry.Text}";
                changePathWindow.Hide();
                changePathWindow.Dispose();
            }
        };
        vbox.PackStart(confirmButton, false, false, 5);
        changePathWindow.Add(vbox);
        changePathWindow.ShowAll();
    }

    // ready
    private async void OnStartMiningClick(object sender, EventArgs e)
    {
        miningButton.Sensitive = false;
        totalFiles = app.GetTotalMp3FilesInPath();
        progressBar.Fraction = 0;

        if (totalFiles == 0)
        {
            progressBar.Fraction = 1;
            progressBar.Text = "100%";
        }

        foreach (Widget child in rolasBox.Children)
        {
            rolasBox.Remove(child);
        }

        await Task.Run(() =>
        {
            app.StartMining((processedFiles) =>
            {
                Application.Invoke(delegate
                {
                    float progress = app.GetProgress((float)processedFiles, totalFiles);
                    progressBar.Fraction = progress;
                    progressBar.Text = $"{(int)(progress * 100)}%";
                });
            });
        });

        app.SetProcessedFilesNumber(0);

        ShowRolasWithCoverArt();

        List<(string rolaInfo, Gdk.Pixbuf albumCover)> rolasWithCovers = app.GetRolasInfoWithCovers();

        foreach (Widget child in rolasBox.Children)
        {
            rolasBox.Remove(child);
        }
        foreach (var (rolaInfo, albumCover) in rolasWithCovers)
        {
            Box rolaBox = new Box(Orientation.Horizontal, 10);

            Gtk.Image albumImage = new Gtk.Image(albumCover.ScaleSimple(100, 100, Gdk.InterpType.Bilinear));

            Label rolaLabel = new Label();
            rolaLabel.Text = rolaInfo;
            rolaLabel.Xalign = 0.0f;
            rolaLabel.Yalign = 0.5f;
            rolaLabel.StyleContext.AddClass("Child-label");
            rolaBox.PackStart(albumImage, false, false, 5);
            rolaBox.PackStart(rolaLabel, true, true, 5);
            rolasBox.PackStart(rolaBox, false, false, 10);
        }
        rolasBox.ShowAll();
        rolasScrolledWindow.ShowAll();
        if (app.GetLog().Count > 0)
            errorLogView.Buffer.Text = "Log:\n" + string.Join("\n", app.GetLog());
        miningButton.Sensitive = true;
        EnableNonMiningActions();
    }

    // ready
    private void ShowRolasWithCoverArt()
    {
        List<(string rolaInfo, Gdk.Pixbuf albumCover)> rolasWithCovers = app.GetRolasInfoWithCovers();
        foreach (var (rolaInfo, albumCover) in rolasWithCovers)
        {
            Box rolaBox = new Box(Orientation.Horizontal, 10);
            Gtk.Image albumImage = new Gtk.Image(albumCover.ScaleSimple(100, 100, Gdk.InterpType.Bilinear));
            Label rolaLabel = new Label();
            rolaLabel.Text = rolaInfo;
            rolaLabel.Xalign = 0.0f;
            rolaLabel.Yalign = 0.5f;
            rolaLabel.StyleContext.AddClass("Child-label");
            rolaBox.PackStart(albumImage, false, false, 5);
            rolaBox.PackStart(rolaLabel, true, true, 5);
            rolasBox.PackStart(rolaBox, false, false, 10);
        }
        rolasBox.ShowAll();
    }

    //ready
    void OnEditRolaClick(object sender, EventArgs e)
    {
        Window editRola = new Window("Edit Rola");
        editRola.SetDefaultSize(600, 100);
        editRola.SetPosition(WindowPosition.Center);
        editRola.StyleContext.AddProvider(cssProvider, 800);
        editRola.TransientFor = this;
        editRola.Modal = true;
        editRola.Resizable = false;

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Enter the rola title:");
        instructionLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(instructionLabel, false, false, 5);

        Entry entry = new Entry();
        vbox.PackStart(entry, false, false, 5);
        entry.StyleContext.AddProvider(cssProvider, uint.MaxValue);

        Button confirm = new Button("Confirm");
        vbox.PackStart(confirm, false, false, 5);

        confirm.Clicked += (s, e) => {
            string rolaTitle = entry.Text;
            List<string> rolasOptions = app.GetRolasOptions(rolaTitle);
            if (rolasOptions.Count == 0)
            {
                MessageDialog errorDialog = new MessageDialog(editRola, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No rola found with that title. Please enter a valid title.");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
            }
            else if (rolasOptions.Count == 1)
            {
                editRola.Hide();
                editRola.Dispose();
                List<string> rolaDetails = app.GetRolaDetails(rolaTitle, rolasOptions.First());
                ShowEditForm(rolaTitle, rolasOptions.First(), rolaDetails);
            }
            else
            {
                editRola.Hide();
                editRola.Dispose();
                Window selectionWindow = new Window("Select Rola");
                selectionWindow.SetDefaultSize(800, 600);
                selectionWindow.SetPosition(WindowPosition.Center);
                selectionWindow.StyleContext.AddProvider(cssProvider, 800);
                selectionWindow.TransientFor = this;
                selectionWindow.Modal = true;
                selectionWindow.Resizable = false;

                ScrolledWindow scrolledWindow = new ScrolledWindow{Vexpand = true, Hexpand = true};

                Box selectionVbox = new Box(Orientation.Vertical, 10);
                Label selectLabel = new Label("Select the Rola to edit:");
                selectLabel.Xalign = 0.0f;
                selectLabel.StyleContext.AddClass("Child-label");
                selectionVbox.PackStart(selectLabel, false, false, 5);

                var rolasWithCovers = app.GetRolasInfoWithCovers();
                foreach (var rolaPath in rolasOptions)
                {
                    var rolaWithCover = rolasWithCovers.FirstOrDefault(rola => rola.rolaInfo.Contains(rolaPath));
                    if (rolaWithCover != default)
                    {
                        Box rolaBox = new Box(Orientation.Horizontal, 10);
                        Gtk.Image albumImage = new Gtk.Image(rolaWithCover.albumCover.ScaleSimple(100, 100, Gdk.InterpType.Bilinear));
                        Label rolaLabel = new Label(rolaWithCover.rolaInfo);
                        rolaLabel.Xalign = 0.0f;
                        rolaLabel.Yalign = 0.5f;
                        
                        Button rolaButton = new Button();
                        
                        rolaButton.Add(rolaBox);
                        rolaBox.PackStart(albumImage, false, false, 5);
                        rolaBox.PackStart(rolaLabel, true, true, 5);
                        selectionVbox.PackStart(rolaButton, false, false, 10);
                        rolaButton.Clicked += (sender, args) => 
                        {
                            selectionWindow.Hide();
                            selectionWindow.Dispose();
                            ShowEditForm(rolaTitle, rolaPath, app.GetRolaDetails(rolaTitle, rolaPath));
                        };
                    }
                }
                scrolledWindow.Add(selectionVbox);
                selectionWindow.Add(scrolledWindow);
                selectionWindow.ShowAll();
            }
        };
        editRola.Add(vbox);
        editRola.ShowAll();
    }

    //ready
    void ShowEditForm(string rolaTitle, string rolaPath, List<string> rolaDetails)
    {
        Window detailsWindow = new Window("Edit Rola");
        detailsWindow.SetDefaultSize(800, 600);
        detailsWindow.SetPosition(WindowPosition.Center);
        detailsWindow.StyleContext.AddProvider(cssProvider, 800);
        detailsWindow.TransientFor = this;
        detailsWindow.Modal = true;

        detailsWindow.Resizable = false;

        ScrolledWindow scrolledWindow = new ScrolledWindow{Vexpand = true, Hexpand = true};

        Box detailsBox = new Box(Orientation.Vertical, 10);

        var rolaWithCover = app.GetRolasInfoWithCovers().FirstOrDefault(rola => rola.rolaInfo.Contains(rolaTitle) && rola.rolaInfo.Contains(rolaPath));
        if (rolaWithCover != default)
        {
            Box rolaBox = new Box(Orientation.Horizontal, 10);
            Gtk.Image albumImage = new Gtk.Image(rolaWithCover.albumCover.ScaleSimple(100, 100, Gdk.InterpType.Bilinear));
            
            Label rolaLabel = new Label(rolaWithCover.rolaInfo);
            rolaLabel.Xalign = 0.0f;
            rolaLabel.Yalign = 0.5f;
            rolaLabel.StyleContext.AddClass("Child-label");
            
            rolaBox.PackStart(albumImage, false, false, 5);
            rolaBox.PackStart(rolaLabel, true, true, 5);
            
            detailsBox.PackStart(rolaBox, false, false, 10);
        }

        Entry newTitleEntry = new Entry { Text = rolaDetails[0] };
        Entry newGenreEntry = new Entry { Text = rolaDetails[1] };
        Entry newTrackEntry = new Entry { Text = rolaDetails[2] };
        Entry performerEntry = new Entry { Text = rolaDetails[3] };
        Entry newYearEntry = new Entry { Text = rolaDetails[4] };
        Entry newAlbumEntry = new Entry { Text = rolaDetails[5] }; 

        Label title = new Label("New Title:");
        title.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(title, false, false, 5);
        detailsBox.PackStart(newTitleEntry, false, false, 5);

        Label genreLabel = new Label("New Genre:");
        genreLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(genreLabel, false, false, 5);
        detailsBox.PackStart(newGenreEntry, false, false, 5);

        Label trackLabel = new Label("New Track Number:");
        trackLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(trackLabel, false, false, 5);
        detailsBox.PackStart(newTrackEntry, false, false, 5);

        Label performerLabel = new Label("New Performer Name:");
        performerLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(performerLabel, false, false, 5);
        detailsBox.PackStart(performerEntry, false, false, 5);

        Label yearLabel = new Label("New Year:");
        yearLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(yearLabel, false, false, 5);
        detailsBox.PackStart(newYearEntry, false, false, 5);

        Label albumLabel = new Label("New Album Name:");
        albumLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(albumLabel, false, false, 5);
        detailsBox.PackStart(newAlbumEntry, false, false, 5);

        Button acceptButton = new Button("Accept");
        detailsBox.PackStart(acceptButton, false, false, 5);

        acceptButton.Clicked += (sender, eventArgs) =>
        {
            bool success = app.UpdateRolaDetails(
                rolaTitle,
                rolaPath,
                newTitleEntry.Text, 
                newGenreEntry.Text, 
                newTrackEntry.Text, 
                performerEntry.Text, 
                newYearEntry.Text, 
                newAlbumEntry.Text
            );

            if (!success)
            {   
                MessageDialog errorDialog = new MessageDialog(detailsWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Failed to update rola. Please check the input values (year and track must be int).");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
            }
            else{
                MessageDialog successDialog = new MessageDialog(detailsWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Rola updated successfully.");
                successDialog.Run();
                successDialog.Hide();
                successDialog.Dispose();

                detailsWindow.Hide();
                detailsWindow.Dispose();

                foreach (Widget child in rolasBox.Children)
                {
                    rolasBox.Remove(child);
                }
                ShowRolasWithCoverArt(); 
            }
        };
        scrolledWindow.Add(detailsBox);
        detailsWindow.Add(scrolledWindow);
        detailsWindow.ShowAll();
    }

    // ready
    void OnEditAlbumButton(object sender, EventArgs e)
    {
        Window editAlbum = new Window("Edit Album");
        editAlbum.SetDefaultSize(600, 100);
        editAlbum.SetPosition(WindowPosition.Center);
        editAlbum.StyleContext.AddProvider(cssProvider, 800);
        editAlbum.TransientFor = this;
        editAlbum.Modal = true;
        editAlbum.Resizable = false;

        Box vbox = new Box(Orientation.Vertical, 10);
        Label instructionLabel = new Label("Enter the album name:");
        instructionLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(instructionLabel, false, false, 5);
        Entry entry = new Entry();
        vbox.PackStart(entry, false, false, 5);
        entry.StyleContext.AddProvider(cssProvider, uint.MaxValue);

        Button confirm = new Button("Confirm");
        vbox.PackStart(confirm, false, false, 5);

        confirm.Clicked += (s, e) =>
        {
            string albumName = entry.Text;
            List<string> albumInfoList = app.GetAlbumDetailsWithOptions(albumName);
            if(albumInfoList.Count == 0)
            {
                MessageDialog errorDialog = new MessageDialog(editAlbum, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "No album found with that name. Please enter a valid name.");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
            }
            else if (albumInfoList.Count == 1)
            {
                editAlbum.Hide();
                editAlbum.Dispose();
                string[] albumInfoParts = albumInfoList.First().Split('\n');
                string albumPath = albumInfoParts.Last().Split(": ")[1];
                List<string> albumDetails = app.GetAlbumDetails(albumName, albumPath);
                ShowEditAlbumForm(albumName, albumPath, albumDetails);
            }
            else
            {
                editAlbum.Hide();
                editAlbum.Dispose();
                Window selectionWindow = new Window("Select Album");
                selectionWindow.SetDefaultSize(800, 600);
                selectionWindow.SetPosition(WindowPosition.Center);
                selectionWindow.StyleContext.AddProvider(cssProvider, 800);
                selectionWindow.TransientFor = this;
                selectionWindow.Modal = true;
                selectionWindow.Resizable = false;

                ScrolledWindow scrolledWindow = new ScrolledWindow{Vexpand = true, Hexpand = true};

                Box selectionVbox = new Box(Orientation.Vertical, 10);
                Label selectLabel = new Label("Select the Album to edit:");
                selectLabel.Xalign = 0.0f;
                selectLabel.StyleContext.AddClass("Child-label");
                selectionVbox.PackStart(selectLabel, false, false, 5);

                foreach (var albumInfo in albumInfoList)
                {
                    string[] albumInfoParts = albumInfo.Split('\n');
                    string extractedAlbumPath = albumInfoParts.Last().Split(": ")[1]; 
                    string albumNameExtracted = albumInfoParts[0].Split(": ")[1];
                    string albumYearExtracted = albumInfoParts[1].Split(": ")[1];

                    Button albumButton = new Button(albumInfo);
                    selectionVbox.PackStart(albumButton, false, false, 5);

                    albumButton.Clicked += (sender, args) => 
                    {
                        selectionWindow.Hide();
                        selectionWindow.Dispose();
                        List<string> albumDetails = app.GetAlbumDetails(albumName, extractedAlbumPath);
                        ShowEditAlbumForm(albumName, extractedAlbumPath, albumDetails);
                    };
                }
                scrolledWindow.Add(selectionVbox);
                selectionWindow.Add(scrolledWindow);
                selectionWindow.ShowAll();
            }
        };
        editAlbum.Add(vbox);
        editAlbum.ShowAll();
    }

    //almost ready
    void ShowEditAlbumForm(string albumName, string albumPath, List<string> albumDetails)
    {
        Window detailsWindow = new Window("Edit Album");
        detailsWindow.SetDefaultSize(300, 400);
        detailsWindow.SetPosition(WindowPosition.Center);
        detailsWindow.StyleContext.AddProvider(cssProvider, 800);
        detailsWindow.TransientFor = this;
        detailsWindow.Modal = true;
        detailsWindow.Resizable = false;

        Box detailsBox = new Box(Orientation.Vertical, 10);

        Label albumDetailsLabel = new Label($"Name: {albumDetails[0]}\nYear: {albumDetails[1]}\nPath: {albumPath}");
        albumDetailsLabel.StyleContext.AddClass("Child-label");
        albumDetailsLabel.Xalign = 0.0f;
        detailsBox.PackStart(albumDetailsLabel, false, false, 5);

        Label NewName = new Label("New Name:");
        NewName.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(NewName, false, false, 5);
        Entry newNameEntry = new Entry { Text = albumDetails[0] };
        detailsBox.PackStart(newNameEntry, false, false, 5);

        Label newYearLabel = new Label("New Year:");
        newYearLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(newYearLabel, false, false, 5);
        Entry newYearEntry = new Entry { Text = albumDetails[1] }; 
        detailsBox.PackStart(newYearEntry, false, false, 5);

        Button acceptButton = new Button("Accept");
        detailsBox.PackStart(acceptButton, false, false, 5);

        acceptButton.Clicked += (sender, eventArgs) =>
        {
            bool isUpdated = app.UpdateAlbumDetails(
            albumName,
            newNameEntry.Text,
            albumPath,
            newYearEntry.Text
            );

            if (isUpdated)
            {
                MessageDialog successDialog = new MessageDialog(detailsWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Album updated successfully.");
                successDialog.Run();
                successDialog.Hide();
                successDialog.Dispose();

                detailsWindow.Hide();
                detailsWindow.Dispose();
                //actualizar la vista 
            }
            else
            {
                MessageDialog errorDialog = new MessageDialog(detailsWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Failed to update album. Please check the input values (year must be an integer).");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
            }
        };
        detailsWindow.Add(detailsBox);
        detailsWindow.ShowAll();
    }

    // ready
    void OnDefinePerformerButton(object sender, EventArgs e)
    {
        Window definePerformer = new Window("Define Performer");
        definePerformer.SetDefaultSize(300, 200);
        definePerformer.SetPosition(WindowPosition.Center);
        definePerformer.StyleContext.AddProvider(cssProvider, 800);
        definePerformer.TransientFor = this;
        definePerformer.Modal = true;
        definePerformer.Resizable = false;

        Box vbox = new Box(Orientation.Vertical, 10);

        Label performerLabel = new Label("Enter performer name:");
        performerLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(performerLabel, false, false, 5);
        Entry performerEntry = new Entry();
        vbox.PackStart(performerEntry, false, false, 5);

        Label instructionLabel = new Label("Define performer as:");
        instructionLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(instructionLabel, false, false, 5);

        Button personButton = new Button("Person");
        vbox.PackStart(personButton, false, false, 5);

        Button groupButton = new Button("Group");
        vbox.PackStart(groupButton, false, false, 5);

        personButton.Clicked += (s, e) =>
        {
            string performerName = performerEntry.Text;
            string result = app.CheckPerformer(performerName, "Person");

            HandlePerformerResult(result, definePerformer, performerName, DefinePerson);
        };

        groupButton.Clicked += (s, e) =>
        {
            string performerName = performerEntry.Text;
            string result = app.CheckPerformer(performerName, "Group");

            HandlePerformerResult(result, definePerformer, performerName, DefineGroup);
        };

        definePerformer.Add(vbox);
        definePerformer.ShowAll();
    }

    // ready
    void HandlePerformerResult(string result, Window definePerformer, string performerName, Action<string> defineAction)
    {
        if (result == "NotFound")
        {
            MessageDialog errorDialog = new MessageDialog(definePerformer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please enter a valid performer name.");
            errorDialog.Run();
            errorDialog.Hide();
            errorDialog.Dispose();
        }
        else if (result == "Redefine")
        {
            MessageDialog redefineDialog = new MessageDialog(definePerformer, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "This performer is already defined. Do you want to redefine it?");
            ResponseType response = (ResponseType)redefineDialog.Run();
            redefineDialog.Hide();
            redefineDialog.Dispose();

            if (response == ResponseType.Yes)
            {
                definePerformer.Hide();
                definePerformer.Dispose();
                defineAction(performerName);
            }
        }
        else if (result == "Person" || result == "Group")
        {
            MessageDialog errorDialog = new MessageDialog(definePerformer, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, $"This performer is already defined as {result}");
            errorDialog.Run();
            errorDialog.Hide();
            errorDialog.Dispose();
        }
        else
        {
            definePerformer.Hide();
            definePerformer.Dispose();
            defineAction(performerName);
        }
    }

    // ready
    void DefinePerson(string performerName)
    {
        Window personWindow = new Window("Define Person");
        personWindow.SetDefaultSize(300, 400);
        personWindow.SetPosition(WindowPosition.Center);
        personWindow.StyleContext.AddProvider(cssProvider, 800);
        personWindow.TransientFor = this;
        personWindow.Modal = true;
        personWindow.Resizable = false;

        Box detailsBox = new Box(Orientation.Vertical, 10);

        Label stageNameLabel = new Label("Stage Name:");
        stageNameLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(stageNameLabel, false, false, 5);
        Entry stageNameEntry = new Entry {Text = performerName, Sensitive = false};
        detailsBox.PackStart(stageNameEntry, false, false, 5);

        Label realNameLabel = new Label("Real Name:");
        realNameLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(realNameLabel, false, false, 5);
        Entry realNameEntry = new Entry();
        detailsBox.PackStart(realNameEntry, false, false, 5);

        Label birthDateLabel = new Label("Birth Date:");
        birthDateLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(birthDateLabel, false, false, 5);
        Entry birthDateEntry = new Entry();
        detailsBox.PackStart(birthDateEntry, false, false, 5);

        Label deathDateLabel = new Label("Death Date:");
        deathDateLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(deathDateLabel, false, false, 5);
        Entry deathDateEntry = new Entry();
        detailsBox.PackStart(deathDateEntry, false, false, 5);

        List<string> performerDetails = app.ShowPerformerDetails(performerName);
        if (performerDetails.Count > 0)
        {
            realNameEntry.Text = performerDetails[1];
            birthDateEntry.Text = performerDetails[2];
            deathDateEntry.Text = performerDetails[3];
        }

        Button confirmButton = new Button("Confirm");
        detailsBox.PackStart(confirmButton, false, false, 5);
        
        confirmButton.Clicked += (s, e) =>
        {
            string stageName = stageNameEntry.Text;
            string realName = realNameEntry.Text;
            string birthDate = birthDateEntry.Text;
            string deathDate = deathDateEntry.Text;
            app.DefinePerformerAsPerson(performerName, stageName, realName, birthDate, deathDate);
            MessageDialog successDialog = new MessageDialog(personWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Performer defined as person.");
            successDialog.Run();
            successDialog.Hide();
            successDialog.Dispose();
            personWindow.Hide();
            personWindow.Dispose();
        };
        personWindow.Add(detailsBox);
        personWindow.ShowAll();
    }

    // ready
    void DefineGroup(string performerName)
    {
        Window groupWindow = new Window("Define Group");
        groupWindow.SetDefaultSize(300, 400);
        groupWindow.SetPosition(WindowPosition.Center);
        groupWindow.StyleContext.AddProvider(cssProvider, 800);
        groupWindow.TransientFor = this;
        groupWindow.Modal = true;
        groupWindow.Resizable = false;

        Box detailsBox = new Box(Orientation.Vertical, 10);

        Label groupNameLabel = new Label("Group Name:");
        groupNameLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(groupNameLabel, false, false, 5);
        Entry groupNameEntry = new Entry
        {
            Text = performerName,
            Sensitive = false
        };
        detailsBox.PackStart(groupNameEntry, false, false, 5);

        Label startDateLabel = new Label("Start Date:");
        startDateLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(startDateLabel, false, false, 5);
        Entry startDateEntry = new Entry();
        detailsBox.PackStart(startDateEntry, false, false, 5);

        Label endDateLabel = new Label("End Date:");
        endDateLabel.StyleContext.AddClass("Child-label");
        detailsBox.PackStart(endDateLabel, false, false, 5);
        Entry endDateEntry = new Entry();
        detailsBox.PackStart(endDateEntry, false, false, 5);

        List<string> performerDetails = app.ShowPerformerDetails(performerName);
        if (performerDetails.Count > 0)
        {
            startDateEntry.Text = performerDetails[1];
            endDateEntry.Text = performerDetails[2];
        }

        Button confirmButton = new Button("Confirm");
        detailsBox.PackStart(confirmButton, false, false, 5);

        confirmButton.Clicked += (s, e) =>
        {
            string groupName = groupNameEntry.Text;
            string startDate = startDateEntry.Text;
            string endDate = endDateEntry.Text;
                app.DefinePerformerAsGroup(performerName, groupName, startDate, endDate);
                MessageDialog successDialog = new MessageDialog(groupWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Performer defined as group.");
                successDialog.Run();
                successDialog.Hide();
                successDialog.Dispose();
                groupWindow.Hide();
                groupWindow.Dispose();
        };
        groupWindow.Add(detailsBox);
        groupWindow.ShowAll();
    }

    // ready
    void OnAddPersonGroup(object sender, EventArgs e)
    {
        Window addPersonGroup = new Window("Add person in a group");
        addPersonGroup.SetDefaultSize(300, 200);
        addPersonGroup.SetPosition(WindowPosition.Center);
        addPersonGroup.StyleContext.AddProvider(cssProvider, 800);
        addPersonGroup.TransientFor = this;
        addPersonGroup.Modal = true;
        addPersonGroup.Resizable = false;

        Box vbox = new Box(Orientation.Vertical, 10);

        Label personLabel = new Label("Enter person name:");
        personLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(personLabel, false, false, 5);

        Entry personEntry = new Entry();
        vbox.PackStart(personEntry, false, false, 5);

        Label groupLabel = new Label("Enter group name:");
        groupLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(groupLabel, false, false, 5);

        Entry groupEntry = new Entry();
        vbox.PackStart(groupEntry, false, false, 5);

        Button confirmButton = new Button("Confirm");
        vbox.PackStart(confirmButton, false, false, 5);

        confirmButton.Clicked += (s, e) =>
        {
            string result = app.CheckPersonAndGroup(personEntry.Text, groupEntry.Text);
            HandleResult(result, addPersonGroup, personEntry.Text, groupEntry.Text);
        };
        addPersonGroup.Add(vbox);
        addPersonGroup.ShowAll();
    }

    //left show group where the person belongs
    void HandleResult(string result, Window addPersonGroup, string personName, string groupName)
    {
        if (result == "Person not found")
        {
            MessageDialog errorPerson = new MessageDialog(addPersonGroup, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please enter a valid person name.");
            errorPerson.Run();
            errorPerson.Hide();
            errorPerson.Dispose();
        }
        else if (result == "Group not found")
        {
            MessageDialog errorGroup = new MessageDialog(addPersonGroup, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please enter a valid group name.");
            errorGroup.Run();
            errorGroup.Hide();
            errorGroup.Dispose();
        }
        else if (result == "Person already in group")
        {
            MessageDialog alreadyInGroupDialog = new MessageDialog(addPersonGroup, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "This person is already in the group.");
            alreadyInGroupDialog.Run();
            alreadyInGroupDialog.Hide();
            alreadyInGroupDialog.Dispose();
        }
        else
        {
            app.AddPersonToGroup(personName, groupName);
            MessageDialog success = new MessageDialog(addPersonGroup, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Person added to a group");
            success.Run();
            success.Hide();
            success.Dispose();
            addPersonGroup.Hide();
            addPersonGroup.Dispose();
        }
    }
        
    void OnSearchButton(object sender, EventArgs e)
    {
        Window searchWindow = new Window("Search");
        searchWindow.SetDefaultSize(300, 200);
        searchWindow.SetPosition(WindowPosition.Center);
        searchWindow.StyleContext.AddProvider(cssProvider, 800);
        searchWindow.TransientFor = this;
        searchWindow.Modal = true;
        searchWindow.Resizable = false;

        Box vbox = new Box(Orientation.Vertical, 10);

        Label searchLabel = new Label("Enter the query to search (e.g., |Album:\"name\" ^ Performer:\"name\" ^ Title:\"song\" ^ InTitle:\"word\"):");
        searchLabel.StyleContext.AddClass("Child-label");
        vbox.PackStart(searchLabel, false, false, 5);

        Entry userEntry = new Entry();
        vbox.PackStart(userEntry, false, false, 5);

        Button confirmButton = new Button("Confirm");
        vbox.PackStart(confirmButton, false, false, 5);

        confirmButton.Clicked += (s, e) =>
        {
            string userQuery = userEntry.Text;
            if (string.IsNullOrEmpty(userQuery) || !app.IsQueryValid(userQuery))
            {
                MessageDialog errorDialog = new MessageDialog(searchWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Invalid query format. Please enter a valid query.");
                errorDialog.Run();
                errorDialog.Hide();
                errorDialog.Dispose();
                return;
            }

            List<string> rolaResults = app.SearchRolas(userQuery);

            Window resultsWindow = new Window("Search Results");
            resultsWindow.SetDefaultSize(400, 400);
            resultsWindow.SetPosition(WindowPosition.Center);
            resultsWindow.StyleContext.AddProvider(cssProvider, 800);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            Box resultsBox = new Box(Orientation.Vertical, 10);

            if (rolaResults.Count == 0)
            {
                Label noResultsLabel = new Label("No results found.");
                noResultsLabel.StyleContext.AddClass("Child-label");
                resultsBox.PackStart(noResultsLabel, false, false, 5);
            }
            else
            {
                foreach (string rolaInfo in rolaResults)
                {
                    Label rolaLabel = new Label(rolaInfo);
                    rolaLabel.Xalign = 0.0f;
                    rolaLabel.Wrap = true;
                    rolaLabel.StyleContext.AddClass("Child-label");
                    resultsBox.PackStart(rolaLabel, false, false, 5);
                }
            }
            scrolledWindow.Add(resultsBox);
            resultsWindow.Add(scrolledWindow);
            resultsWindow.ShowAll();
            searchWindow.Hide();
            searchWindow.Dispose();
        };
        searchWindow.Add(vbox);
        searchWindow.ShowAll();
    }


    void DisplayResults(Window searchWindow, List<string> results, string successMessage, string failureMessage)
    {
        if (results.Count > 0)
        {
            string resultText = successMessage + "\n" + string.Join("\n", results);
            MessageDialog resultsDialog = new MessageDialog(searchWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, "{0}", resultText);
            resultsDialog.Run();
            resultsDialog.Hide();
        }
        else
        {
            MessageDialog noResultsDialog = new MessageDialog(searchWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, failureMessage);
            noResultsDialog.Run();
            noResultsDialog.Hide();
        }
    }
        
    private void DisableNonMiningActions()
    {
        editRolaButton.Sensitive = false;
        editAlbumButton.Sensitive = false;
        definePerformerButton.Sensitive = false;
        addPersonButton.Sensitive = false;
        searchButton.Sensitive = false;
    }

    private void EnableNonMiningActions()
    {
        editRolaButton.Sensitive = true;
        editAlbumButton.Sensitive = true;
        definePerformerButton.Sensitive = true;
        addPersonButton.Sensitive = true;
        searchButton.Sensitive = true;
    }

    public static void Main()
    {
        Application.Init();
        new GraphicInterface();
        Application.Run();
    }
}