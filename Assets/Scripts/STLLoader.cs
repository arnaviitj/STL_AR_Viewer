using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Dummiesman
{
    public class STLLoader
    {
        private static bool IsBinary(byte[] data)
        {
            if (data.Length < 84) return false;
            uint triCount = BitConverter.ToUInt32(data, 80);
            long expected = 84 + triCount * 50;
            return data.Length == expected;
        }

        private GameObject LoadBinary(byte[] data)
        {
            uint triCount = BitConverter.ToUInt32(data, 80);
            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();
            int offset = 84;

            for (int i = 0; i < triCount; i++)
            {
                offset += 12;
                for (int v = 0; v < 3; v++)
                {
                    float x = BitConverter.ToSingle(data, offset);
                    float y = BitConverter.ToSingle(data, offset + 4);
                    float z = BitConverter.ToSingle(data, offset + 8);
                    verts.Add(new Vector3(x, y, z));
                    indices.Add(verts.Count - 1);
                    offset += 12;
                }
                offset += 2;
            }
            return BuildGameObject(verts, indices);
        }

        private GameObject LoadASCII(byte[] data)
        {
            string text = Encoding.UTF8.GetString(data);
            string[] lines = text.Split('\n');
            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!trimmed.StartsWith("vertex")) continue;
                string[] parts = trimmed.Split(
                    new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 4) continue;
                float x = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                verts.Add(new Vector3(x, y, z));
                indices.Add(verts.Count - 1);
            }
            return BuildGameObject(verts, indices);
        }

        private GameObject BuildGameObject(List<Vector3> vertices, List<int> indices)
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = vertices.Count > 65535
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16;

            // Convert STL right-hand coords to Unity left-hand coords
            Vector3[] verts = vertices.ToArray();
            for (int i = 0; i < verts.Length; i++)
                verts[i] = new Vector3(verts[i].x, verts[i].z, verts[i].y);

            // Flip winding order for Unity
            int[] tris = indices.ToArray();
            for (int i = 0; i < tris.Length; i += 3)
            {
                int tmp = tris[i];
                tris[i] = tris[i + 2];
                tris[i + 2] = tmp;
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GameObject obj = new GameObject("STLModel");
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>();
            return obj;
        }

        public GameObject Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("[STLLoader] File not found: " + path);
                return null;
            }
            byte[] data = File.ReadAllBytes(path);
            return IsBinary(data) ? LoadBinary(data) : LoadASCII(data);
        }
    }
}