using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class BoolArray2DConverter : JsonConverter<bool[,]>
{
    public override bool[,] Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // 读取 JSON 数组
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        int rows = root.GetArrayLength();
        if (rows == 0) return new bool[0, 0];

        int cols = root[0].GetArrayLength();
        bool[,] array = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            JsonElement row = root[i];
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = row[j].GetBoolean();
            }
        }

        return array;
    }

    public override void Write(
        Utf8JsonWriter writer,
        bool[,] value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        int rows = value.GetLength(0);
        int cols = value.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            writer.WriteStartArray();
            for (int j = 0; j < cols; j++)
            {
                writer.WriteBooleanValue(value[i, j]);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}