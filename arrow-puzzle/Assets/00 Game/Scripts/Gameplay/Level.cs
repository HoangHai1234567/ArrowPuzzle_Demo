using Newtonsoft.Json;

namespace ArrowsPuzzle
{
    public class Level
    {
        public LevelData Data { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Level(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void LoadFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Data = new LevelData();
                return;
            }

            Data = JsonConvert.DeserializeObject<LevelData>(text);
        }
    }
}
