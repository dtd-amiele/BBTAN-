using System;
using System.Collections.Generic;
using SplashKitSDK;

public class Player
{
    private Bitmap _playerBitmap; // bitmap of player
    private const int SPEED = 7; 
    private double _angle; //angle between player and mouse
    public double X { get; private set;} // X position of player
    public double Y { get; private set;} // Y position of player
    public bool Quit {get; private set;} // whether player want to quit

    public bool Shoot{get;set;} = true; // whether player allow to shoot (shoot is true when all bullets are back to player)
    
    // Width of Bitmap
    public int Width 
    {
        get
        {
            return _playerBitmap.Width;
        }
    }
    //Height of Bitmap
    public int Height
    {
        get
        {
            return _playerBitmap.Height;
        }
    }
    //Constuctor of Player 
    //with gameWindow and Bullet pass as parameter
    public Player(Window gameWindow, List<Bullet> Bullets)
    {
        _playerBitmap = SplashKit.BitmapNamed("Pegasi");
        X = (gameWindow.Width - Width)/2;
        Y = (gameWindow.Height - Height);
        Bullet bullet = new Bullet(X + (Width /2),Y+(Height/2));
        Bullets.Add(bullet);
        _angle = 0;

        Quit = false;        
    }

    // Change position based on the first bullet touch the bottom of Window
    public void ChangePosition(double x)
    {
        X = x;
        _angle=0;
    }

    //handle input of player
    public void HandleInput(List<Bullet> Bullets)
    {
        
        // Shoot when player clicking the left-mouse
        if (SplashKit.MouseClicked(MouseButton.LeftButton) && (Shoot)) 
        {
            ChangeVelocity(Bullets,SplashKit.MouseX(),SplashKit.MouseY());
            Shoot = false;
            
        }
        if (SplashKit.KeyDown(KeyCode.EscapeKey)) Quit = true;

    }
    //Change velocity of bullet based on the player position and the mouse position
    private void ChangeVelocity(List<Bullet> Bullets,double mouseX,double mouseY)
    {
        double angle;
        // get the point for player
        Point2D fromPt = new Point2D()
        {
            X = X + Width /2 , Y = Y + Height / 2
        };

        // get the point for mouse
        Point2D toPt = new Point2D()
        {
            X = mouseX, Y = mouseY
        };

        // Calculate the angle between player and mouse
        angle = SplashKit.PointPointAngle(fromPt,toPt);
        // Change angle of the player
        _angle = angle+90;
        Vector2D tempVelocity = new Vector2D();
        Matrix2D rotation = SplashKit.RotationMatrix(angle);
        tempVelocity.X = Bullets[0].SPEED;
        tempVelocity = SplashKit.MatrixMultiply(rotation,tempVelocity);
        //UpdateVelocity for each Bullet
        for (int i=0; i< Bullets.Count; i++)
        {
            Bullets[i].UpdateVelocity(tempVelocity,i);
        }
    }
    //Draw the player
    public void Draw()
    {
        DrawingOptions options = SplashKit.OptionRotateBmp(_angle);
        SplashKit.DrawBitmap(_playerBitmap, X, Y,options);
    }
}