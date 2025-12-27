using System.IO;

namespace kardseditor
{
    public class Config
    {
        public string uasset = "";
        public string usmap = "";
        public string exportPath = "";
        public int uever = 0;
        public void save()
        {
            string[] configs = { usmap, uasset, exportPath, uever.ToString() };
            File.WriteAllLines("./setting.txt", configs);
        }
        public void read()
        {
            String[] configStr = File.ReadAllLines("./setting.txt");
            usmap = configStr[0];
            uasset = configStr[1];
            exportPath = configStr[2];
            uever = Convert.ToInt32(configStr[3]);
        }
    };
    public class CardProperty
    {
        public string T { get; set; }
        public string K { get; set; }
        public string V { get; set; }
        public string Desc { get; set; }
        public bool IsActive {get;set;}
    };
    public class CardProperties
    {
        static readonly public List<Tuple<string, string,string>> unitp= new List<Tuple<string, string,string>>
{
    Tuple.Create("attack", "int","攻击力"),
    Tuple.Create("defense", "int","防御力"),
    Tuple.Create("range", "int","范围"),
    Tuple.Create("kredits", "int","费用"),
    Tuple.Create("operationCost", "int","行动费用"),
    Tuple.Create("heavyArmor", "int","重甲"),
    Tuple.Create("cipher", "int","情报"),
    Tuple.Create("hasBlitz", "bool","嫩山鸡"),
    Tuple.Create("hasAmbush", "bool","有腹肌"),
    Tuple.Create("hasSmokescreen", "bool","抽锐刻"),
    Tuple.Create("hasMobilize", "bool", "动员"),
    Tuple.Create("hasAlpine", "bool", "爬山"),
    Tuple.Create("hasFury", "bool", "凤展"),
    Tuple.Create("hasGuard", "bool", "手护"),
    Tuple.Create("hasShock", "bool", "手冲"),
    Tuple.Create("hasCovert", "bool", "转环"),
    Tuple.Create("hasScrying", "bool", "会占卜"),
    Tuple.Create("hasPincer", "bool", "有前几"),
    Tuple.Create("hasSalvage", "bool", "抢救"),
};
    }
}
