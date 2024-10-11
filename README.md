Music Library Editor Project

Elaborated by: Alan Kevin Cano Tenorio
Account number: 321259967

This project is a music Library editor with a graphical user interface (GUI) where users can view information about songs (title, artist, album, cover, etc.) and perform queries on the database using an easy-to-use query language. The music data is mined from MP3 files with ID3v2.4 tags.

Project Requirements:
-.NET 8 SDK: Make sure you have the latest version of the .NET 8 SDK installed. You can download it from here.
-GTK# for .NET: This project uses GTK# for the graphical user interface. You may need to install GTK# if it's not already available on your platform.
-SQLite: SQLite is used as the database engine. Ensure you have the SQLite library and Microsoft.Data.Sqlite package installed.
-TagLibSharp: This library is used to extract ID3v2.4 tags from MP3 files.

How to Compile
Open a terminal and navigate to the root directory of the project.

Run the following command to restore dependencies and compile the project:

dotnet build

This will compile the entire project and generate the binaries.

How to Run the Application
After compiling, you can run the miner or databasae application using the following command:

miner:
dotnet run --project ./MinerApp/MinerApp.csproj

database:
dotnet run --project ./DataBaseApp/DataBaseApp.csproj

This will launch the graphical interface where you can interact with the music database.

Running Tests:
To run the tests, use the following command:

dotnet test

This will execute all the unit tests for the project and display the results in the terminal.

For detailed test: dotnet test --logger "console;verbosity=detailed"

Cleaning Up (Removing Build Artifacts)
To remove the compiled files and clear the bin and obj directories, use the following command:

rm -rf **/bin **/obj

This will delete all bin and obj directories recursively throughout the project.

To search in the program:
You may type:    |Title:"name of the song"|Performer:"name performer"|Album:"nombre del album"

in any order and with any parameter you want.
to search to any parameter combined: |Performer:"1"|Performer:"2"





NOTE:
The Project model is in the diagrams path