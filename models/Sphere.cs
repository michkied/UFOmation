using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Sphere : Model
{
    private readonly List<float> _vertices = GenerateSphere(64, 0.5f);

    public Sphere(Shader shader) : base(shader)
    {
        Init();
    }

    public override void Draw(double time)
    {
        // var model = Matrix4.CreateTranslation(-1.0f, 0.2f, 1.0f);
        var model = Matrix4.Identity;
        _shader.SetMatrix4("model", model);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }

    protected override float[] GetVertices()
    {
        return _vertices.ToArray();
    }

    private static List<float> GenerateSphere(int precision, float radius)
    {
        var vertices = new List<float>();

        for (var i = 0; i < precision; i++)
        {
            // theta ranges from 0 to PI.
            var theta1 = (float)(i * Math.PI / precision);
            var theta2 = (float)((i + 1) * Math.PI / precision);

            for (var j = 0; j < precision; j++)
            {
                // phi ranges from 0 to 2*PI.
                var phi1 = (float)(j * 2 * Math.PI / precision);
                var phi2 = (float)((j + 1) * 2 * Math.PI / precision);

                // Compute the four vertex positions.
                var x1 = (float)(Math.Sin(theta1) * Math.Cos(phi1)) * radius;
                var y1 = (float)Math.Cos(theta1) * radius;
                var z1 = (float)(Math.Sin(theta1) * Math.Sin(phi1)) * radius;

                var x2 = (float)(Math.Sin(theta2) * Math.Cos(phi1)) * radius;
                var y2 = (float)Math.Cos(theta2) * radius;
                var z2 = (float)(Math.Sin(theta2) * Math.Sin(phi1)) * radius;

                var x3 = (float)(Math.Sin(theta1) * Math.Cos(phi2)) * radius;
                var y3 = (float)Math.Cos(theta1) * radius;
                var z3 = (float)(Math.Sin(theta1) * Math.Sin(phi2)) * radius;

                var x4 = (float)(Math.Sin(theta2) * Math.Cos(phi2)) * radius;
                var y4 = (float)Math.Cos(theta2) * radius;
                var z4 = (float)(Math.Sin(theta2) * Math.Sin(phi2)) * radius;

                // Compute texture coordinates for each vertex.
                // u = phi / (2π), v = theta / π.
                var u1 = phi1 / (2 * (float)Math.PI);
                var v1 = theta1 / (float)Math.PI;

                var u2 = phi1 / (2 * (float)Math.PI);
                var v2 = theta2 / (float)Math.PI;

                var u3 = phi2 / (2 * (float)Math.PI);
                var v3 = theta1 / (float)Math.PI;

                var u4 = phi2 / (2 * (float)Math.PI);
                var v4 = theta2 / (float)Math.PI;

                // Compute normals by normalizing the position (since the sphere is centered at the origin).
                var nx1 = x1 / radius;
                var ny1 = y1 / radius;
                var nz1 = z1 / radius;

                var nx2 = x2 / radius;
                var ny2 = y2 / radius;
                var nz2 = z2 / radius;

                var nx3 = x3 / radius;
                var ny3 = y3 / radius;
                var nz3 = z3 / radius;

                var nx4 = x4 / radius;
                var ny4 = y4 / radius;
                var nz4 = z4 / radius;

                // First triangle (vertices: 1, 2, 3)
                vertices.AddRange(new[] { x1, y1, z1, u1, v1, nx1, ny1, nz1 });
                vertices.AddRange(new[] { x2, y2, z2, u2, v2, nx2, ny2, nz2 });
                vertices.AddRange(new[] { x3, y3, z3, u3, v3, nx3, ny3, nz3 });

                // Second triangle (vertices: 3, 2, 4)
                vertices.AddRange(new[] { x3, y3, z3, u3, v3, nx3, ny3, nz3 });
                vertices.AddRange(new[] { x2, y2, z2, u2, v2, nx2, ny2, nz2 });
                vertices.AddRange(new[] { x4, y4, z4, u4, v4, nx4, ny4, nz4 });
            }
        }

        return vertices;
    }
}