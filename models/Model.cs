using OpenTK.Graphics.OpenGL4;

namespace UFOmation.models;

public abstract class Model(Shader shader)
{
    protected readonly Shader Shader = shader;

    protected int VertexArrayObject;
    protected int VertexBufferObject;

    protected void Init()
    {
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        var vertices = GetVertices();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
            BufferUsageHint.StaticDraw
        );

        var attributeLocation = Shader.GetAttribLocation("pos");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false,
            8 * sizeof(float), 0
        );

        attributeLocation = Shader.GetAttribLocation("tex");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            3 * sizeof(float)
        );

        attributeLocation = Shader.GetAttribLocation("normal");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            5 * sizeof(float)
        );
    }

    public virtual void Draw(double time)
    {
        throw new NotImplementedException();
    }

    protected virtual float[] GetVertices()
    {
        return [];
    }
}