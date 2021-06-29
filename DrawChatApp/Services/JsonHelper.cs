using DrawChatApp.Infrastructure;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DrawChatApp.Services
{
    public static class JsonHelper
    {
        public static async Task CreateJsonFile<T>(string fileName, T entity)
        {
            string path = $"{Constants.StorageDirectory}\\{fileName}.json";

            using FileStream createStream = File.Create(path);
            await JsonSerializer.SerializeAsync(createStream, entity);
            await createStream.DisposeAsync();
        }

        public static async Task<T> ReadJsonFile<T>(string fileName)
        {
            string path = $"{Constants.StorageDirectory}\\{fileName}.json";

            using FileStream openStream = File.OpenRead(path);
           
            return await JsonSerializer.DeserializeAsync<T>(openStream);
        }
    }
}
