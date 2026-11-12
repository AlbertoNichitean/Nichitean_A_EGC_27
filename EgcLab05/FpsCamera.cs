using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc;

/// <summary>
/// Camera FPS (first-person): orientarea vine din două unghiuri Euler, yaw și pitch,
/// controlate cu mouse-ul; deplasarea se face cu W A S D.
/// În modul "mers" (implicit) camera rămâne la înălțimea ochilor; în modul "zbor liber"
/// (free-fly, tasta G) se deplasează pe direcția privirii.
/// </summary>
public class FpsCamera : Camera
{
    // --- Starea camerei ---------------------------------------------------------

    private Vector3 _position;
    public override Vector3 Position => _position;

    /// <summary>Girația (yaw), în grade: rotația stânga-dreapta, în jurul axei Y. -90° = privire spre -Z.</summary>
    public float Yaw { get; private set; }

    /// <summary>Tangajul (pitch), în grade: rotația sus-jos. Limitat la ±MaxPitch.</summary>
    public float Pitch { get; private set; }

    /// <summary>Zbor liber (true) sau mers pe sol (false). Comutat cu tasta G (facultativ, TODO 5.7).</summary>
    public bool FreeFly { get; set; }

    // Baza camerei: trei vectori unitate, perpendiculari doi câte doi (secțiunea 5).
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _right = Vector3.UnitX;
    private Vector3 _up = Vector3.UnitY;

    public override Vector3 Front => _front;

    // --- Parametri ----------------------------------------------------------------

    public const float MaxPitch = 89.0f;               // grade; de ce nu 90, vezi secțiunea 5.3
    public float MoveSpeed = 3.0f;                     // unități de lume pe secundă
    public float SprintFactor = 2.5f;                  // Shift ținut apăsat
    public float MouseSensitivity = 0.1f;              // grade pe pixel
    public float FovStep = 2.0f;                       // grade pentru un pas al rotiței

    private static readonly Vector3 WorldUp = Vector3.UnitY;
    private static readonly Vector3 StartPosition = new(0.0f, 1.7f, 6.0f);   // 1,7 m = înălțimea ochilor
    private const float StartYaw = -90.0f;
    private const float StartPitch = 0.0f;

    public FpsCamera()
    {
        Reset();
    }

    public override string Name => FreeFly ? "FPS – zbor liber" : "FPS – mers";

    /// <summary>Matricea view: camera aflată în Position privește spre punctul Position + Front.</summary>
    public override Matrix4 ViewMatrix =>
        Matrix4.LookAt(_position, _position + _front, _up);

    public override void Update(KeyboardState keyboard, MouseState mouse, Vector2 mouseDelta, float dt)
    {
        // --- 1. Orientarea: mouse-ul modifică yaw și pitch -------------------------

        // TODO 5.5 (a): deplasarea pe orizontală modifică yaw, cea pe verticală modifică pitch.
        //               Atenție: pe ecran Y crește în jos, iar pitch pozitiv înseamnă privire în sus.
        //               Apoi limitați pitch-ul la intervalul [-MaxPitch, MaxPitch].
        Yaw += mouseDelta.X * MouseSensitivity;
        Pitch -= mouseDelta.Y * MouseSensitivity; // Minus pentru că pe ecran Y crește în jos

        // Limităm Pitch-ul (tangajul) la +/- 89 de grade pentru a evita "datul peste cap" (gimbal lock)
        Pitch = Math.Clamp(Pitch, -MaxPitch, MaxPitch);


        UpdateVectors();

        // --- 2. FOV: rotița mouse-ului --------------------------------------------

        // TODO 5.1 (c): rotița în sus (ScrollDelta.Y > 0) micșorează FOV-ul (efect de zoom)
        //               (folosiți ChangeFov din clasa Camera și FovStep)
        if (mouse.ScrollDelta.Y != 0)
        {
            // Rotița în sus micșorează FOV-ul (am pus un factor de multiplicare de 5 grade pe pas)
            ChangeFov(-mouse.ScrollDelta.Y * 5.0f);
        }


        // --- 3. Deplasarea: W A S D ---------------------------------------------------

        // TODO 5.6: direcția "înainte" (în modul mers: proiecția lui Front pe planul orizontal),
        //           apoi deplasarea cu viteză constantă, independentă de numărul de cadre pe secundă.
        //           Pași: (1) forward; (2) vectorul "move" = suma direcțiilor tastelor apăsate
        //           (W +forward, S -forward, D +_right, A -_right); (3) normalizați "move";
        //           (4) _position += move * viteză * dt (viteza crește de SprintFactor ori cu Shift stânga).
        // 1. Calculăm direcția "înainte" proiectată pe orizontală (y = 0)
        Vector3 forwardHorizontal = new Vector3(_front.X, 0, _front.Z);
        if (forwardHorizontal != Vector3.Zero)
        {
            forwardHorizontal = Vector3.Normalize(forwardHorizontal);
        }

        // 2. Adunăm direcțiile tastelor apăsate
        Vector3 moveDirection = Vector3.Zero;
        if (keyboard.IsKeyDown(Keys.W)) moveDirection += forwardHorizontal;
        if (keyboard.IsKeyDown(Keys.S)) moveDirection -= forwardHorizontal;
        if (keyboard.IsKeyDown(Keys.A)) moveDirection -= _right;
        if (keyboard.IsKeyDown(Keys.D)) moveDirection += _right;

        // 3. Normalizăm vectorul de deplasare (pentru a nu merge mai repede pe diagonală)
        if (moveDirection.LengthSquared > 0)
        {
            moveDirection = Vector3.Normalize(moveDirection);
        }

        // 4. Calculăm viteza de deplasare (Shift stânga pentru sprint)
        float currentSpeed = 3.0f; // Viteza de bază (dacă ai o variabilă WalkSpeed în clasă, folosește-o pe aceea)
        if (keyboard.IsKeyDown(Keys.LeftShift))
        {
            currentSpeed *= SprintFactor;
        }

        // 5. Actualizăm poziția camerei
        _position += moveDirection * currentSpeed * dt;

        // TODO 5.7 (facultativ): în zbor liber (FreeFly), "forward" este chiar _front,
        //           iar Space urcă și Ctrl stânga coboară (pe axa Y a lumii, WorldUp).
    }

    /// <summary>
    /// Recalculează baza camerei din yaw și pitch (unghiuri Euler -> vector de direcție).
    /// </summary>
    
        // TODO 5.5 (b): Front din yaw și pitch (formula din secțiunea 5.2), apoi Right și Up
        //               prin produse vectoriale; toți trei normalizați.
        //               Până atunci camera privește mereu spre -Z.
        private void UpdateVectors()
    {
        // Funcțiile trigonometrice au nevoie de radiani
        float yawRad = MathHelper.DegreesToRadians(Yaw);
        float pitchRad = MathHelper.DegreesToRadians(Pitch);

        // 1. Calculăm direcția Front (înainte) folosind coordonate sferice
        Vector3 front;
        front.X = MathF.Cos(yawRad) * MathF.Cos(pitchRad);
        front.Y = MathF.Sin(pitchRad);
        front.Z = MathF.Sin(yawRad) * MathF.Cos(pitchRad);
        
        _front = Vector3.Normalize(front);

        // 2. Calculăm direcția Right (dreapta) făcând produsul vectorial dintre Front și vectorul "Up" al lumii (0, 1, 0)
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));

        // 3. Calculăm direcția Up (sus) a camerei făcând produsul vectorial dintre Right și Front
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }
       

    public override void Reset()
    {
        _position = StartPosition;
        Yaw = StartYaw;
        Pitch = StartPitch;
        FieldOfView = 60.0f;
        UpdateVectors();
    }

    public override IEnumerable<string> Describe()
    {
        yield return $"Poziție        : ({_position.X,6:F2}, {_position.Y,6:F2}, {_position.Z,6:F2})";
        yield return $"Yaw / pitch    : {NormalizeAngle(Yaw),7:F1}° / {Pitch,6:F1}°";
        yield return $"Front          : ({_front.X,6:F2}, {_front.Y,6:F2}, {_front.Z,6:F2})";
        yield return $"Right          : ({_right.X,6:F2}, {_right.Y,6:F2}, {_right.Z,6:F2})";
        yield return $"Up             : ({_up.X,6:F2}, {_up.Y,6:F2}, {_up.Z,6:F2})";
    }

    /// <summary>Aduce un unghi în intervalul (-180°, 180°] (doar pentru afișare).</summary>
    private static float NormalizeAngle(float degrees)
    {
        degrees %= 360.0f;
        if (degrees > 180.0f) degrees -= 360.0f;
        if (degrees <= -180.0f) degrees += 360.0f;
        return degrees;
    }
}
