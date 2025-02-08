using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class UFO : Model
{
    private Texture _texture;

    public UFO(Shader shader) : base(shader)
    {
        _texture = new Texture("../../../textures/dirt/dirt.jpg");
        Init();
    }

    public override void Draw()
    {
        var model = Matrix4.CreateTranslation(0.0f, 0.2f, 0.0f) * Matrix4.CreateScale(0.5f, 0.1f, 0.5f);
        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    protected override float[] GetVertices()
    {
        return Cube.Vertices;
    }
}