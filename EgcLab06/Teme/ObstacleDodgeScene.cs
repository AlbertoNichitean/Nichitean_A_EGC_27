// Tema 06 - Varianta B: Evita obstacolele
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc.Teme;

public class ObstacleDodgeScene : Scene
{
    private const string ActionLeft = "Banda stânga";
    private const string ActionRight = "Banda dreapta";
    private const string ActionJump = "Salt";
    private const string ActionRestart = "Joc nou";

    private static readonly float[] LaneX = { -2.4f, 0.0f, 2.4f };

    private const float PlayerWidth = 0.85f;
    private const float PlayerHeight = 0.95f;
    private const float PlayerDepth = 0.85f;
    private const float PlayerZ = 3.5f;

    private const float Gravity = 22.0f;
    private const float JumpSpeed = 9.5f;

    private const float RoadStartZ = -34.0f;
    private const float RoadEndZ = 10.0f;

    private Mesh _cubeMesh = null!;
    private Mesh _sphereMesh = null!;

    private SceneObject _playerRoot = null!;
    private SceneObject _playerVisor = null!;
    private SceneObject _shadow = null!;

    private readonly List<Obstacle> _obstacles = new();
    private readonly List<SceneObject> _roadMarkers = new();
    private readonly Random _rng = new();

    private int _currentLane = 1;
    private Tween _laneTween = null!;

    private float _playerY;
    private float _verticalSpeed;
    private bool _onGround = true;

    private float _worldSpeed = 9.0f;
    private float _spawnTimer;
    private float _spawnInterval = 1.35f;

    private float _score;
    private float _bestScore;
    private bool _gameOver;

    public override string Name => "Varianta B: Evită obstacolele";

    public override string Status => _gameOver
        ? $"GAME OVER | Scor: {_score:F1}s | Record: {_bestScore:F1}s (R = restart)"
        : $"Scor: {_score:F1}s | Record: {_bestScore:F1}s | Viteză: {_worldSpeed:F1}";

    public override IEnumerable<string> ActionNames => new[]
    {
        ActionLeft, ActionRight, ActionJump, ActionRestart
    };

    private sealed class Obstacle
    {
        public SceneObject Obj = null!;
        public bool IsTall;
        public float HalfW;
        public float HalfH;
        public float HalfD;
    }

    public override void Load(Input input)
    {
        CameraPosition = new Vector3(0.0f, 5.2f, 10.5f);
        CameraTarget = new Vector3(0.0f, 1.0f, -4.0f);

        input.Bind(ActionLeft, Keys.A, Keys.Left);
        input.Bind(ActionRight, Keys.D, Keys.Right);
        input.Bind(ActionJump, Keys.Space, Keys.W, Keys.Up);
        input.Bind(ActionRestart, Keys.R);

        _cubeMesh = AddMesh(Shapes.ShadedCube());
        _sphereMesh = AddMesh(Shapes.ShadedSphere(0.5f, 16, 12));

        // 1. Drumul cu 3 benzi + borduri
        var road = AddObject("Drum", _cubeMesh);
        road.Transform.Position = new Vector3(0.0f, -0.1f, -12.0f);
        road.Transform.Scale = new Vector3(7.6f, 0.2f, 46.0f);
        road.Tint = new Color4(0.18f, 0.20f, 0.24f, 1.0f);

        for (int side = -1; side <= 1; side += 2)
        {
            var border = AddObject("Bordură", _cubeMesh);
            border.Transform.Position = new Vector3(side * 3.95f, 0.08f, -12.0f);
            border.Transform.Scale = new Vector3(0.3f, 0.35f, 46.0f);
            border.Tint = new Color4(0.85f, 0.30f, 0.25f, 1.0f);
        }

        // Bonus: marcaje discontinue între benzi care se deplasează odată cu lumea
        float[] dividerX = { -1.2f, 1.2f };
        for (int d = 0; d < dividerX.Length; d++)
        {
            for (float z = RoadStartZ; z <= RoadEndZ; z += 4.0f)
            {
                var mark = AddObject("Marcaj", _cubeMesh);
                mark.Transform.Position = new Vector3(dividerX[d], 0.01f, z);
                mark.Transform.Scale = new Vector3(0.14f, 0.02f, 1.6f);
                mark.Tint = new Color4(0.90f, 0.90f, 0.85f, 1.0f);
                _roadMarkers.Add(mark);
            }
        }

        // 2. Jucătorul (ierarhie părinte-copil: corp + vizor copil)
        _playerRoot = AddObject("Jucător", _cubeMesh);
        _playerRoot.Transform.Scale = new Vector3(PlayerWidth, PlayerHeight, PlayerDepth);
        _playerRoot.Tint = new Color4(0.20f, 0.80f, 0.95f, 1.0f);

        _playerVisor = AddObject("VizorJucător", _cubeMesh, _playerRoot);
        _playerVisor.Transform.Position = new Vector3(0.0f, 0.22f, -0.45f);
        _playerVisor.Transform.Scale = new Vector3(0.75f, 0.24f, 0.22f);
        _playerVisor.Tint = new Color4(0.98f, 0.92f, 0.30f, 1.0f);

        _shadow = AddObject("Umbră", _sphereMesh);
        _shadow.Tint = new Color4(0.06f, 0.07f, 0.09f, 1.0f);

        ResetGame();
    }

    private void ResetGame()
    {
        foreach (var obs in _obstacles)
        {
            Objects.Remove(obs.Obj);
        }
        _obstacles.Clear();

        _currentLane = 1;
        _laneTween = new Tween(new Vector3(LaneX[1], 0f, 0f), new Vector3(LaneX[1], 0f, 0f), 0.14f, Easing.EaseOut);
        _laneTween.Update(1.0f);

        _playerY = PlayerHeight * 0.5f;
        _verticalSpeed = 0.0f;
        _onGround = true;

        _worldSpeed = 9.0f;
        _spawnInterval = 1.35f;
        _spawnTimer = 0.4f;
        _score = 0.0f;
        _gameOver = false;

        _playerRoot.Tint = new Color4(0.20f, 0.80f, 0.95f, 1.0f);
        SyncPlayerTransform();
    }

    public override void Update(float dt, Input input)
    {
        if (input.WasPressed(ActionRestart))
        {
            ResetGame();
            return;
        }

        if (_gameOver || dt <= 0.0f) return;

        _score += dt;
        if (_score > _bestScore) _bestScore = _score;

        // Viteza lumii și frecvența obstacolelor cresc treptat
        _worldSpeed = MathF.Min(23.0f, 9.0f + _score * 0.35f);
        _spawnInterval = MathF.Max(0.55f, 1.35f - _score * 0.02f);

        // 1. Schimbare bandă lină (Tween cu EaseOut sub 0.2s)
        if (input.WasPressed(ActionLeft) && _currentLane > 0)
        {
            float currentX = _laneTween.Value.X;
            _currentLane--;
            _laneTween = new Tween(new Vector3(currentX, 0f, 0f), new Vector3(LaneX[_currentLane], 0f, 0f), 0.14f, Easing.EaseOut);
        }
        if (input.WasPressed(ActionRight) && _currentLane < 2)
        {
            float currentX = _laneTween.Value.X;
            _currentLane++;
            _laneTween = new Tween(new Vector3(currentX, 0f, 0f), new Vector3(LaneX[_currentLane], 0f, 0f), 0.14f, Easing.EaseOut);
        }
        _laneTween.Update(dt);

        // 2. Salt cu gravitație (Euler semi-implicit)
        if (_onGround && input.WasPressed(ActionJump))
        {
            _verticalSpeed = JumpSpeed;
            _onGround = false;
        }

        if (!_onGround)
        {
            _verticalSpeed -= Gravity * dt;
            _playerY += _verticalSpeed * dt;

            float groundY = PlayerHeight * 0.5f;
            if (_playerY <= groundY)
            {
                _playerY = groundY;
                _verticalSpeed = 0.0f;
                _onGround = true;
            }
        }

        SyncPlayerTransform();

        // 3. Animarea marcajelor de pe drum
        foreach (var mark in _roadMarkers)
        {
            var pos = mark.Transform.Position;
            pos.Z += _worldSpeed * dt;
            if (pos.Z > RoadEndZ)
            {
                pos.Z -= (RoadEndZ - RoadStartZ);
            }
            mark.Transform.Position = pos;
        }

        // 4. Generare și deplasare obstacole + verificare coliziune AABB
        _spawnTimer -= dt;
        if (_spawnTimer <= 0.0f)
        {
            SpawnWave();
            _spawnTimer = _spawnInterval;
        }

        Vector3 pCenter = _playerRoot.Transform.Position;
        Vector3 pHalf = new(PlayerWidth * 0.45f, PlayerHeight * 0.48f, PlayerDepth * 0.45f);

        for (int i = _obstacles.Count - 1; i >= 0; i--)
        {
            var obs = _obstacles[i];
            var pos = obs.Obj.Transform.Position;
            pos.Z += _worldSpeed * dt;
            obs.Obj.Transform.Position = pos;

            // Test coliziune AABB pe toate cele 3 axe
            bool overlapX = MathF.Abs(pCenter.X - pos.X) < (pHalf.X + obs.HalfW);
            bool overlapY = MathF.Abs(pCenter.Y - pos.Y) < (pHalf.Y + obs.HalfH);
            bool overlapZ = MathF.Abs(pCenter.Z - pos.Z) < (pHalf.Z + obs.HalfD);

            if (overlapX && overlapY && overlapZ)
            {
                _gameOver = true;
                _playerRoot.Tint = new Color4(0.95f, 0.22f, 0.22f, 1.0f);
                return;
            }

            if (pos.Z > RoadEndZ + 2.0f)
            {
                Objects.Remove(obs.Obj);
                _obstacles.RemoveAt(i);
            }
        }
    }

    private void SyncPlayerTransform()
    {
        float x = _laneTween.Value.X;
        _playerRoot.Transform.Position = new Vector3(x, _playerY, PlayerZ);

        float heightAboveGround = MathF.Max(0.0f, _playerY - PlayerHeight * 0.5f);
        float shadowScale = MathF.Max(0.35f, 0.95f - heightAboveGround * 0.22f);
        _shadow.Transform.Position = new Vector3(x, 0.015f, PlayerZ);
        _shadow.Transform.Scale = new Vector3(shadowScale, 0.02f, shadowScale);
    }

    private void SpawnWave()
    {
        // Blocăm cel mult 2 benzi simultan ca să existe întotdeauna o cale de scăpare
        int count = _rng.NextDouble() < 0.45 ? 2 : 1;
        int firstLane = _rng.Next(0, 3);
        SpawnObstacleOnLane(firstLane);

        if (count == 2)
        {
            int secondLane = (firstLane + _rng.Next(1, 3)) % 3;
            SpawnObstacleOnLane(secondLane);
        }
    }

    private void SpawnObstacleOnLane(int lane)
    {
        bool tall = _rng.NextDouble() < 0.45;
        float w = 1.45f;
        float h = tall ? 2.25f : 0.85f;
        float d = 1.15f;

        var obj = AddObject(tall ? "ObstacolÎnalt" : "ObstacolJos", _cubeMesh);
        obj.Transform.Position = new Vector3(LaneX[lane], h * 0.5f, RoadStartZ);
        obj.Transform.Scale = new Vector3(w, h, d);
        obj.Tint = tall
            ? new Color4(0.92f, 0.28f, 0.25f, 1.0f)   // roșu = înalt (se ocolește)
            : new Color4(0.96f, 0.68f, 0.18f, 1.0f);  // portocaliu = jos (se poate sări)

        _obstacles.Add(new Obstacle
        {
            Obj = obj,
            IsTall = tall,
            HalfW = w * 0.48f,
            HalfH = h * 0.48f,
            HalfD = d * 0.48f
        });
    }

    public override IEnumerable<string> Describe()
    {
        yield return $"Stare joc      : {(_gameOver ? "GAME OVER" : "în desfășurare")}";
        yield return $"Scor / Record  : {_score:F2} s / {_bestScore:F2} s";
        yield return $"Viteză lume    : {_worldSpeed:F1} unități/s";
        yield return $"Obstacole      : {_obstacles.Count} active";
    }
}