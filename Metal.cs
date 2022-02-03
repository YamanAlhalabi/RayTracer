using System;
using OpenTK.Mathematics;

namespace SharpCanvas
{
    public class Metal : IMaterial
    {
        public Metal(Color4 albedo, float fuzz)
        {
            Fuzz = fuzz;
            Albedo = albedo;
        }

        public float Fuzz { get; set; }
        public Color4 Albedo { get; set; }

        public bool Scatter(Ray ray, HitRecord record, ref Color4 attenuation, ref Ray scattered)
        {
            Vector3 reflected = Helper.Reflect(ray.Direction.Normalized(), record.normal);
            scattered = new Ray(record.p, reflected + Fuzz * Helper.RandomInUnitSphere(), ray.Time);
            attenuation = Albedo;
            return (Vector3.Dot(scattered.Direction, record.normal) > 0);
        }
    }
}