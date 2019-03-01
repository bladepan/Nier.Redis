using System;
using System.IO;
using System.Reflection;

namespace Nier.Redis
{
    public class AssemblyResourceReader
    {
        private readonly Assembly _assembly;

        internal AssemblyResourceReader()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        public AssemblyResourceReader(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string ReadFile(string path)
        {
            Stream stream = _assembly.GetManifestResourceStream(path);
            // returns null when file not found
            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find file by {path}");
            }

            return ReadStream(stream);
        }


        public string ReadFile(Type type, string path)
        {
            Stream stream = _assembly.GetManifestResourceStream(type, path);
            // returns null when file not found
            if (stream == null)
            {
                throw new FileNotFoundException($"Cannot find file by {type.Namespace}.{path}");
            }

            return ReadStream(stream);
        }

        private static string ReadStream(Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}