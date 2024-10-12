Music Library Editor
Elaborated by: Alan Kevin Cano Tenorio
Account Number: 321259967

The Music Library Editor is a graphical application that allows users to interact with a music library. With this application, you can view information about songs (title, artist, album, cover art, etc.), perform complex queries on a database, and manage your music library. The music data is mined from MP3 files using ID3v2.4 tags.

Project Requirements
To run this project, ensure the following dependencies are installed:

1. .NET 8 SDK: Make sure you have the latest version of the .NET 8 SDK installed. Download here.
2. GTK# for .NET: This project uses GTK# for its graphical interface. Install GTK# if it's not already available on your system.
3. SQLite: The database engine used for managing the music library. Install the SQLite library and the Microsoft.Data.Sqlite NuGet package.
4. TagLibSharp: A library used to extract and manipulate ID3v2.4 tags from MP3 files.

How to Compile
From the root directory of the project (where the .sln solution file is located), open a terminal and run:

dotnet build

This will restore all dependencies and compile the project, generating the binaries.

How to Run the Application
Graphical Interface (GUI Mode)
To run the graphical user interface, use the following command:

dotnet run --project ./src/GraphicInterface/GraphicInterface.csproj

This will launch the graphical interface, where you can interact with the music database. Note: The interface may take a few seconds to update when performing certain actions.

Terminal Mode (Command Line Mode)
To run the program in console mode (note: this is an auxiliary program and not fully implemented):

dotnet run --project ./src/Controller/Controller.csproj

Running Tests
To execute the unit tests, specifically those for the database, use the following command:

dotnet test --logger "console;verbosity=detailed"

This will run all unit tests and provide detailed results in the terminal.

Main Features of the Graphical Interface
The Music Library Editor interface includes the following features:

1. Change Directory: Modify the directory where your music files are located.
2. Start Mining: Begin mining music files in the selected directory. (Note: It may take a few seconds for the screen to update.)
3. Edit Song (Rola): Update song information. (Note: It may take a few seconds for the screen to update.)
4. Edit Album: Modify album details. (Note: It may take a few seconds for the screen to update.)
5. Define Performer: Define a performer as a person or group.
6. Add Person to Group: Assign a person to a group.
7. Search: Perform queries to find specific songs, albums, or performers.
8. Mining Log: View the log to see the results of the mining process.

Search Queries
You can perform searches using the following query structure:

Search by Title:
Title:"song name"

Add more filters:
You can combine multiple search filters like this:
Title:"song name" ^ Performer:"artist name" | Album:"album name"

Search by Performer (only performer name is allowed):
Performer:"artist name"
and will appear performers with that name, also, their rolas and their group or members.

Search by Album (only album name is allowed):
Album:"album name"
and will appear albums with that name and their rolas

Note:
In search function, the results appears like simple text, i was out of time to display it better.
I cant remade the diagrams because i was out of time, but i tried to follow it.
The actual code and the class diagram have a lot of diferences.