using System;
using SplashKitSDK;

public class Program
{
    // Load resources using for game
    public static void LoadResources()
    {
        SplashKit.LoadBitmap("player", "Player.png");
        SplashKit.LoadBitmap("Pegasi", "Pegasi.png");
        SplashKit.LoadBitmap("Gliese", "Gliese.png");
        SplashKit.LoadBitmap("Aquarii", "Aquarii.png");
        SplashKit.LoadBitmap("MetalBullet", "MetalBullet.png");
        SplashKit.LoadFont("Font1","font.ttf");
        SplashKit.LoadSoundEffect("HitSound","hit.wav");
    }

    public static void Main()
    {
        LoadResources();
        // Declare the database and query
        Database _myDB;
        QueryResult query;
        _myDB = SplashKit.OpenDatabase("ScoreAAADatabase","ScoreAAADatabase");
        _myDB.RunSql("CREATE TABLE HighScore (NAME STRING PRIMARY KEY, SCORE INTEGER);");
        
        // Prompt to take Name of Player
        // If Name is already existed, ask whether the player is the old player to resume the High Score 
        // of previous time
        // If Name is not existed, ask new field to database with the highScore = 0
        string name= "";
        int preHighScore = 0;
        while (name=="")
        {
            Console.Write("Please Type your name: ");
            string tempName = Console.ReadLine();
            tempName = tempName.ToLower();
            query= _myDB.RunSql("SELECT * FROM HighScore WHERE NAME='"+tempName+"';");

            if (SplashKit.HasRow(query))
            {

                Console.Write("Name is already existed. Is it you? (Please type 1 for yes and 2 for no: ");
                int ans = -1;
                while (ans==-1)
                {
                    //string temp = Console.ReadLine();
                    try
                    {
                       ans = Convert.ToInt32(Console.ReadLine()); 
                       if ((ans==1) || (ans==2) )
                        {
                            if (ans ==2) break;
                            Console.WriteLine("Continue your previous game");
                            preHighScore = SplashKit.QueryColumnForInt(query,1);
                            name = tempName;
                        }
                        else ans=-1;
                    }
                    catch 
                    {
                    Console.WriteLine("Please type valid number");
                    ans= -1;
                    }
                }

            }     
            else
            { 
                name = tempName;
                _myDB.RunSql("INSERT INTO HighScore VALUES('"+name+"',0)");
            }
         
        }
        // When recieving the name from player, start the game
        if (name != "")
        {
        // Declare Window and game
        // Create the new object of game, pass the Window, the database, the name of player 
        //and the highscore (0 for new player or highscore from database for old player)
            Window gameWindow = new Window("AAA", 400,600);
            AAA game = new AAA(gameWindow,_myDB, name,preHighScore);
            while (( !gameWindow.CloseRequested) && (!game.Quit))
            {
                SplashKit.ProcessEvents();
                game.HandleInput();
                // Check the whether player is lose -> draw lose screen
                if (game.Lose)
                {   
                    //Check whether player want to draw the LeaderBoard
                    if (!game.LeaderBoardPrint) game.DrawLose();
                    else 
                    {
                        game.DrawLeaderBoard();
                    }
                }
                else
                {
                    game.Update();
                    game.Draw(); 
                }
            }
            SplashKit.FreeAllDatabases();
            SplashKit.FreeAllQueryResults();
        }

    }
}
