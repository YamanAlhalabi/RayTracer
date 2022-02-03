using OpenTK.Mathematics;

namespace SharpCanvas 
{
    public class SceneGenerator
    {
        public static HittableList GenerateTwoSphere()
        {
            var world = new HittableList();
            var checker = new Lambertian(new CheckerTexture(new RandomColorTexture(), Color4.White));

            world.Add(new Sphere(new Vector3(0f, -10f, 0), 10f, checker));
            world.Add(new Sphere(new Vector3(0f, +10f, 0), 10f, checker));
            
            return world;
        }

        public static HittableList GenerateBouncingMarbles()
        {
            var world = new HittableList();

            var groundMaterial = new Lambertian(new CheckerTexture(Color4.BurlyWood, Color4.White));
            world.Add(new Sphere(new Vector3(0f, -1000f, 0), 1000, groundMaterial));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMat = Helper.RandomFloat(0, 1);
                    var center = new Vector3(a + 0.9f * Helper.RandomFloat(0, 1), 0.2f, b + 0.9f * Helper.RandomFloat(0, 1));
                
                    if ((center - new Vector3(4f, 0.2f, 0)).Length > 0.9f)
                    {
                        IMaterial sphereMaterial;

                        if (chooseMat > 0.8)
                        {
                            var albedo = Helper.RandomColor();
                            
                            sphereMaterial = new Lambertian(albedo);

                            var center2 = center + new Vector3(0f, Helper.RandomFloat(0, .5f), 0f);
                            world.Add(new MovingSphere(center, center2, 0.0f, 1.0f, 0.2f, sphereMaterial));
                        }
                        else if (chooseMat < 0.95)
                        {
                            var albedo = Helper.RandomColor(0.5f, 1.0f);
                            var fuzz = Helper.RandomFloat(0.0f, 0.5f);

                            sphereMaterial = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                        else 
                        {
                            sphereMaterial = new Dielectric(1.5f);
                            world.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                    }
                }
            }

            var material1 = new Dielectric(1.333f);
            world.Add(new Sphere(new Vector3(0f, 1f, 0f), 1.0f, material1));

            var material2 = new Lambertian(new CheckerTexture(new RandomColorTexture(), Color4.Black));
            world.Add(new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, material2));
            
            var material3 = new Metal(new Color4(0.7f, 0.6f, 0.5f, 1f), 0f);
            world.Add(new Sphere(new Vector3(4f, 1f, 0f), 1.0f, material3));

            return world;
        } 
    }
}