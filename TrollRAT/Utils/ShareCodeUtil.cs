using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using TrollRAT.Actions;
using TrollRAT.Payloads;

namespace TrollRAT.Utils
{
    static class ShareCodeUtil
    {
        public class ShareCodeWrongVersionException : Exception { }

        // Cause why not?
        public static void obfuscateData(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] hash = md5.ComputeHash(data.Take(i).ToArray());
                    data[i] = (byte)((data[i] + hash[0] + hash[1]) % 256);
                }
            }

            Array.Reverse(data);
        }

        public static void deobfuscateData(byte[] data)
        {
            Array.Reverse(data);

            using (var md5 = MD5.Create())
            {
                byte[] original = data.ToArray();

                for (int i = 0; i < data.Length; i++)
                {
                    byte[] hash = md5.ComputeHash(original.Take(i).ToArray());
                    data[i] = (byte)((data[i] - hash[0] - hash[1]) % 256);
                }
            }
        }

        public static byte[] getPluginsHash()
        {
            using (var md5 = MD5.Create())
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                foreach (Assembly assembly in TrollRAT.pluginManager
                    .plugins.OrderBy(p => p.Name).Select(p => Assembly.GetAssembly(p.GetType()))
                    .Concat(new[] { Assembly.GetExecutingAssembly() }))
                {
                    using (var fstream = File.OpenRead(assembly.Location))
                    {
                        byte[] hash = md5.ComputeHash(fstream);
                        writer.Write(hash);
                    }
                }

                return md5.ComputeHash(stream.ToArray());
            }
        }

        public static void readShareCode(string code)
        {
            byte[] data = Convert.FromBase64String(code);
            
            for (int i = 0; i < 10; i++)
                deobfuscateData(data);

            using (var memstream = new MemoryStream(data))
            {
                using (var stream = new DeflateStream(memstream, CompressionMode.Decompress))
                using (var reader = new BinaryReader(stream))
                {
                    byte[] pluginsHash = getPluginsHash();
                    if (!reader.ReadBytes(pluginsHash.Length).SequenceEqual(pluginsHash))
                        throw new ShareCodeWrongVersionException();

                    foreach (GlobalActionServer action in TrollRAT.Server.Actions.Where(a => a is GlobalActionServer))
                    {
                        action.readFromStream(reader);
                    }

                    foreach (var payload in TrollRAT.Server.Payloads)
                    {
                        payload.readFromStream(reader);
                    }
                }
            }
        }

        public static string createShareCode()
        {
            using (var memstream = new MemoryStream())
            {
                using (var stream = new DeflateStream(memstream, CompressionMode.Compress))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(getPluginsHash());

                    foreach (GlobalActionServer action in TrollRAT.Server.Actions.Where(a => a is GlobalActionServer))
                    {
                        action.writeToStream(writer);
                    }

                    foreach (Payload payload in TrollRAT.Server.Payloads)
                    {
                        payload.writeToStream(writer);
                    }
                }

                byte[] data = memstream.ToArray();
                for (int i = 0; i < 10; i++)
                    obfuscateData(data);

                return Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks);
            }
        }
    }
}
