using OpenTK.Mathematics;

namespace SharpCanvas
{
    public interface IMaterial
    {
        bool Scatter(Ray ray, HitRecord record, ref Color4 attenuation, ref Ray scattered);
    }
}