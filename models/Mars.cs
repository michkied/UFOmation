using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Mars : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(100, 0.3f);

    private readonly Texture _diffuse;
    private readonly Texture _specular;
    
    public Mars(Shader shader) : base(shader)
    {
        _diffuse = new Texture("../../../textures/mars.jpg");
        _specular = new Texture("../../../textures/mars.jpg");
        Init();
    }

    public override void Draw(double time)
    {
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);
        
        Shader.Use();
        Shader.SetInt("material.diffuse", 0);
        Shader.SetInt("material.specular", 1);
        var model = Matrix4.CreateTranslation(-1.5f, 0.5f, 0.5f);
        Shader.SetMatrix4("model", model);
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}