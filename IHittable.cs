namespace SharpCanvas
{
    public interface IHittable 
    {
        bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record);
        bool HitBoundingBox(float time0, float time1, out AxisAlignedBoundingBox output);
    }
}