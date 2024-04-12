using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ReRoExtension.Helper
{
    public static class ReflectionHelper
    {
        public static string GetEmbeddedResource(
            Assembly assembly,
            string resourceName
            )
        {
            using (var source = assembly.GetManifestResourceStream(resourceName))
            {
                var body = new byte[source.Length];
                source.Read(body, 0, body.Length);

                var index = 0;
                if (body[0] == 0xEF && body[1] == 0xBB && body[2] == 0xBF) //cut BOM
                {
                    index = 3;
                }

                var result = Encoding.UTF8.GetString(body, index, body.Length - index);
                return result;
            }
        }

        public static void ExtractEmbeddedResource(
            Assembly assembly,
            string fullPath,
            string resourceName
            )
        {
            if (!File.Exists(fullPath))
            {
                using (var target = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var source = assembly.GetManifestResourceStream(resourceName))
                {
                    source.CopyTo(target);

                    target.Flush();
                }
            }
        }

    }
}
