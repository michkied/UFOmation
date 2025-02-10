using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using UFOmation.models;

namespace UFOmation;

public class Animation : GameWindow
{
    private readonly Shader _shader;
    private readonly Shader _mirrorShader;

    private int _vertexArrayObject;
    private int _vertexBufferObject;

    private int _width;
    private int _height;

    private readonly List<Model> _models = new();

    private readonly Mirror _mirror;
    private readonly UFO _ufo;

    private bool _rotate;

    public Animation(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings
        {
            ClientSize = (width, height), Title = title, NumberOfSamples = 4, WindowState = WindowState.Fullscreen,
            StencilBits = 8
        }
    )
    {
        _shader = new Shader("../../../shaders/shader.vert", "../../../shaders/shader.frag");
        _mirrorShader = new Shader("../../../shaders/mirror.vert", "../../../shaders/mirror.frag");

        // _models.Add(new Surface(_shader));
        _models.Add(new Sphere(_shader));

        _ufo = new UFO(_shader);
        _models.Add(_ufo);

        _mirror = new Mirror(_mirrorShader);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        if (KeyboardState.IsKeyDown(Keys.Space)) _rotate = false;
        if (KeyboardState.IsKeyDown(Keys.R)) _rotate = true;

        if (KeyboardState.IsKeyDown(Keys.D1)) _cameraType = CameraType.Static;
        if (KeyboardState.IsKeyDown(Keys.D2)) _cameraType = CameraType.Follow;
        if (KeyboardState.IsKeyDown(Keys.D3)) _cameraType = CameraType.UFO;
    }

    private enum CameraType
    {
        Static,
        Follow,
        UFO
    }

    private CameraType _cameraType = CameraType.Static;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);

        var cameraPosition = new Vector3(0.0f, 1.0f, 3.0f);
        var view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
            (float)ClientSize.X / ClientSize.Y, 0.1f, 100.0f
        );

        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        _shader.SetVector3("viewPos", cameraPosition);

        _mirrorShader.SetMatrix4("view", view);
        _mirrorShader.SetMatrix4("projection", projection);

        _mirror.SetupMirrorFBO();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }

    private float _angle;
    private readonly float _radius = 3.0f; // Distance from the origin
    private readonly float _speed = 0.5f; // Rotation speed

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Update rotation angle
        // if (_rotate) _angle += (float)e.Time * _speed;
        // var camX = _radius * MathF.Sin(_angle);
        // var camX = 0.0f;
        // var camZ = _radius * MathF.Cos(_angle);
        Vector3 cameraPosition;
        Matrix4 view;
        // var view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        switch (_cameraType)
        {
            case CameraType.Static:
                cameraPosition = new Vector3(0.0f, 0.7f, 3.0f);
                view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
                break;
            case CameraType.Follow:
                cameraPosition = new Vector3(1.0f, 0.7f, 1.0f);
                view = Matrix4.LookAt(cameraPosition, _ufo.Position, Vector3.UnitY);
                break;
            case CameraType.UFO:
                cameraPosition = _ufo.Eye;
                view = _ufo.GetUFOView();
                break;
            default:
                throw new Exception("Unknown CameraType!");
        }

        _mirror.DrawReflection(_shader, cameraPosition, _models);

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        var projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
            ClientSize.X / (float)ClientSize.Y,
            0.1f,
            100.0f);
        _shader.Use();
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        _shader.SetVector3("viewPos", cameraPosition);
        _mirrorShader.SetMatrix4("view", view);
        _mirrorShader.SetMatrix4("projection", projection);

        foreach (var model in _models) model.Draw((float)e.Time);
        _mirror.Draw((float)e.Time);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}