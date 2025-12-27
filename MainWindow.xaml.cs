using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;
using UAssetAPI.Unversioned;
namespace kardseditor
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Config currentConfig = new Config();
        public UAsset uasset;
        public Usmap usmap;
        public string fileName = "";
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Unreal资产文件|*.uasset";
            if (dialog.ShowDialog(this) == false) return;
            fileName = dialog.FileName;
            usmap = new Usmap(currentConfig.usmap);
            uasset = new UAsset(fileName, (UAssetAPI.UnrealTypes.EngineVersion)currentConfig.uever, usmap);
            LoadKard();
            MessageBox.Show(uasset.FilePath);
        }
        List<string> zazac = new List<string>() { "？", "。", "；", "，", "！", "", "", "" };
        public string zaza(string str)
        {
            Random r = new Random();
            var builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                string s = zazac[r.Next(zazac.Count)];
                builder.Append(str[i]);
                builder.Append(s);
            }
            return builder.ToString();
        }
        public void LoadKardZaza(object sender , RoutedEventArgs e)
        {
            ConfirmPropertiesButton.IsEnabled = true;
            ReloadPropertiesButton.IsEnabled = true;
            List<CardProperty> unitsProperties = new List<CardProperty>();
            NormalExport defaultExp = (NormalExport)uasset.Exports[1];
            foreach (var pair in CardProperties.unitp)
            {
                var p = new CardProperty { K = zaza(pair.Item1), T = zaza(pair.Item2), Desc = zaza(pair.Item3) };
                foreach (var data in defaultExp.Data)
                {
                    if (data.Name.Value.ToString().Equals(pair.Item1, StringComparison.CurrentCultureIgnoreCase))
                    {
                        p.V = pair.Item2 == "int" ? data.RawValue.ToString() : ((bool)data.RawValue).ToString();
                        p.IsActive = true;
                        break;
                    }
                    else
                    {
                        if (pair.Item2 == "bool")
                            p.V = "false";
                        else
                            p.V = "0";
                    }
                }
                p.V = zaza(p.V);
                unitsProperties.Add(p);
            }
            properties.ItemsSource = unitsProperties;
        }
        public void LoadKard()
        {
            ConfirmPropertiesButton.IsEnabled = true;
            ReloadPropertiesButton.IsEnabled = true;
            List<CardProperty> unitsProperties = new List<CardProperty>();
            NormalExport defaultExp = (NormalExport)uasset.Exports[1];
            foreach (var pair in CardProperties.unitp)
            {
                var p = new CardProperty { K = (pair.Item1), T = (pair.Item2), Desc = (pair.Item3) };
                foreach (var data in defaultExp.Data)
                {
                    if (data.Name.Value.ToString().Equals(pair.Item1, StringComparison.CurrentCultureIgnoreCase))
                    {
                        p.V = pair.Item2 == "int" ? data.RawValue.ToString() : ((bool)data.RawValue).ToString();
                        p.IsActive = true;
                        break;
                    }
                    else
                    {
                        if (pair.Item2 == "bool")
                            p.V = "false";
                        else
                            p.V = "0";
                    }
                }
                unitsProperties.Add(p);
            }
            properties.ItemsSource = unitsProperties;
        }
        public MainWindow()
        {
            InitializeComponent();
            currentConfig.read();
            ConfirmPropertiesButton.IsEnabled = false;
            ReloadPropertiesButton.IsEnabled = false;
            List<string> c= new List<string>() { };
            for (int i = 0; i < 100; i++)
                c.Add(i.ToString());
            exportsListView.ItemsSource = c;
        }
        class ccb { 
            public int Index;
            public string Name { get; set; }
        }
        private void Setting(object sender, RoutedEventArgs e)
        {
            setting s = new setting(currentConfig);
            s.Owner = this;  // 可选：设置父窗口
            s.WindowStartupLocation = WindowStartupLocation.CenterOwner;  // 居中显示
            s.Show();
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }
        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
        private void ConfirmProperties(object sender, RoutedEventArgs e)
        {
                int count1 = ((NormalExport)(uasset.Exports[1])).Data.Count;
            foreach (var a in ((List<CardProperty>)properties.ItemsSource)) {
                if (a.IsActive)
                    SetPropertyOrCreate(a);
                else
                    DeletePropertyOrNothing(a);
            }
            int count2 = ((NormalExport)(uasset.Exports[1])).Data.Count;
            MessageBox.Show($"添加了{count2 - count1}个属性");
        }
        private void Save(object sender, RoutedEventArgs r)
        {
            uasset.Write(Path.Combine(currentConfig.exportPath, Path.GetFileName(fileName)));
        }
        private void ReloadProperties(object sender, RoutedEventArgs e)
        {
            uasset = new UAsset(fileName, (UAssetAPI.UnrealTypes.EngineVersion)currentConfig.uever, usmap);
            LoadKard();
        }
        private void SetPropertyOrCreate(CardProperty cp)
        {
            NormalExport normalExport = (NormalExport)uasset.Exports[1];
            var existingData = normalExport.Data.FirstOrDefault(data => data.Name.ToString() == cp.K);

            switch (existingData)
            {
                case IntPropertyData intData when cp.T == "int":
                    intData.Value = Convert.ToInt32(cp.V);
                    break;
                case BoolPropertyData boolData when cp.T == "bool":
                    boolData.Value = GetBool(cp.V);
                    break;
                case null:
                    normalExport.Data.Add(cp.T switch
                    {
                        "int" => new IntPropertyData(FName.FromString(uasset, cp.K)) { Value = Convert.ToInt32(cp.V) },
                        "bool" => new BoolPropertyData(FName.FromString(uasset, cp.K)) { Value = GetBool(cp.V) },
                        _ => throw new ArgumentException($"Unsupported type: {cp.T}")
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Type mismatch for property {cp.K}");
            }
        }
        private void DeletePropertyOrNothing(CardProperty cp)
        {
            NormalExport normalExport = (NormalExport)uasset.Exports[1];
            if (normalExport.Data.Any(p => p.Name.ToString() == cp.K))
            {
                int removedCount = normalExport.Data.RemoveAll(p => p.Name.ToString() == cp.K);
            }
        }
        public bool GetBool(string str) {
            if (str.Equals("true", StringComparison.OrdinalIgnoreCase) || str.Equals("t", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}