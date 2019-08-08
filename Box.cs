using System;
using System.Collections.Generic;
using SplashKitSDK;

public abstract class Box
{
    public int LENGTH{get; private set;} = 50 ; //length of the box
    public Color MainColor; //color to draw box
    public Window _gameWindow;    //gamewindow

    public int Value{get;private set;} // value inside box -> number of time need to hit to make box disappear
    public double X {get;set;} // X position of box
    public double Y{get;set;}//Y position of box
    // Circle Box
    public virtual Circle CollisionCircle
    {
        get
        {
            return SplashKit.CircleAt(X + (LENGTH/2),Y + (LENGTH /2),LENGTH/2);
        }
    }
    // COnstructor of box
    //pass the window, x, y position and the value inside the box
    public Box(Window gameWindow, int x, int y, int value )
    {
        X = x;
        Y = y;
        Value =value;
        _gameWindow = gameWindow;
        MainColor = Color.RandomRGB(200);
        while ((SplashKit.RedOf(MainColor)==0) && (SplashKit.GreenOf(MainColor)==0) && (SplashKit.BlueOf(MainColor)==0))
        {
            MainColor = Color.RandomRGB(200);
        }
    }
    // remove 1 point of value
    public  void Lost()
    {
        Value -= 1;
    }
    //Update position of box (move toward to bottom of Window)
    public void Update()
    {
        Y += LENGTH;
    }
    public abstract void Draw();    
    

}
//Class Boxy : inheritance by Box 
public class Boxy: Box
{

    public Boxy(Window gameWindow, int x, int y, int value) :base(gameWindow,x,y,value)
    {
        
    }    
    //Draw Boxy Box
    public override void Draw()
    {
        _gameWindow.DrawRectangle(MainColor, X+1, Y+1, LENGTH-2, LENGTH-2);
        _gameWindow.DrawRectangle(MainColor, X+2, Y+2, LENGTH-4, LENGTH-4);
        _gameWindow.DrawRectangle(MainColor, X+3, Y+3, LENGTH-6, LENGTH-6);
        _gameWindow.DrawRectangle(MainColor, X+4, Y+4, LENGTH-8, LENGTH-8);

        _gameWindow.DrawText(Convert.ToString(Value), MainColor, X+LENGTH/2-5, Y+LENGTH/2-5);
    }
}
//Class Plusy : inheritance by Box 
public class Plusy: Box
{
    private int radius = 10;    
    public Plusy(Window gameWindow, int x, int y, int value) :base(gameWindow,x,y,value)
    {

    }    
    //Circle the plusy box
    public override Circle CollisionCircle
    {
        get
        {
            return SplashKit.CircleAt(X + (LENGTH/2),Y + (LENGTH /2),radius);
        }
    }
    //Draw Plusy box
    public override void Draw()
    {
        _gameWindow.DrawCircle(Color.Green, X+ (LENGTH/2), Y + (LENGTH /2), radius);
        _gameWindow.DrawCircle(Color.Green, X+ (LENGTH/2), Y + (LENGTH /2), radius+1);
        _gameWindow.DrawCircle(Color.Green, X+ (LENGTH/2), Y + (LENGTH /2), radius+2);

        _gameWindow.FillRectangle(Color.Green,X+ (LENGTH/2)-5,Y + (LENGTH /2)-1,11,3 );
        _gameWindow.FillRectangle(Color.Green,X+ (LENGTH/2)-1,Y + (LENGTH /2)-5,3,11 );

    }
}