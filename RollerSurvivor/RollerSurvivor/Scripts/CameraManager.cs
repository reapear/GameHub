using Raylib_cs;
using System.Numerics;
using RollerSurvivor.Scripts.Framework;

public class CameraManager : Singleton<CameraManager>
{
    public Camera2D Camera { get; private set; }

    public CameraManager()
    {
        Camera = new Camera2D
        {
            Target = new Vector2(0, 0),
            Offset = new Vector2(400, 300),
            Rotation = 0.0f,
            Zoom = 1.0f
        };
    }

    public void Update(Vector2 targetPosition)
    {
        var tempCamera = Camera;
        tempCamera.Target = targetPosition;
        Camera = tempCamera;
    }

    public void SetZoom(float zoom)
    {
        var tempCamera = Camera;
        tempCamera.Zoom = zoom;
        Camera = tempCamera;
    }

    public void ModifyZoom(float delta)
    {
        var tempCamera = Camera;
        tempCamera.Zoom += delta;
        Camera = tempCamera;
    }
}
