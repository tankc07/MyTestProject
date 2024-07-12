using Newtonsoft.Json.Serialization;

namespace api测试
{
    public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            // 如果属性名是第一个单词，则直接转为小写  
            // 否则，使用CamelCase命名风格  
            if (propertyName.IndexOfAny(new[] { '.', '+' }) == -1 && propertyName.IndexOf('_') == -1)
            {
                return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            }

            return base.ResolvePropertyName(propertyName);
        }
    }
}
