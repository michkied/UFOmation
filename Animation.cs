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
            ClientSize = (width, height), Title = title, NumberOfSamples = 4, WindowState = WindowState.Fullscreen
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
        
        CursorState = CursorState.Grabbed;

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


        // DirLight dirLight;
        // dirLight.direction = normalize(vec3(vec4(0.0, -1.0, 0.0, 0.0) * view));
        // dirLight.ambient = vec3(0.05, 0.05, 0.05);
        // dirLight.diffuse = vec3(1.0, 1.0, 1.0);
        // dirLight.specular = vec3(0.5, 0.5, 0.5);

        _shader.SetVector3("dirLight.direction", new Vector3(new Vector4(0.0f, -1.0f, 0.0f, 0.0f) * view));
        _shader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
        _shader.SetVector3("dirLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
        _shader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));


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


    private Vector2 _mousePosition;

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        _mousePosition = new Vector2(e.X, e.Y);
    }

    private Vector3 GetMouseRayDirection(Vector2 mousePos, Matrix4 projection)
    {
        var x = 2.0f * mousePos.X / ClientSize.X - 1.0f;
        var y = 1.0f - 2.0f * mousePos.Y / ClientSize.Y;
        
        var rayClip = new Vector4(x, y, -1.0f, 1.0f);
        
        var invProjection = Matrix4.Invert(projection);
        var rayEye = invProjection * rayClip;
        rayEye.Z = -1.0f;
        rayEye.W = 0.0f;
        
        rayEye.Normalize();

        return new Vector3(rayEye);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        Vector3 cameraPosition;
        Matrix4 view;
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

        var projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
            ClientSize.X / (float)ClientSize.Y,
            0.1f,
            100.0f);

        _shader.SetVector3("dirLight.direction", new Vector3(new Vector4(0.0f, -1.0f, 0.0f, 0.0f) * view));
        
        var ufoViewPos = new Vector3(new Vector4(_ufo.Position, 1.0f) * view);
        Vector3 spotlightDir;
        if (_cameraType == CameraType.UFO)
        {
            spotlightDir = GetMouseRayDirection(_mousePosition, projection) - ufoViewPos;
        }
        else
        {
            spotlightDir = new Vector3(new Vector4(-_ufo.Position, 0.0f) * view);
        }
        _shader.SetVector3("spotLight.direction", spotlightDir);
        _shader.SetVector3("spotLight.position", ufoViewPos);
        _shader.SetFloat("spotLight.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20.0f)));
        _shader.SetFloat("spotLight.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(30.0f)));
        _shader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
        _shader.SetVector3("spotLight.diffuse", new Vector3(0.643f, 1.0f, 0.357f));
        _shader.SetVector3("spotLight.specular", new Vector3(0.643f, 1.0f, 0.357f));
        _shader.SetFloat("spotLight.constant", 1.0f);
        _shader.SetFloat("spotLight.linear", 0.09f);
        _shader.SetFloat("spotLight.quadratic", 0.032f);

        _mirror.DrawReflection(_shader, cameraPosition, _models);

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
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