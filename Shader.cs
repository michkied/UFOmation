using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation;

public class Shader
{
    private readonly int _handle;

    private bool _disposedValue;
    private int _fragmentShader;
    private int _vertexShader;

    public Shader(string vertexPath, string fragmentPath)
    {
        CompileVertex(vertexPath);
        CompileFragment(fragmentPath);

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, _vertexShader);
        GL.AttachShader(_handle, _fragmentShader);

        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetProgramInfoLog(_handle);
            Console.WriteLine(infoLog);
        }

        GL.DetachShader(_handle, _vertexShader);
        GL.DetachShader(_handle, _fragmentShader);
        GL.DeleteShader(_fragmentShader);
        GL.DeleteShader(_vertexShader);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(_handle);

            _disposedValue = true;
        }
    }

    ~Shader()
    {
        if (_disposedValue == false) Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(_handle, attribName);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        Use();
        GL.UniformMatrix4(GL.GetUniformLocation(_handle, name), true, ref matrix);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        Use();
        GL.Uniform3(GL.GetUniformLocation(_handle, name), vector);
    }

    private void CompileVertex(string path)
    {
        _vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_vertexShader, File.ReadAllText(path));
        GL.CompileShader(_vertexShader);
        GL.GetShader(_vertexShader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(_vertexShader);
            Console.WriteLine(infoLog);
        }
    }

    private void CompileFragment(string path)
    {
        _fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_fragmentShader, File.ReadAllText(path));
        GL.CompileShader(_fragmentShader);
        GL.GetShader(_fragmentShader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(_fragmentShader);
            Console.WriteLine(infoLog);
        }
    }
}