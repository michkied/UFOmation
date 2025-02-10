using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Mirror : Model
{
    private readonly Texture _texture;

    public int _mirrorFBO;
    public int _mirrorTexture;
    public int _mirrorDepthBuffer;
    public readonly int _mirrorTexWidth = 1024;
    public readonly int _mirrorTexHeight = 1024;

    public Mirror(Shader shader) : base(shader)
    {
        _texture = new Texture("../../../textures/dirt/dirt.jpg");
        Init();
    }

    public void SetupMirrorFBO()
    {
        _mirrorFBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _mirrorFBO);

        // Create the color texture.
        _mirrorTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _mirrorTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            _mirrorTexWidth, _mirrorTexHeight, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, _mirrorTexture, 0);

        // Create and attach a renderbuffer for depth.
        _mirrorDepthBuffer = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _mirrorDepthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24,
            _mirrorTexWidth, _mirrorTexHeight);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
            RenderbufferTarget.Renderbuffer, _mirrorDepthBuffer);

        // Check for completeness.
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            throw new Exception("Mirror Framebuffer not complete!");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void DrawReflection(Shader globalShader, Vector3 cameraPosition, List<Model> models)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _mirrorFBO);
        GL.Viewport(0, 0, _mirrorTexWidth, _mirrorTexHeight);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var fboProjection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(14.0f),
            _mirrorTexWidth / (float)_mirrorTexHeight,
            0.1f,
            100.0f);


        // fboProjection = Matrix4.CreateOrthographic(1, 1,
        //     0.1f,
        //     100.0f);


        var mirrorCamPos = new Vector3(cameraPosition.X, cameraPosition.Y, -2 - cameraPosition.Z);
        var mirrorTarget = new Vector3(0, 0.5f, -1);
        var mirrorView = Matrix4.LookAt(mirrorCamPos, mirrorTarget, Vector3.UnitY);

        globalShader.SetMatrix4("view", mirrorView);
        globalShader.SetMatrix4("projection", fboProjection);
        globalShader.SetVector3("viewPos", mirrorCamPos);
        _shader.SetMatrix4("view", mirrorView);
        _shader.SetMatrix4("projection", fboProjection);

        // GL.ColorMask(false, false, false, false);
        // _mirror.Draw();
        // GL.ColorMask(true, true, true, true);

        foreach (var model in models)
            model.Draw(0);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public override void Draw(double time)
    {
        // var model = Matrix4.CreateTranslation(0.0f, 0.2f, 0.0f) * Matrix4.CreateScale(0.5f, 0.1f, 0.5f);
        _texture.Use();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _mirrorTexture);
        _shader.Use();
        _shader.SetMatrix4("model", Matrix4.Identity);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    protected override float[] GetVertices()
    {
        return new[]
        {
            -0.5f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,

            -0.5f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, 1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f
        };
    }
}