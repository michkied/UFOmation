using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace UFOmation.models;

public class Mirror : Model
{
    private readonly Texture _texture;

    private int _mirrorFbo;
    private int _mirrorTexture;
    private int _mirrorDepthBuffer;
    private const int MirrorTexWidth = 1024;
    private const int MirrorTexHeight = 1024;

    public Mirror(Shader shader) : base(shader)
    {
        _texture = new Texture("../../../textures/dirt/dirt.jpg");
        Init();
    }

    public void SetupMirrorFbo()
    {
        _mirrorFbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _mirrorFbo);

        _mirrorTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _mirrorTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            MirrorTexWidth, MirrorTexHeight, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, _mirrorTexture, 0);

        _mirrorDepthBuffer = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _mirrorDepthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24,
            MirrorTexWidth, MirrorTexHeight);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
            RenderbufferTarget.Renderbuffer, _mirrorDepthBuffer);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            throw new Exception("Mirror Framebuffer not complete!");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void DrawReflection(Shader globalShader, Shader lightPointShader, Vector3 cameraPosition, List<Model> models)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _mirrorFbo);
        GL.Viewport(0, 0, MirrorTexWidth, MirrorTexHeight);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var fboProjection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(14.0f),
            MirrorTexWidth / (float)MirrorTexHeight,
            0.1f,
            100.0f);

        var mirrorCamPos = new Vector3(cameraPosition.X, cameraPosition.Y, -2 - cameraPosition.Z);
        var mirrorTarget = new Vector3(0, 0.5f, -1);
        var mirrorView = Matrix4.LookAt(mirrorCamPos, mirrorTarget, Vector3.UnitY);

        globalShader.SetMatrix4("view", mirrorView);
        globalShader.SetMatrix4("projection", fboProjection);
        globalShader.SetVector3("viewPos", mirrorCamPos);
        Shader.SetMatrix4("view", mirrorView);
        Shader.SetMatrix4("projection", fboProjection);
        lightPointShader.SetMatrix4("view", mirrorView);
        lightPointShader.SetMatrix4("projection", fboProjection);

        foreach (var model in models)
            model.Draw(0);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public override void Draw(double time)
    {
        _texture.Use();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _mirrorTexture);
        Shader.Use();
        Shader.SetMatrix4("model", Matrix4.Identity);
        GL.BindVertexArray(VertexArrayObject);
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