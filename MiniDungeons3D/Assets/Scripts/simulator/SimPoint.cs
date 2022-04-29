using Newtonsoft.Json.Linq;
using System;

public struct SimPoint{
	
  public static readonly SimPoint North = new SimPoint(0,-1);
  public static readonly SimPoint East = new SimPoint(1,0);
  public static readonly SimPoint South = new SimPoint(0,1);
  public static readonly SimPoint West = new SimPoint(-1,0);
  public static readonly SimPoint Zero = new SimPoint(0,0);

	public int X{get;set;}
	public int Y{get;set;}

	public SimPoint(int x, int y) : this(){
		X = x;
		Y = y;
	}

	public static SimPoint operator +(SimPoint p1, SimPoint p2){
      return new SimPoint(p1.X + p2.X, p1.Y + p2.Y);
  	}

  	public static SimPoint operator -(SimPoint p1, SimPoint p2){
      return new SimPoint(p1.X - p2.X, p1.Y - p2.Y);
  	}

    public static bool operator ==(SimPoint p1, SimPoint p2){
      return (p1.X == p2.X) && (p1.Y == p2.Y);
    }

    public static bool operator !=(SimPoint p1, SimPoint p2){
      return (p1.X != p2.X) || (p1.Y != p2.Y);
    }

    public override bool Equals(object obj){
      var other = (SimPoint) obj;
      return (this.X == other.X) && (this.Y == other.Y);
    }

    public override int GetHashCode()
    { 
        return X ^ Y;
    } 

    public override string ToString(){
      return "X: " + X + " Y: " + Y;
    }

	public float EuclidianDistance(SimPoint other){
		SimPoint difference = this - other;
		return (float)Math.Sqrt(Math.Pow(difference.X,2) + Math.Pow(difference.Y,2));
	}
}