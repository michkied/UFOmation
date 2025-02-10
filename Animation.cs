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
    private readonly Shader _lightPointShader;

    private readonly List<Model> _models = new();

    private readonly Mirror _mirror;
    private readonly Ufo _ufo;
    private readonly Sky _sky;

    private float _fogDensity = 0.0f;
    private float _specularStrength = 1.0f;

    public Animation(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings
        {
            ClientSize = (width, height), Title = title, NumberOfSamples = 4, WindowState = WindowState.Fullscreen
        }
    )
    {
        _shader = new Shader("../../../shaders/shader.vert", "../../../shaders/shader.frag");
        _mirrorShader = new Shader("../../../shaders/mirror.vert", "../../../shaders/mirror.frag");
        _lightPointShader = new Shader("../../../shaders/lightSource.vert", "../../../shaders/lightSource.frag");

        // _models.Add(new Surface(_shader));
        _models.Add(new Earth(_shader));
        _models.Add(new Sun(_shader));

        _sky = new Sky(_shader);
        _models.Add(_sky);

        _ufo = new Ufo(_shader, _lightPointShader);
        _models.Add(_ufo);

        _mirror = new Mirror(_mirrorShader);

        CursorState = CursorState.Grabbed;
        _shader.SetFloat("fogDensity", _fogDensity);
        _shader.SetFloat("specularStrength", _specularStrength);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

        if (KeyboardState.IsKeyDown(Keys.D1)) _cameraType = CameraType.Static;
        if (KeyboardState.IsKeyDown(Keys.D2)) _cameraType = CameraType.Follow;
        if (KeyboardState.IsKeyDown(Keys.D3)) _cameraType = CameraType.Ufo;

        if (KeyboardState.IsKeyDown(Keys.F))
        {
            _fogDensity += 0.005f;
            _sky.FogDensity = _fogDensity;
            _shader.SetFloat("fogDensity", _fogDensity);
        }
        if (KeyboardState.IsKeyDown(Keys.G))
        {
            _fogDensity -= 0.005f;
            _sky.FogDensity = _fogDensity;
            _shader.SetFloat("fogDensity", _fogDensity);
        }
        
        if (KeyboardState.IsKeyDown(Keys.H))
        {
            _specularStrength += 0.005f;
            _shader.SetFloat("specularStrength", _specularStrength);
        }
        if (KeyboardState.IsKeyDown(Keys.J))
        {
            _specularStrength -= 0.005f;
            if (_specularStrength < 0.0f) _specularStrength = 0.0f;
            _shader.SetFloat("specularStrength", _specularStrength);
        }
    }

    private enum CameraType
    {
        Static,
        Follow,
        Ufo
    }

    private CameraType _cameraType = CameraType.Static;

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.VertexProgramPointSize);

        var cameraPosition = new Vector3(0.0f, 1.0f, 3.0f);
        var view = Matrix4.LookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
            (float)ClientSize.X / ClientSize.Y, 0.1f, 100.0f
        );

        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        _shader.SetVector3("viewPos", cameraPosition);

        _shader.SetVector3("dirLight.direction", new Vector3(new Vector4(0.0f, -1.0f, 0.0f, 0.0f) * view));
        _shader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
        _shader.SetVector3("dirLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
        _shader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));


        _mirrorShader.SetMatrix4("view", view);
        _mirrorShader.SetMatrix4("projection", projection);

        _lightPointShader.SetMatrix4("view", view);
        _lightPointShader.SetMatrix4("projection", projection);

        _mirror.SetupMirrorFbo();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }

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
            case CameraType.Ufo:
                cameraPosition = _ufo.Eye;
                view = _ufo.GetUfoView();
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
        if (_cameraType == CameraType.Ufo)
            spotlightDir = GetMouseRayDirection(_mousePosition, projection) - ufoViewPos;
        else
            spotlightDir = new Vector3(new Vector4(-_ufo.Position, 0.0f) * view);
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

        _ufo.GenerateUfoLights(view);

        _mirror.DrawReflection(_shader, _lightPointShader, cameraPosition, _models);

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _shader.Use();
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        _shader.SetVector3("viewPos", cameraPosition);
        _mirrorShader.SetMatrix4("view", view);
        _mirrorShader.SetMatrix4("projection", projection);
        _lightPointShader.SetMatrix4("view", view);
        _lightPointShader.SetMatrix4("projection", projection);

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