using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Sun : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(100, 0.5f);

    private readonly Texture _diffuse;
    private readonly Texture _specular;

    public Sun(Shader shader) : base(shader)
    {
        _diffuse = new Texture("../../../textures/sun.jpg");
        _specular = new Texture("../../../textures/black.jpg");
        Init();
    }

    public override void Draw(double time)
    {
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);

        var model = Matrix4.CreateTranslation(0.0f, 10.0f, 0.0f);
        Shader.Use();
        Shader.SetMatrix4("model", model);
        Shader.SetInt("material.diffuse", 0);
        Shader.SetInt("material.specular", 1);

        // Make Sun ignore the lighting
        Shader.SetVector3("dirLight.ambient", Vector3.One);

        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);

        Shader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}