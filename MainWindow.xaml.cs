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
        public void LoadKardZaza(object sender, RoutedEventArgs e)
        {
            ConfirmPropertiesButton.IsEnabled = true;
            ReloadMenu.IsEnabled = true;
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
            ReloadMenu.IsEnabled = true;
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
            List<Wrapper<Export>> wrappers = new List<Wrapper<Export>>() { };
            foreach (var e in uasset.Exports) {
                wrappers.Add(new Wrapper<Export>(e, "ObjectName"));
            }
            exportsListView.ItemsSource=wrappers;
        }
        public MainWindow()
        {
            InitializeComponent();
            currentConfig.read();
            ConfirmPropertiesButton.IsEnabled = false;
            ReloadMenu.IsEnabled = false;
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
            foreach (var a in ((List<CardProperty>)properties.ItemsSource))
            {
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
        private void Reload(object sender, RoutedEventArgs e)
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
        public bool GetBool(string str)
        {
            if (str.Equals("true", StringComparison.OrdinalIgnoreCase) || str.Equals("t", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void CreateExport(object sender, RoutedEventArgs e)
        {
            var SE = (StructExport)uasset.Exports[0];
            var newSE=(StructExport)SE.Clone();
            newSE.ObjectName = FName.FromString(uasset,"test11");
            newSE.ObjectGuid=Guid.NewGuid();
            FunctionExport newFE = new FunctionExport(uasset, new byte[] { 00, 00, 00, 00 })
            {
                ObjectName = FName.FromString(uasset, "ccb"),
                ClassIndex = uasset.Exports[0].ClassIndex,
               OuterIndex = FPackageIndex.FromExport(0),
                FunctionFlags = UAssetAPI.UnrealTypes.EFunctionFlags.FUNC_Event | UAssetAPI.UnrealTypes.EFunctionFlags.FUNC_Public | UAssetAPI.UnrealTypes.EFunctionFlags.FUNC_BlueprintCallable | UAssetAPI.UnrealTypes.EFunctionFlags.FUNC_BlueprintEvent,
            };
            /*
             -		[4]	{ObjectName: OnStartOfTurn
OuterIndex: 1
ClassIndex: -3
SuperIndex: -23
TemplateIndex: -9
ObjectFlags: RF_Public
SerialSize: 133
SerialOffset: 10199
ScriptSerializationStartOffset: 0
ScriptSerializationEndOffset: 0
bForcedExport: False
bNotForClient: False
bNotForServer: False
PackageGuid: 00000000-0000-0000-0000-000000000000
IsInheritedInstance: False
PackageFlags: PKG_None
bNotAlwaysLoadedForEditorGame: False
bIsAsset: False
GeneratePublicHash: False
SerializationBeforeSerializationDependencies: System.Collections.Generic.List`1[UAssetAPI.UnrealTypes.FPackageIndex]
CreateBeforeSerializationDependencies: System.Collections.Generic.List`1[UAssetAPI.UnrealTypes.FPackageIndex]
SerializationBeforeCreateDependencies: System.Collections.Generic.List`1[UAssetAPI.UnrealTypes.FPackageIndex]
CreateBeforeCreateDependencies: System.Collections.Generic.List`1[UAssetAPI.UnrealTypes.FPackageIndex]
}	UAssetAPI.ExportTypes.Export {UAssetAPI.ExportTypes.FunctionExport}

             */
            uasset.Exports.Add(newSE);
        }
        private void DeleteExport(object sender, RoutedEventArgs e)
        {

        }
    }
}