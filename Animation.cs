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

    private int _vertexArrayObject;
    private int _vertexBufferObject;

    private int _width;
    private int _height;

    private readonly List<Model> _models = new();

    public Animation(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings
            { ClientSize = (width, height), Title = title, NumberOfSamples = 4, WindowState = WindowState.Fullscreen }
    )
    {
        _shader = new Shader("../../../shaders/shader.vert", "../../../shaders/shader.frag");

        _models.Add(new Surface(_shader));
        // _models.Add(new Sphere(_shader));
        // _models.Add(new UFO(_shader));
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
    }

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

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }

    private float _angle;
    private readonly float _radius = 3.0f; // Distance from the origin
    private readonly float _speed = 0.2f; // Rotation speed

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Update rotation angle
        _angle += (float)e.Time * _speed;

        // Compute new camera position
        var camX = _radius * MathF.Cos(_angle);
        var camZ = _radius * MathF.Sin(_angle);

        var cameraPosition = new Vector3(camX, 1.0f, camZ);
        var view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        _shader.SetMatrix4("view", view);
        _shader.SetVector3("viewPos", cameraPosition);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _shader.Use();

        foreach (var model in _models) model.Draw();

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}