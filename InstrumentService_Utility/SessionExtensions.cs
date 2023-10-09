using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace InstrumentService_Utility
{
    //методы расширения служат для сераилазации и десерализации 
    //key - ключ, под которым хотим сохранить объект, ключ позволит индефицировать данные в сессии 
    //value - объект, который мы хотим сохранить в сессии
    //в set - происходит сериализация объекта(то есть преобразование в байтовый поток) value и сохрание объекта в key
    //в get превращаем байтовый поток в объект, извлекаем объект типа T из сессии
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value)); 
            
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
            
        }
    }
}
