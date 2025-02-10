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
    private readonly float _radius = 0.7f; // Distance from the origin
    private readonly float _rotSpeed = 1.5f; // Rotation speed
    private readonly float _moveSpeed = 0.5f; // Movement speed

    public override void Draw(double time)
    {
        _texture.Use();
        _shader.Use();
        var model = Matrix4.Identity;

        _rotAngle += (float)time * _rotSpeed;
        _moveAngle += (float)time * _moveSpeed;
        model *= Matrix4.CreateScale(0.1f, 0.01f, 0.1f);
        model *= Matrix4.CreateRotationY(_rotAngle);
        model *= Matrix4.CreateRotationZ(_moveAngle + (float)Math.PI / 2);
        model *= Matrix4.CreateTranslation(
            _radius * (float)Math.Cos(_moveAngle),
            _radius * (float)Math.Sin(_moveAngle),
            0.0f
        );
        // model *= Matrix4.CreateScale(0.2f, 0.2f, 0.2f) * Matrix4.CreateTranslation(0.6f, 0.2f, 0.4f);

        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected override float[] GetVertices()
    {
        return Cube.Vertices;
    }
}