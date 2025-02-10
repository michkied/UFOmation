using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Sky : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(100, 15.0f);

    private readonly Texture _diffuse;
    private readonly Texture _specular;

    public float FogDensity;

    public Sky(Shader shader) : base(shader)
    {
        _diffuse = new Texture("../../../textures/sky.jpg");
        _specular = new Texture("../../../textures/sky.jpg");
        Init();
    }

    public override void Draw(double time)
    {
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);

        Shader.Use();
        Shader.SetInt("material.diffuse", 0);
        Shader.SetInt("material.specular", 1);
        Shader.SetVector3("dirLight.ambient", Vector3.One);
        Shader.SetFloat("fogDensity", 0);
        var model = Matrix4.Identity;
        Shader.SetMatrix4("model", model);
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
        Shader.SetFloat("fogDensity", FogDensity);
        Shader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}