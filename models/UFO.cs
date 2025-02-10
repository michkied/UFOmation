using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Ufo : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(64, Radius);
    private readonly List<float> _lightPoints = GenerateLightPoints();

    private readonly Texture _diffuse;
    private readonly Texture _specular;

    private static readonly float Radius = 0.05f;
    private const int PointCount = 6;

    private float _rotAngle;
    private readonly float _rotRadius = 0.6f;
    private readonly float _rotSpeed = 1.5f;

    private float _moveAngle;
    private readonly float _moveSpeed = 0.2f;

    private Vector3 _position = new(0.0f, 0.0f, 0.0f);
    private Vector3 _eye = new(0.0f, 0.0f, 0.0f);
    private readonly float _eyeHeight = 0.5f;
    private readonly float _eyeDistance = 0.5f;

    private readonly Shader _lightPointShader;
    private int _lightPointVertexArrayObject;
    private int _lightPointVertexBufferObject;

    public Vector3 Position => _position;
    public Vector3 Eye => _eye;

    public Ufo(Shader mainShader, Shader lightPointShader) : base(mainShader)
    {
        _lightPointShader = lightPointShader;
        _diffuse = new Texture("../../../textures/metal/metal.jpg");
        _specular = new Texture("../../../textures/metal/metal_specular.jpg");
        Init();
        InitLightPoints();
    }

    private void InitLightPoints()
    {
        _lightPointVertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_lightPointVertexArrayObject);

        _lightPointVertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _lightPointVertexBufferObject);

        var vertices = GetVertices();
        GL.BufferData(BufferTarget.ArrayBuffer, _lightPoints.Count * sizeof(float), _lightPoints.ToArray(),
            BufferUsageHint.StaticDraw
        );
        var attributeLocation = _lightPointShader.GetAttribLocation("pos");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false,
            6 * sizeof(float), 0
        );

        attributeLocation = _lightPointShader.GetAttribLocation("col");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
            3 * sizeof(float)
        );
    }

    private static List<float> GenerateLightPoints()
    {
        var lightPoints = new List<float>();
        var angleStep = 360f / PointCount;

        for (var i = 0; i < PointCount; i++)
        {
            var angleRadians = float.DegreesToRadians(i * angleStep);
            lightPoints.AddRange(new[]
            {
                Radius * (float)Math.Cos(angleRadians),
                0,
                Radius * (float)Math.Sin(angleRadians),
                1.0f, 0.0f, 0.0f
            });
        }

        return lightPoints;
    }

    public override void Draw(double time)
    {
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);
        Shader.Use();
        var model = Matrix4.Identity;

        _rotAngle += (float)time * _rotSpeed;
        _moveAngle += (float)time * _moveSpeed;

        _position = new Vector3(
            _rotRadius * (float)Math.Cos(_moveAngle),
            _rotRadius * (float)Math.Sin(_moveAngle),
            0.0f
        );

        model *= Matrix4.CreateScale(1.0f, 0.15f, 1.0f);
        model *= Matrix4.CreateRotationY(_rotAngle);
        model *= Matrix4.CreateRotationZ(_moveAngle + (float)Math.PI / 2);
        model *= Matrix4.CreateTranslation(
            _position.X,
            _position.Y,
            0.0f
        );

        _eye = new Vector3(
            (_rotRadius + _eyeHeight) * (float)Math.Cos(_moveAngle - _eyeDistance),
            (_rotRadius + _eyeHeight) * (float)Math.Sin(_moveAngle - _eyeDistance),
            0.0f
        );
        // model *= Matrix4.CreateScale(0.2f, 0.2f, 0.2f) * Matrix4.CreateTranslation(0.6f, 0.2f, 0.4f);

        _model = model;

        Shader.SetMatrix4("model", model);
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);

        _lightPointShader.Use();
        _lightPointShader.SetMatrix4("model", model);
        GL.BindVertexArray(_lightPointVertexArrayObject);
        GL.PointSize(7);
        GL.DrawArrays(PrimitiveType.Points, 0, _lightPoints.Count);
        GL.PointSize(1);
    }

    private Matrix4 _model;

    public void GenerateUfoLights(Matrix4 view)
    {
        for (var i = 0; i < PointCount; i++)
        {
            var absolutePosition = new Vector3(_lightPoints[i * 6], _lightPoints[i * 6 + 1], _lightPoints[i * 6 + 2]);
            var modelPosition = new Vector3(new Vector4(absolutePosition, 1.0f) * _model);
            var viewPosition = new Vector3(new Vector4(modelPosition, 1.0f) * view);

            var directionNudge = (modelPosition - _position) * 5f;
            var viewDirection = new Vector3(new Vector4(-modelPosition + directionNudge, 0.0f) * view);

            Shader.SetVector3($"ufoLights[{i}].position", viewPosition);
            Shader.SetVector3($"ufoLights[{i}].direction", viewDirection);
            Shader.SetFloat($"ufoLights[{i}].cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(10.0f)));
            Shader.SetFloat($"ufoLights[{i}].outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(15.0f)));
            Shader.SetVector3($"ufoLights[{i}].ambient", new Vector3(0.0f, 0.0f, 0.0f));
            Shader.SetVector3($"ufoLights[{i}].diffuse", new Vector3(1.0f, 0.0f, 0.0f));
            Shader.SetVector3($"ufoLights[{i}].specular", new Vector3(1.0f, 0.0f, 0.0f));
            Shader.SetFloat($"ufoLights[{i}].constant", 1.0f);
            Shader.SetFloat($"ufoLights[{i}].linear", 0.09f);
            Shader.SetFloat($"ufoLights[{i}].quadratic", 0.032f);
        }
    }

    public Matrix4 GetUfoView()
    {
        var up = Vector3.UnitY;
        if (_eye.X - _position.X < 0.0f) up = -Vector3.UnitY;
        return Matrix4.LookAt(_eye, _position, up);
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}