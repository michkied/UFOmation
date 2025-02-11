using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Earth : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(100, 0.5f);

    private readonly Texture _diffuse;
    private readonly Texture _specular;

    public Earth(Shader shader) : base(shader)
    {
        _diffuse = new Texture("../../../textures/earth.jpg");
        _specular = new Texture("../../../textures/earth_specular.jpg");
        Init();
    }

    public override void Draw(double time)
    {
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);

        Shader.Use();
        Shader.SetInt("material.diffuse", 0);
        Shader.SetInt("material.specular", 1);
        Shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        Shader.SetFloat("material.shininess", 32.0f);
        Shader.SetMatrix4("model", Matrix4.Identity);
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}