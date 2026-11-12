using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

namespace Egc;

public class OrbitCamera : Camera
{
    public Vector3 Target = Vector3.Zero;
    public float Distance = 15.0f;
    public float Azimuth = 45.0f;
    public float Elevation = 30.0f;

    public override string Name => "Orbit Camera (Tema)";

    // Calculăm poziția camerei pe baza țintei, distanței și unghiurilor
    public override Vector3 Position
    {
        get
        {
            float elRad = MathHelper.DegreesToRadians(Elevation);
            float azRad = MathHelper.DegreesToRadians(Azimuth);

            float x = Target.X + Distance * MathF.Cos(elRad) * MathF.Cos(azRad);
            float y = Target.Y + Distance * MathF.Sin(elRad);
            float z = Target.Z + Distance * MathF.Cos(elRad) * MathF.Sin(azRad);

            return new Vector3(x, y, z);
        }
    }

    public override Vector3 Front => Vector3.Normalize(Target - Position);

    public override Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Vector3.UnitY);

    public override void Update(KeyboardState keyboard, MouseState mouse, Vector2 mouseDelta, float dt)
    {
        // Rotița schimbă distanța (nu FOV-ul)
        if (mouse.ScrollDelta.Y != 0)
        {
            Distance -= mouse.ScrollDelta.Y;
            Distance = Math.Clamp(Distance, 2.0f, 50.0f);
        }

        // Clic stânga ținut + mișcare mouse rotește camera
        if (mouse.IsButtonDown(MouseButton.Left))
        {
            Azimuth -= mouseDelta.X * 0.5f;
            Elevation += mouseDelta.Y * 0.5f;
        }

        // Săgețile rotesc camera de la tastatură
        float rotSpeed = 90.0f * dt;
        if (keyboard.IsKeyDown(Keys.Left)) Azimuth -= rotSpeed;
        if (keyboard.IsKeyDown(Keys.Right)) Azimuth += rotSpeed;
        if (keyboard.IsKeyDown(Keys.Up)) Elevation += rotSpeed;
        if (keyboard.IsKeyDown(Keys.Down)) Elevation -= rotSpeed;

        // Limităm elevația la +/- 89 grade pentru a preveni răsturnarea imaginii
        Elevation = Math.Clamp(Elevation, -89.0f, 89.0f);
    }

    public override void Reset()
    {
        Target = Vector3.Zero;
        Distance = 15.0f;
        Azimuth = 45.0f;
        Elevation = 30.0f;
    }

    public override IEnumerable<string> Describe()
    {
        yield return $"Target: {Target:F2}";
        yield return $"Distance: {Distance:F2}";
        yield return $"Azimuth: {Azimuth:F1}° | Elevation: {Elevation:F1}°";
        yield return $"Position: {Position:F2}";
    }
}