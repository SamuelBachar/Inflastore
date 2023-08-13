using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Utils;

static class EmbeddedResource
{
    public static Stream OpenEmbeddedImageStream(string fileName)
    {
        Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
        var stream = assembly.GetManifestResourceStream($"Neminaj.Resources.ImagesEmbedded.{fileName}");

        return stream;
    }
}
