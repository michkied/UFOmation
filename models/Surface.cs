using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Surface : Model
{
    private Texture _texture;

    public Surface(Shader shader) : base(shader)
    {
        _texture = new Texture("../../../textures/dirt/dirt.jpg");
        Init();
    }

    public override void Draw()
    {
        _shader.SetMatrix4("model", Matrix4.Identity);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected override float[] GetVertices()
    {
        return _vertices;
    }

    private readonly float[] _vertices =
    [
        // Front face
        -1.0f, -0.2f, 1.0f, 0.0f, 0.0f, // Bottom-left
        1.0f, -0.2f, 1.0f, 1.0f, 0.0f, // Bottom-right
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f, // Top-right

        -1.0f, -0.2f, 1.0f, 0.0f, 0.0f, // Bottom-left
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f, // Top-right
        -1.0f, 0.0f, 1.0f, 0.0f, 1.0f, // Top-left

        // Back face
        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, -0.2f, -1.0f, 1.0f, 0.0f,
        1.0f, 0.0f, -1.0f, 1.0f, 1.0f,

        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        -1.0f, 0.0f, -1.0f, 0.0f, 1.0f,

        // Left face
        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        -1.0f, -0.2f, 1.0f, 1.0f, 0.0f,
        -1.0f, 0.0f, 1.0f, 1.0f, 1.0f,

        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        -1.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        -1.0f, 0.0f, -1.0f, 0.0f, 1.0f,

        // Right face
        1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, -0.2f, 1.0f, 1.0f, 0.0f,
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f,

        1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 0.0f, -1.0f, 0.0f, 1.0f,

        // Top face
        -1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f,

        -1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        -1.0f, 0.0f, 1.0f, 0.0f, 1.0f,

        // Bottom face
        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, -0.2f, -1.0f, 1.0f, 0.0f,
        1.0f, -0.2f, 1.0f, 1.0f, 1.0f,

        -1.0f, -0.2f, -1.0f, 0.0f, 0.0f,
        1.0f, -0.2f, 1.0f, 1.0f, 1.0f,
        -1.0f, -0.2f, 1.0f, 0.0f, 1.0f
    ];
}