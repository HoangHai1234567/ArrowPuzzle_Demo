using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ArrowsPuzzle.Editor
{
    public class Utils
    {
    }

    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _propsToIgnore;

        public IgnorePropertiesResolver(IEnumerable<string> propsToIgnore)
        {
            _propsToIgnore = new HashSet<string>(propsToIgnore);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => !_propsToIgnore.Contains(p.PropertyName)).ToList();
        }
    }
}
