using System;
using SplashKitSDK;

public class Bullet
{
    private double _x, _y; // X and y position of bullet
    private Bitmap _bulletBitmap; // bitmap of bullet
    private Vector2D Velocity; // velocity of bullet
    //private const int RADIUS = 10;
    public int RADIUS =10; 
    public int SPEED{get;} =6; // SPEED of bullet
    public double X 
    {
        get
        {
            return _x;
        }
    }
    public double Y
    {
        get
        {
            return _y;
        }
    }
    // Width of bullet bitmap
    public double Width
    {
        get
        {
            return _bulletBitmap.Width;
        }
    }
    //Height of bullet bitmap
    public double Height
    {
        get
        {
            return _bulletBitmap.Height;
        }
    }

    // constructor of Bullet 
    public Bullet(double x, double y)
    {
        _bulletBitmap = SplashKit.BitmapNamed("MetalBullet");
        _x = x- (Width/2);
        _y = y -Height;
        Velocity = new Vector2D();

    }
    
    // Bullet position back to player position
    public void BackToPlayer(Player player)
    {
        _x = player.X + (player.Width /2)-(Width/2);
        _y = player.Y + (player.Height/2)-Height;
        Velocity.X = 0;
        Velocity.Y = 0;
    }

    // update velocity of bullet with different num for the ability to see different bullets
    public void UpdateVelocity(Vector2D tempVelocity, int num)
    {
        Velocity = tempVelocity;   

        _x += 0.75*num*Velocity.X;  
        _y += 0.75*num*Velocity.Y;  
    }
    //Update position of bullet
    public void Update()
    {
        
        _x += Velocity.X;
        _y += Velocity.Y;

    }

    //return whether bullet collide with box
    public bool CollideWith(Box box)
    {
        return _bulletBitmap.CircleCollision(X,Y, box.CollisionCircle);
    }

    //Bouncing when touch Top or bottom edge
    public void TopBottomEdge()
    {
        Velocity.Y = Velocity.Y * -1;
    }
    //Bouncing when touch left or right edge
    public void LeftRightEdge()
    {
        Velocity.X = Velocity.X * -1;
    }
    //Draw bullet
    public void Draw()
    {

        SplashKit.DrawBitmap(_bulletBitmap, _x, _y);

    }
}