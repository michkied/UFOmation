using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class UFO : Model
{
    private readonly Texture _texture;

    public UFO(Shader shader) : base(shader)
    {
        _texture = new Texture("../../../textures/dirt/dirt.jpg");
        Init();
    }

    private float _rotAngle;
    private float _moveAngle;
    private readonly float _radius = 0.6f; // Distance from the origin
    private readonly float _rotSpeed = 1.5f; // Rotation speed
    private readonly float _moveSpeed = 0.2f; // Movement speed

    private Vector3 _position = new(0.0f, 0.0f, 0.0f);
    private Vector3 _eye = new(0.0f, 0.0f, 0.0f);
    private readonly float _eyeHeight = 0.5f;
    private readonly float _eyeDistance = 0.5f;

    public Vector3 Position => _position;
    public Vector3 Eye => _eye;

    public override void Draw(double time)
    {
        _texture.Use();
        _texture.Use(TextureUnit.Texture1);
        _shader.Use();
        var model = Matrix4.Identity;

        _rotAngle += (float)time * _rotSpeed;
        _moveAngle += (float)time * _moveSpeed;
        model *= Matrix4.CreateScale(0.05f, 0.005f, 0.05f);
        model *= Matrix4.CreateRotationY(_rotAngle);
        model *= Matrix4.CreateRotationZ(_moveAngle + (float)Math.PI / 2);
        model *= Matrix4.CreateTranslation(
            _radius * (float)Math.Cos(_moveAngle),
            _radius * (float)Math.Sin(_moveAngle),
            0.0f
        );
        _position = new Vector3(
            _radius * (float)Math.Cos(_moveAngle),
            _radius * (float)Math.Sin(_moveAngle),
            0.0f
        );

        _eye = new Vector3(
            (_radius + _eyeHeight) * (float)Math.Cos(_moveAngle - _eyeDistance),
            (_radius + _eyeHeight) * (float)Math.Sin(_moveAngle - _eyeDistance),
            0.0f
        );
        // model *= Matrix4.CreateScale(0.2f, 0.2f, 0.2f) * Matrix4.CreateTranslation(0.6f, 0.2f, 0.4f);

        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public Matrix4 GetUFOView()
    {
        var up = Vector3.UnitY;
        if (_eye.X - _position.X < 0.0f) up = -Vector3.UnitY;
        return Matrix4.LookAt(_eye, _position, up);
    }

    protected override float[] GetVertices()
    {
        return Cube.Vertices;
    }
}