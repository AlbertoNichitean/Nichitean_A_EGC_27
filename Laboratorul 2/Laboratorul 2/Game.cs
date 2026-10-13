using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;

namespace Egc;

/// <summary>
/// Fereastra aplicației și bucla principală (main game loop).
/// GameWindow apelează automat metodele On... de mai jos, în ordinea descrisă în laborator.
/// </summary>
public class Game : GameWindow
{

    // Obiecte care NU depind de OpenGL pot fi create imediat.
    private readonly Camera _camera = new();

    // Obiectele care folosesc OpenGL se creează în OnLoad (abia atunci există contextul OpenGL).
    private Shader _shader = null!;
    private Triangle _triangle = null!;

    // Starea aplicației (se modifică în OnUpdateFrame, se folosește în OnRenderFrame).

    private Color4 _clearColor = new(0.0f, 0.0f, 0.5f, 1.0f);   // culoarea de fundal
    private Color4 _tint = new Color4(1.0f, 0.3f, 0.3f, 1.0f);                          // nuanța triunghiului (alb = neschimbat)
    private Vector3 _offset = Vector3.Zero;                       // deplasarea triunghiului
    private bool _wireframe = false;
    private float _topX = 0.0f;
    private float _topY = 0.5f;

    // Contoare pentru exemplul cu bucla principală (afișate în titlul ferestrei).
    private readonly string _baseTitle;
    private double _elapsed;     // timpul total scurs (secunde)
    private double _secondTimer; // cronometru pentru actualizarea titlului o dată pe secundă
    private int _updates;        // câte OnUpdateFrame în ultima secundă



    private int _frames;         // câte OnRenderFrame în ultima secundă

    public Game(int width, int height, string title)
        : base(GameWindowSettings.Default,
               new NativeWindowSettings
               {
                   ClientSize = new Vector2i(width, height),
                   Title = title,
                   APIVersion = new Version(3, 3),          // cerem OpenGL 3.3 ...
                   Profile = ContextProfile.Core,           // ... profil Core (fără modul imediat)
                   Flags = ContextFlags.ForwardCompatible
               })
    {
        _baseTitle = title;
        VSync = VSyncMode.On;   // swap-ul bufferelor așteaptă reîmprospătarea monitorului
    }


    /// <summary>Apelată O SINGURĂ DATĂ, după crearea ferestrei și a contextului OpenGL.</summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        // Informații despre driverul și placa grafică folosite (vezi Problema 1 din Laboratorul 01).
        Console.WriteLine($"OpenGL  : {GL.GetString(StringName.Version)}");
        Console.WriteLine($"Vendor  : {GL.GetString(StringName.Vendor)}");
        Console.WriteLine($"Renderer: {GL.GetString(StringName.Renderer)}");

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.PointSize(10.0f);

        _shader = Shader.FromFiles("Shaders/shader.vert", "Shaders/shader.frag");
        _triangle = new Triangle();
    }

    /// <summary>Apelată când se schimbă dimensiunea ferestrei.</summary>
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);   // zona ferestrei în care se desenează
        _camera.Resize(e.Width, e.Height);
    }

    /// <summary>Apelată la fiecare iterație a buclei: LOGICA aplicației (intrări, mișcare, timp).</summary>
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        // args.Time = secundele scurse de la apelul anterior (delta time).
        _elapsed += args.Time;
        _updates++;
        if (KeyboardState.IsKeyPressed(Keys.D1)) _tint = Color4.Red;
        if (KeyboardState.IsKeyPressed(Keys.D2)) _tint = Color4.Green;
        if (KeyboardState.IsKeyPressed(Keys.D3)) _tint = Color4.Blue;
        if (KeyboardState.IsKeyPressed(Keys.D0)) _tint = Color4.White; // Alb înseamnă neschimbat
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        // --- CODUL PENTRU PROBLEMA 5 (WASD) ---
        float dt = (float)args.Time;
        float viteza = 1.0f;

        if (KeyboardState.IsKeyDown(Keys.W)) _topY += viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.S)) _topY -= viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.A)) _topX -= viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.D)) _topX += viteza * dt;

        _triangle.SetPosition(2, _topX, _topY, 0.0f);
        _triangle.UpdateVertices();
        if (KeyboardState.IsKeyPressed(Keys.C))
        {
            // Setăm vârful 2 (cel de sus) să devină Alb (1.0f, 1.0f, 1.0f)
            _triangle.SetColor(2, 1.0f, 1.0f, 1.0f);

            // CRITIC: Fără această linie, imaginea nu se schimbă! Copiază datele noi din RAM în memoria GPU.
            _triangle.UpdateVertices();
        }

        if (KeyboardState.IsKeyPressed(Keys.F))
        {
            _wireframe = !_wireframe; // Inversează valoarea (din true în false și invers)
        }

        // --- TODO (P1–P5): codul vostru de logică se adaugă aici ---

        float r = 0.5f + 0.5f * MathF.Sin((float)_elapsed);
        float g = 0.5f + 0.5f * MathF.Sin((float)_elapsed + 2.094f); // defazaj de 2pi/3
        float b = 0.5f + 0.5f * MathF.Sin((float)_elapsed + 4.189f); // defazaj de 4pi/3

        _clearColor = new Color4(r, g, b, 1.0f);

        // Exemplul bucla principală: o dată pe secundă afișăm în titlu câte
        // actualizări și câte cadre desenate au avut loc.
        _secondTimer += args.Time;
        if (_secondTimer >= 1.0)
        {
            Title = $"{_baseTitle} | update/s: {_updates} | render/s: {_frames}";
            _updates = 0;
            _frames = 0;
            _secondTimer -= 1.0;
        }

        float t = (float)_elapsed;

        // Vârful 0 se mișcă stânga-dreapta
        _triangle.SetPosition(0, -0.5f + 0.3f * MathF.Sin(t), -0.5f, 0.0f);

        // Vârful 1 se mișcă sus-jos
        _triangle.SetPosition(1, 0.5f, -0.5f + 0.3f * MathF.Cos(t), 0.0f);

        // Vârful 2 se mișcă sus-jos cu o frecvență diferită
        _triangle.SetPosition(2, 0.0f, 0.5f + 0.3f * MathF.Sin(t * 2.0f), 0.0f);

        _triangle.UpdateVertices();

       // _triangle.SetPosition(0, 0.0f, 0.0f, 0.0f);
       // _triangle.SetPosition(1, 0.0f, 0.0f, 0.0f);
       // _triangle.SetPosition(2, 0.0f, 0.0f, 0.0f);
       // _triangle.UpdateVertices();


    }

    /// <summary>Apelată la fiecare iterație a buclei: DESENAREA unui cadru.</summary>
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        _frames++;

        // 1. Ștergem imaginea veche din back buffer. GL.ClearColor SETEAZĂ starea (culoarea de ștergere),
        //    GL.Clear o FOLOSEȘTE. Aici o setăm la fiecare cadru pentru că vom putea s-o schimbăm (P1).
        GL.ClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // 2. Activăm programul shader și îi trimitem uniformele.
        _shader.Use();
        _shader.SetMatrix4("uView", _camera.ViewMatrix);
        _shader.SetMatrix4("uProjection", _camera.ProjectionMatrix);
        _shader.SetVector3("uOffset", _offset);
        _shader.SetColor("uTint", _tint);
        float dt = (float)args.Time;
        float viteza = 1.0f; // 1 unitate NDC pe secundă

        if (KeyboardState.IsKeyDown(Keys.Left)) _offset.X -= viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.Right)) _offset.X += viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.Up)) _offset.Y += viteza * dt;
        if (KeyboardState.IsKeyDown(Keys.Down)) _offset.Y -= viteza * dt;

        if (_wireframe)
        {
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line); // Desenează doar conturul
            //GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Point);
        }
        else
        {
            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill); // Desenează triunghiul plin
        }

        // 3. Desenăm triunghiul.
        _triangle.Draw();

        // 4. Afișăm cadrul terminat: back buffer <-> front buffer.
        SwapBuffers();
    }


    /// <summary>Apelată O SINGURĂ DATĂ, la închiderea ferestrei: eliberăm resursele GPU.</summary>
    protected override void OnUnload()
    {
        _triangle.Dispose();
        _shader.Dispose();

        base.OnUnload();
    }
}




