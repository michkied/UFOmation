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
        float[] vertices = GetVertices();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
            BufferUsageHint.StaticDraw
        );

        // _elementBufferObject = GL.GenBuffer();
        // GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        // GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
        //     BufferUsageHint.StaticDraw);

        int attributeLocation = _shader.GetAttribLocation("pos");
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false,
            6 * sizeof(float), 0
        );
        GL.EnableVertexAttribArray(attributeLocation);

        attributeLocation = _shader.GetAttribLocation("color");
        GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
            3 * sizeof(float)
        );
        GL.EnableVertexAttribArray(attributeLocation);
    }

    public virtual void Draw()
    {
        throw new NotImplementedException();
    }

    protected virtual float[] GetVertices() => [];
}