namespace UFOmation.models;

public static class GeometricPrimitives
{
    public static List<float> GenerateSphere(int precision, float radius)
    {
        var vertices = new List<float>();

        for (var i = 0; i < precision; i++)
        {
            var theta1 = (float)(i * Math.PI / precision);
            var theta2 = (float)((i + 1) * Math.PI / precision);

            for (var j = 0; j < precision; j++)
            {
                var phi1 = (float)(j * 2 * Math.PI / precision);
                var phi2 = (float)((j + 1) * 2 * Math.PI / precision);

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

                var u1 = phi1 / (2 * (float)Math.PI);
                var v1 = theta1 / (float)Math.PI;

                var u2 = phi1 / (2 * (float)Math.PI);
                var v2 = theta2 / (float)Math.PI;

                var u3 = phi2 / (2 * (float)Math.PI);
                var v3 = theta1 / (float)Math.PI;

                var u4 = phi2 / (2 * (float)Math.PI);
                var v4 = theta2 / (float)Math.PI;

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

                vertices.AddRange(new[] { x1, y1, z1, u1, v1, nx1, ny1, nz1 });
                vertices.AddRange(new[] { x2, y2, z2, u2, v2, nx2, ny2, nz2 });
                vertices.AddRange(new[] { x3, y3, z3, u3, v3, nx3, ny3, nz3 });

                vertices.AddRange(new[] { x3, y3, z3, u3, v3, nx3, ny3, nz3 });
                vertices.AddRange(new[] { x2, y2, z2, u2, v2, nx2, ny2, nz2 });
                vertices.AddRange(new[] { x4, y4, z4, u4, v4, nx4, ny4, nz4 });
            }
        }

        return vertices;
    }

    public static readonly float[] Cube =
    [
        // Front face
        -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,

        -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,

        // Back face
        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,

        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
        -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f,

        // Left face
        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, -1.0f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, 1.0f, 1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,

        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, 1.0f, 1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f,

        // Right face
        1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,

        1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
        1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f,

        // Top face
        -1.0f, 1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        1.0f, 1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,

        -1.0f, 1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,

        // Bottom face
        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,

        -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -1.0f, -1.0f, 1.0f, 0.0f, 1.0f, 0.0f, -1.0f, 0.0f
    ];
}