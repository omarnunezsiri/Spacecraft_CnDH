// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;
#region All Classes
#region Coordinate Class
/// <summary>
/// The coordinate class contains data for the x, y and z coordinates of the spaceship.
/// </summary>
public class Coordinate
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    /// <summary>
    /// Constructor for the Coordinate class
    /// </summary>
    /// <param name="X">X Coordinate</param>
    /// <param name="Y">Y Coordinate</param>
    /// <param name="Z">Z Coordinate</param>
    public Coordinate(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public override bool Equals(object? obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Coordinate c = (Coordinate)obj;
            return (x == c.x) && (y == c.y) && (z == c.z);
        }
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
#endregion
#region Rotation Class
/// <summary>
/// The Rotation class will be used to keep track of the pitch, yaw and roll of the spaceship.
/// </summary>
public class Rotation
{
    public float p { get; set; }
    public float y { get; set; }
    public float r { get; set; }

    /// <summary>
    /// Constructor for the Rotation class
    /// </summary>
    /// <param name="P">Holds the Pitch</param>
    /// <param name="Y">Holds the Yaw</param>
    /// <param name="R">Holds the Roll</param>
    public Rotation(float P, float Y, float R)
    {
        p = P;
        y = Y;
        r = R;
    }
    public override bool Equals(object? obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Rotation rotation = (Rotation)obj;
            return (p == rotation.p) && (y == rotation.y) && (r == rotation.r);
        }
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
#endregion
#region Status Class
/// <summary>
/// The Status classed will keep control of various things that need to be known about the spaceship
/// at all times (Ex. its chargeStatus).
/// </summary>
public class Status
{
    public bool payloadPower { get; set; }
    public bool dataWaiting { get; set; }
    public bool chargeStatus { get; set; }
    public float voltage { get; set; }

    /// <summary>
    /// This is the constructor for the Status class
    /// </summary>
    /// <param name="payloadPower">If the payload currently has power</param>
    /// <param name="dataWaiting">If there is data waiting for download</param>
    /// <param name="chargeStatus">If the spaceship is currently charging or not</param>
    /// <param name="voltage">Voltage level of the spaceship</param>
    public Status(bool payloadPower, bool dataWaiting, bool chargeStatus, float voltage)
    {
        this.payloadPower = payloadPower;
        this.dataWaiting = dataWaiting;
        this.chargeStatus = chargeStatus;
        this.voltage = voltage;
    }
    public override bool Equals(object? obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Status status = (Status)obj;
            return (payloadPower == status.payloadPower) && (dataWaiting == status.dataWaiting) &&
                (chargeStatus == status.chargeStatus) && (voltage == status.voltage);
        }
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
#endregion
#region Telemetry Class
/// <summary>
/// The Telemetry class will contain all of the data that needs to be tracked/maintained
/// </summary>
public class Telemetry
{
    public Coordinate? coordinate { get; set; }
    public Rotation? rotation { get; set; }
    public float fuel { get; set; }
    public float temp { get; set; }
    public Status? status { get; set; }
    public override bool Equals(object? obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Telemetry telemetry = (Telemetry)obj;
            return (coordinate.Equals(telemetry.coordinate)) && (rotation.Equals(telemetry.rotation))
                && (fuel.Equals(telemetry.fuel)) && (temp.Equals(telemetry.temp)) && (status.Equals(telemetry.status));
        }
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
#endregion
#endregion
