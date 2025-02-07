using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Sphere : Model
{
    private readonly List<float> _vertices = GenerateSphere(64, 0.2f);

    public Sphere(Shader shader) : base(shader)
    {
        Init();
    }

    public override void Draw()
    {
        var model = Matrix4.CreateTranslation(0.0f, 0.2f, 0.0f);
        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }

    protected override float[] GetVertices() => _vertices.ToArray();

    private static List<float> GenerateSphere(int precision, float radius)
    {
        var vertices = new List<float>();

        for (int i = 0; i < precision; i++)
        {
            float theta1 = (float)(i * Math.PI / precision);
            float theta2 = (float)((i + 1) * Math.PI / precision);

            for (int j = 0; j < precision; j++)
            {
                float phi1 = (float)(j * 2 * Math.PI / precision);
                float phi2 = (float)((j + 1) * 2 * Math.PI / precision);

                float x1 = (float)(Math.Sin(theta1) * Math.Cos(phi1)) * radius;
                float y1 = (float)Math.Cos(theta1) * radius;
                float z1 = (float)(Math.Sin(theta1) * Math.Sin(phi1)) * radius;

                float x2 = (float)(Math.Sin(theta2) * Math.Cos(phi1)) * radius;
                float y2 = (float)Math.Cos(theta2) * radius;
                float z2 = (float)(Math.Sin(theta2) * Math.Sin(phi1)) * radius;

                float x3 = (float)(Math.Sin(theta1) * Math.Cos(phi2)) * radius;
                float y3 = (float)Math.Cos(theta1) * radius;
                float z3 = (float)(Math.Sin(theta1) * Math.Sin(phi2)) * radius;

                float x4 = (float)(Math.Sin(theta2) * Math.Cos(phi2)) * radius;
                float y4 = (float)Math.Cos(theta2) * radius;
                float z4 = (float)(Math.Sin(theta2) * Math.Sin(phi2)) * radius;

                float r = (x1 + 1) / 2;
                float g = (y1 + 1) / 2;
                float b = (z1 + 1) / 2;

                // First triangle
                vertices.AddRange(new[] { x1, y1, z1, r, g, b });
                vertices.AddRange(new[] { x2, y2, z2, r, g, b });
                vertices.AddRange(new[] { x3, y3, z3, r, g, b });

                // Second triangle
                vertices.AddRange(new[] { x3, y3, z3, r, g, b });
                vertices.AddRange(new[] { x2, y2, z2, r, g, b });
                vertices.AddRange(new[] { x4, y4, z4, r, g, b });
            }
        }

        return vertices;
    }
}