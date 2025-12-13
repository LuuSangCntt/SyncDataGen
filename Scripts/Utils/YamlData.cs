using System;
using System.IO;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[Serializable]
public class YamlData<T> where T : YamlData<T>
{
    //Chú ý: Yaml chỉ hoạt động tốt với Properties, không hoạt động với các trường (field) thông thường.

    public static void Save(T data, string path)
    {
        try
        {
            var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) // đặt tên theo camelCase
            .WithTypeConverter(new Vector3YamlConverter())
            //.WithTypeConverter(new QuaternionYamlConverter(useDoublePrecision: false))
            .Build();

            string yamlString = serializer.Serialize(data);
            File.WriteAllText(path, yamlString);
            Debug.Log("YAML save succeeded. \n" + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("YAML save failed: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public static T Load(string fullPath)
    {
        try
        {
            string yamlString = File.ReadAllText(fullPath);
            var input = new StringReader(yamlString);

            //Debug.Log($"YAML load \n{yamlString}");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new Vector3YamlConverter())
                //.WithTypeConverter(new QuaternionYamlConverter(useDoublePrecision: false))
                .IgnoreUnmatchedProperties()
                .IgnoreUnmatchedProperties()
                .Build();
            //Debug.Log($"YAML deserializer created for {typeof(T)}");
            T config = deserializer.Deserialize<T>(input);
            Debug.Log($"YAML load {typeof(T)} succeeded. \n{fullPath}");
            return config;
        }
        catch (Exception ex)
        {
            Debug.LogError("YAML load failed: " + ex.Message + "\n" + ex.StackTrace);
            return null;
        }
    }
}



public class Vector3YamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(Vector3);

    // Dùng cho Deserialization
    public object ReadYaml(IParser parser, Type type, ObjectDeserializer nestedObjectDeserializer)
    {
        // Deserialize về 1 object tạm (ẩn danh có x,y,z)
        var temp = nestedObjectDeserializer(typeof(Vector3Proxy)) as Vector3Proxy;
        return new Vector3(temp.x, temp.y, temp.z);
    }

    // Dùng cho Serialization
    public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer nestedObjectSerializer)
    {
        var v = (Vector3)value;
        nestedObjectSerializer(new Vector3Proxy { x = v.x, y = v.y, z = v.z });
    }

    // proxy class để giữ dữ liệu YAML (tránh serialize thêm magnitude, normalized...)
    private class Vector3Proxy
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}

//public class QuaternionYamlConverter : IYamlTypeConverter
//{
//    private readonly bool useDouble;

//    public QuaternionYamlConverter(bool useDoublePrecision = false)
//    {
//        useDouble = useDoublePrecision;
//    }

//    public bool Accepts(Type type) => type == typeof(Quaternion);

//    public object ReadYaml(IParser parser, Type type, ObjectDeserializer nestedObjectDeserializer)
//    {
//        var arr = nestedObjectDeserializer(typeof(float[])) as float[];
//        if (arr == null || arr.Length < 4)
//            return Quaternion.identity;

//        return new Quaternion(arr[0], arr[1], arr[2], arr[3]);
//    }

//    public void WriteYaml(IEmitter emitter, object value, Type type, ObjectSerializer nestedObjectSerializer)
//    {
//        var q = (Quaternion)value;

//        if (useDouble)
//        {
//            nestedObjectSerializer(new double[] { q.x, q.y, q.z, q.w });
//        }
//        else
//        {
//            nestedObjectSerializer(new float[] { q.x, q.y, q.z, q.w });
//        }
//    }
//}


