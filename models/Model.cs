using OpenTK.Graphics.OpenGL4;

namespace UFOmation.models;

public abstract class Model(Shader shader)
{
    protected readonly Shader _shader = shader;

    // private int _elementBufferObject;
    protected int _vertexArrayObject;
    protected int _vertexBufferObject;

    public void Init()
    {
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        var vertices = GetVertices();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
            BufferUsageHint.StaticDraw
        );

        // _elementBufferObject = GL.GenBuffer();
        // GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        // GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
        //     BufferUsageHint.StaticDraw);

        var attributeLocation = _shader.GetAttribLocation("pos");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false,
            8 * sizeof(float), 0
        );

        attributeLocation = _shader.GetAttribLocation("tex");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            3 * sizeof(float)
        );

        attributeLocation = _shader.GetAttribLocation("normal");
        GL.EnableVertexAttribArray(attributeLocation);
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            5 * sizeof(float)
        );
    }

    public virtual void Draw()
    {
        throw new NotImplementedException();
    }

    protected virtual float[] GetVertices()
    {
        return [];
    }
}