using Gtk;
using System;
using System.Collections.Generic;
using ApplicationApp;  

class SharpApp : Window
{
    public Controller app = new Controller();  

    private VBox vbox;

    public SharpApp() : base("Music Library Mining")
    {
        SetDefaultSize(400, 300);
        SetPosition(WindowPosition.Center);
        DeleteEvent += delegate { Application.Quit(); };
        Fixed fix = new Fixed();
        Button mineButton = new Button("Start Mining");
        mineButton.Clicked += OnMineClick;
        mineButton.SetSizeRequest(100, 40);
        
        fix.Put(mineButton, 50, 50);
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
        List<string> titles = app.ShowRolasInPath();

        foreach (string title in titles)
        {
            Label titleLabel = new Label(title); 
            vbox.PackStart(titleLabel, false, false, 5);
        }
        
        vbox.ShowAll();  
    }

    public static void Main()
    {
        Application.Init();
        new SharpApp();
        Application.Run();
    }
}
