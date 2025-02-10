using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Sphere : Model
{
    private readonly List<float> _vertices = GeometricPrimitives.GenerateSphere(64, 0.5f);

    private readonly Texture _diffuse;
    private readonly Texture _specular;

    public Sphere(Shader shader) : base(shader)
    {
        _diffuse = new Texture("../../../textures/earth/earth.jpg");
        _specular = new Texture("../../../textures/earth/earth_specular.jpg");
        Init();
    }

    public override void Draw(double time)
    {
        // var model = Matrix4.CreateTranslation(-1.0f, 0.2f, 1.0f);
        _diffuse.Use();
        _specular.Use(TextureUnit.Texture1);

        _shader.Use();
        _shader.SetInt("material.diffuse", 0);
        _shader.SetInt("material.specular", 1);
        _shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        _shader.SetFloat("material.shininess", 32.0f);
        // var model = Matrix4.CreateRotationX(float.DegreesToRadians(-90.0f));
        var model = Matrix4.Identity;
        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }
}