using SplashKitSDK;
using System;
using System.Collections.Generic;

public class AAA
{
    private Player _player; // the player
    private string _playerName; // the player's name
    private Window _gameWindow; // the Window
    private List<Bullet> _Bullets; // List of Bullets( Balls)
    private List<Box> _Boxes; // List of Boxes
    private int newBullet= 0; // Number of NumBullet for each Round ( number of Plusy boxes are disappear)
    private double firstBallX=-1; // The X position of first ball touch the bottom of the window -> move the position of the ship
    private const int topBorder = 47; // Border to draw Box
    public int HighScore{get; private set;} //HighScore of the game
    public int Score {get; private set;} // Score of the current game
    public bool Lose{get; private set;} // Check whether player is lose
    public bool LeaderBoardPrint{get; private set;} // check whether Leader Board need to printed
    private int count = 0;// Count the number of Bullets touch the bottom of window for each round 
    private Font _font;// font used to draw text
    private SoundEffect _soundHit; // sound when the bullet/ball hit the box
    private Database _myDB; // the database
    public bool Quit // return the Quit of player to check whether player want to quit game
    {
        get
        {
            return _player.Quit;
        }
    }

    //Constructor of AAA
    // receive the window, database, nam ofplayer and highscore as the parameters
    public AAA(Window gameWindow,Database mydb,string name, int preHighScore)
    {
        _Bullets = new List<Bullet>();
        _Boxes = new List<Box>();
        _gameWindow = gameWindow;
        _player = new Player(_gameWindow, _Bullets);
        _font = SplashKit.FontNamed("Font1");
        _soundHit = SplashKit.SoundEffectNamed("HitSound");
        HighScore = preHighScore;
        Score = 0;
        Lose = false;
        LeaderBoardPrint = false;
        _myDB = mydb;
        _playerName = name;
        GenerateBox();
    }
    //Generate new row of boxes
    // Using random to choose the number of box 
    // Using random to create Plusy box ( box used for get more bullets) or normal Boxy box
    public void GenerateBox()
    {
        double temp = _gameWindow.Width/50;
        int maxBox = Convert.ToInt32(Math.Floor(temp));
        int numBox = SplashKit.Rnd(1, maxBox);
        int minPos = 0;
        int maxPos = maxBox - numBox ; 
        
        for (int i = 1; i <= numBox; i++)
        {
            int pos;
            if (maxPos == minPos)
                pos = maxPos;
            else pos = SplashKit.Rnd(minPos,maxPos);
            if (minPos<=pos) minPos = pos+1;
            maxPos += 1;
            Box box;
            if ((SplashKit.Rnd()<0.2) && (_Bullets.Count <= 30)) 
            {
                box = new Plusy(_gameWindow, pos * 50,50,1);
            }
            else
            { 
                if (_Bullets.Count==1)
                    box = new Boxy(_gameWindow,pos * 50,50, 1);
                else 
                    box = new Boxy(_gameWindow,pos * 50,50, SplashKit.Rnd(1,_Bullets.Count));
            }
            _Boxes.Add(box);
        }
    }

    // Handle Input 
    // Call for Player Hanlde Input
    // Press 2 to print the LeaderBoard 
    // Press 1 to restrat fame
    public void HandleInput()
    {
        _player.HandleInput(_Bullets);
        if ((SplashKit.KeyDown(KeyCode.Num2Key) || SplashKit.KeyDown(KeyCode.Keypad2)) && (Lose)) 
        {           
            LeaderBoardPrint = true; 
        }
        if ((SplashKit.KeyDown(KeyCode.Num1Key) || SplashKit.KeyDown(KeyCode.Keypad1)) && (Lose)) 
        {
            RestartGame();
           // Console.WriteLine("game Start");
        }
    }

    // Update game
    // Update the bullets and call check collision
    public void Update()
    {
        for (int i=0; i<_Bullets.Count;i++)
            _Bullets[i].Update();
        CheckCollisions();
    }

    // Increase score for each round
    public void GetScore()
    {
        Score ++;
    }

    // Check collision
    

    public void CheckCollisions()
    {
        // Whether bullet touch any obstacles then bouncing; if bullet touch the bottom of window, it will disappear and return back to player for next round
        for (int i = 0 ; i < _Bullets.Count; i++)
        {
            if (_Bullets[i].Y + _Bullets[i].Height >= _gameWindow.Height)
            {
                if (firstBallX == -1)
                {
                    firstBallX = _Bullets[i].X + (_Bullets[i].Width/2)- (_player.Width/2);
                    _player.ChangePosition(firstBallX);
                }
                _Bullets[i].BackToPlayer(_player);
                count += 1;
            }
            if (_Bullets[i].Y <= topBorder)
                _Bullets[i].TopBottomEdge();
            if (_Bullets[i].X <= 0)
                _Bullets[i].LeftRightEdge();
            if (_Bullets[i].X + _Bullets[i].Width >= _gameWindow.Width)
                _Bullets[i].LeftRightEdge();
        }
       // whether Bullet touch any Box to remove Box and bouncing the bullet
       //increase extra bullets(if touch Plusy box)
        for (int i=0; i<_Bullets.Count; i++)
        {
            List<Box> _removeBoxes = new List<Box>();
            for (int j=0; j<_Boxes.Count;j++)
            {
                if ((_Bullets[i].CollideWith(_Boxes[j])) && (!_player.Shoot))
                {
                    _Boxes[j].Lost();
                    _soundHit.Play();
                    if (_Boxes[j].Value == 0)
                        _removeBoxes.Add(_Boxes[j]);
                    if (_Boxes[j] is Boxy)
                    {
                    if (_Bullets[i].Y >= _Boxes[j].Y)
                        _Bullets[i].TopBottomEdge();
                    if (_Bullets[i].Y <= _Boxes[j].Y)
                        _Bullets[i].TopBottomEdge();
                    if (_Bullets[i].X <= _Boxes[j].X)
                        _Bullets[i].LeftRightEdge();
                    if (_Bullets[i].X >= _Boxes[j].X)
                        _Bullets[i].LeftRightEdge();
                    }
                }
            }
            // Remove Box and count the number of extra bullets
            for (int j = 0; j < _removeBoxes.Count; j++)
            {
                _Boxes.Remove(_removeBoxes[j]);
                if (_removeBoxes[j] is Plusy) 
                {
                    //Console.WriteLine("Plus");
                    newBullet++;
                }
            }
        }
        // Next round start when all the shooting bullet go back to the player
        // If any boxy box touch the bottom of Window, player is lose
        // ext round starts by add one row of boxes and add 1 point for score
        if (count == _Bullets.Count) 
        {
            //Console.WriteLine("Shoot turn true");
            _player.Shoot = true;
            firstBallX = -1;
            for (int i =0; i < _Boxes.Count; i++)
            {
                _Boxes[i].Update();
                if ((_Boxes[i] is Boxy)&&(_Boxes[i].Y>=_gameWindow.Height)) Lose = true;
            }
            GetScore();
            GenerateBox();
            count = 0;
            for (int i =0; i< newBullet;i++)
            {
                Bullet bullet = new Bullet(_player.X + (_player.Width /2),_player.Y+(_player.Height/2)); 
                _Bullets.Add(bullet);  

            }
            newBullet = 0;
        }

    }
    
    //Restart player, bullet, robot
    // Assign Lose and LeaderBoardPrint back to false
    private void RestartGame()
    {
        _Bullets = new List<Bullet>();
        _player = new Player(_gameWindow,_Bullets);
        Score = 0;
        _Boxes = new List<Box>();
        GenerateBox();
        Lose = false;
        LeaderBoardPrint =false;
    }
    //Draw the game
    public void Draw()
    {

        _gameWindow.Clear(Color.Black);
        SplashKit.DrawText(Convert.ToString(Score), Color.White,_font, 25 , _gameWindow.Width/2 -  Convert.ToString(Score).Length , 5);
        SplashKit.FillRectangle(Color.White,0, topBorder-5, _gameWindow.Width,5);  
        for (int i = 0;  i < _Bullets.Count; i++)
        {
            _Bullets[i].Draw();      
      
        }
        SplashKit.DrawText("Bullet: "+ Convert.ToString(_Bullets.Count), Color.White, _font, 17, 0,10);
        string strHighScore =  "Top: "+ Convert.ToString(HighScore);
        SplashKit.DrawText(strHighScore, Color.White, _font, 17, _gameWindow.Width-13*strHighScore.Length ,10);
        for (int i = 0;  i < _Boxes.Count; i++)
        {
            _Boxes[i].Draw();            
        }
        _player.Draw();
        _gameWindow.Refresh(60);

    }

    // Draw lose screen
    public void DrawLose()
    {
        if (Score > HighScore) 
        {
            HighScore = Score;
            // Update database if highScore is changed
            _myDB.RunSql("UPDATE HighScore SET SCORE = "+HighScore+" WHERE NAME = '"+_playerName+"';");
        }

        _gameWindow.Clear(Color.Black);
        string strScore = "Score: " +Convert.ToString(Score);
        string strHighScore = "High Score: " +Convert.ToString(HighScore);
        string strOption = "Press Esc to Quit";
        string strOption1 = "Press 1 to Restart";
        string strOption2 = "Press 2: Leader Board";
        SplashKit.DrawText(strScore, Color.White,_font, 25 ,  (_gameWindow.Width/2)- 6*strScore.Length,_gameWindow.Height/2 -150 );
        SplashKit.DrawText(strHighScore, Color.White,_font, 25 ,  (_gameWindow.Width/2)- 6*strHighScore.Length,_gameWindow.Height/2 -60 );
        SplashKit.DrawText(strOption, Color.White, _font, 25 , (_gameWindow.Width/2) - 7*strOption.Length,_gameWindow.Height/2 +30);
        SplashKit.DrawText(strOption1, Color.White, _font, 25 , (_gameWindow.Width/2) - 7*strOption1.Length,_gameWindow.Height/2 +120);
        SplashKit.DrawText(strOption2, Color.White, _font, 25 , (_gameWindow.Width/2) - 7*strOption2.Length,_gameWindow.Height/2 +210);
        _gameWindow.Refresh(60);
        
    }  
    // Draw LeaderBoard with top ten player with highest scores 
    public void DrawLeaderBoard()
    {
        QueryResult query;
        //create array for names of all player, maximum 1000
        string[] nameArr = new string[1000]; 
        int[] scoreArr = new int[1000];
        //the number of players score in database
        int count = 0;

        //run query select all in highscore database
        query= _myDB.RunSql("SELECT * FROM HighScore;");
        // If query is valid the assign values of query to nameArr and scoreArr and come to next query
        // if numberof query is greater than 1000, print notification as the maximum array is 1000
        while (SplashKit.HasRow(query))
        {
            nameArr[count]=SplashKit.QueryColumnForString(query,0);
            scoreArr[count]=SplashKit.QueryColumnForInt(query,1);
            if (count==1000) 
                {
                    Console.WriteLine("The database is already exceed max value for 1000 member. Your information may not be updated in the database");
                    break;
                } 
            count++;
            if (!query.GetNextRow()) 
                break;
        }
        // Sort the array with bigger numbers of score go first
        // loop using the normal algorithm with 2 for - run through all the pairs to compare and swap
        for (int i=0; i<count; i++)
            for (int j =i+1; j<count; j++)
                if (scoreArr[i]<scoreArr[j])
                {
                    int temp = scoreArr[i];
                    scoreArr[i]= scoreArr[j];
                    scoreArr[j]= temp;
                    string tempst = nameArr[i];
                    nameArr[i]= nameArr[j];
                    nameArr[j]= tempst;
                }
        
        int maxI=10; //maximum for leaderboard (top 10)
        if (count<maxI) maxI = count; // if the number of player is les than 10, take the number of player instead
        
        //Draw the leaderboard
        _gameWindow.Clear(Color.Black);
        SplashKit.DrawText("Press 1 to Restart the game", Color.White,_font, 15 , 30, 20);
        SplashKit.DrawText("Press ESC to Quit", Color.White,_font, 15 , 30, 40);
        SplashKit.DrawText("LEADER BOARD", Color.Red,_font, 20 , 120, 70);

        int posY = 100;
        for (int i=0; i< count; i++)
        {
            SplashKit.DrawText(nameArr[i], Color.White,_font, 15 , 50, posY);
            SplashKit.DrawText(Convert.ToString(scoreArr[i]), Color.White,_font, 15 , 300, posY);
            posY += 50;
        }
        _gameWindow.Refresh(60);

    }
    
}