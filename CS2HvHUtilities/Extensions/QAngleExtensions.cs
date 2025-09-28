namespace cs2hvh_utilities.Extensions;

public static class QAngleExtensions
{
    public static bool IsValid(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        return angle.IsFinite() && !angle.IsNaN() && angle.IsReasonable();
    }

    private static bool IsFinite(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        return float.IsFinite(angle.X) && float.IsFinite(angle.Y) && float.IsFinite(angle.Z);
    }

    private static bool IsNaN(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        return float.IsNaN(angle.X) || float.IsNaN(angle.Y) || float.IsNaN(angle.Z);
    }
    public static void Fix(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        angle.FixInfinity();
        angle.FixNaN();

        if (!angle.IsReasonable())
            angle.Normalize();

        angle.Clamp();
    }

    private static void FixInfinity(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        if (!float.IsFinite(angle.X))
            angle.X = 0;
        if (!float.IsFinite(angle.Y))
            angle.Y = 0;
        if (!float.IsFinite(angle.Z))
            angle.Z = 0;
    }

    private static void FixNaN(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        if (float.IsNaN(angle.X))
            angle.X = 0;
        if (float.IsNaN(angle.Y))
            angle.Y = 0;
        if (float.IsNaN(angle.Z))
            angle.Z = 0;
    }

    private static void Clamp(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        angle.X = Math.Clamp(angle.X, -179.0f, 179.0f);
        angle.Y = Math.Clamp(angle.Y, -180.0f, 180.0f);
        angle.Z = 0;
    }

    private static void Normalize(this CounterStrikeSharp.API.Modules.Utils.QAngle angle)
    {
        angle.X = (angle.X + 180.0f) % 360.0f - 180.0f;
        angle.Y = (angle.Y + 180.0f) % 360.0f - 180.0f;
        angle.Z = (angle.Z + 180.0f) % 360.0f - 180.0f;
    }

    private static bool IsReasonable(this CounterStrikeSharp.API.Modules.Utils.QAngle q )
    {
        return
            q.X is >= -89 and <= 89 &&
            q.Y is >= -180 and <= 180 &&
            q.Z == 0;
    }
}