using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
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
        public Export cloneExport;
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Unreal资产文件|*.uasset";
            if (dialog.ShowDialog(this) == false) return;
            fileName = dialog.FileName;
            usmap = new Usmap(currentConfig.usmap);
            uasset = new UAsset(fileName, (UAssetAPI.UnrealTypes.EngineVersion)currentConfig.uever, usmap);
            LoadKard();
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
            LoadExportsAndImports();
        }
        public MainWindow()
        {
            InitializeComponent();
            currentConfig.read();
            ConfirmPropertiesButton.IsEnabled = false;
            ReloadMenu.IsEnabled = false;
            fileName = ".\\card_event_supply_shortage.uasset";
            usmap = new Usmap(currentConfig.usmap);
            uasset = new UAsset(fileName, (UAssetAPI.UnrealTypes.EngineVersion)currentConfig.uever, usmap);
            LoadKard();
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
        public void LoadExportsAndImports() {
            List<Wrapper<Export>> wrappers = new List<Wrapper<Export>>() { };
            foreach (var e in uasset.Exports)
            {
                wrappers.Add(new Wrapper<Export>(e, "ObjectName"));
            }
            exportsListView.ItemsSource = wrappers;
            List<Wrapper<Import>> wrappers1 = new List<Wrapper<Import>>() { };
            foreach (var e in uasset.Imports)
            {
                wrappers1.Add(new Wrapper<Import>(e, "ObjectName"));
            }
            importsListView.ItemsSource = wrappers1;
        }
        private void PasteE(object sender, RoutedEventArgs e)
        {
            uasset.Exports.Add(cloneExport);
            LoadExportsAndImports();
        }
        private void ExportSE(object sender, RoutedEventArgs e)
        {
            var export = ((Wrapper<Export>)(exportsListView.SelectedItem)).Origin;
            string json = JsonConvert.SerializeObject(export, Formatting.Indented, UAsset.jsonSettings);
            File.WriteAllText(Path.Combine(currentConfig.exportPath, Path.GetFileNameWithoutExtension(fileName))+export.ObjectName+".json",json);
        }
        //private void ImportSE(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new OpenFileDialog();
        //    dialog.Filter = "Json文件|*.json";
        //    if (dialog.ShowDialog(this) == false) return;
        //    fileName = dialog.FileName;
        //    Export exp = JsonConvert.DeserializeObject<Export>(File.ReadAllText(fileName));
        //    var export = ((Wrapper<Export>)(exportsListView.SelectedItem)).Origin;
        //    export = exp;
        //}
        private void CopyOE(object sender, RoutedEventArgs e)
        {
            cloneExport = (Export)((Wrapper<Export>)(exportsListView.SelectedItem)).Origin.Clone();
            cloneExport.ObjectName = FName.FromString(uasset,cloneExport.ObjectName.ToString()+"1");
            pasteMenu.IsEnabled= true;
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
        public void CreateImport(object s,RoutedEventArgs e) {
        }
        public bool GetBool(string str)
        {
            if (str.Equals("true", StringComparison.OrdinalIgnoreCase) || str.Equals("t", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        private void CreateExport(object sender, RoutedEventArgs e)
        {
            var b = (FunctionExport)(uasset.Exports[3]);
            FunctionExport newFE = new FunctionExport(uasset, new byte[] { 00, 00, 00, 00 })
            {
                ObjectName = FName.FromString(uasset, "ccb"),
                ClassIndex = b.ClassIndex,//-3 Function
                LoadedProperties = b.LoadedProperties,//
                OuterIndex = b.OuterIndex,//1 class名
                TemplateIndex = b.TemplateIndex,//-9 Default_Function
                Children = b.Children,//{UAssetAPI.UnrealTypes.FPackageIndex[0]}
                FunctionFlags = b.FunctionFlags,//FUNC_Event | FUNC_Public | FUNC_BlueprintCallable | FUNC_BlueprintEvent
                ObjectFlags = b.ObjectFlags,//EObjectFlags.RF_Public
                ScriptBytecode = b.ScriptBytecode,
                CreateBeforeSerializationDependencies = b.CreateBeforeSerializationDependencies,
                CreateBeforeCreateDependencies = b.CreateBeforeCreateDependencies,
                Data = b.Data,//null
                Field = b.Field,//Next=null
                SuperStruct = b.SuperStruct,
                SuperIndex = b.SuperIndex//-22  
            };
            uasset.Exports.Add(newFE);
        }
        private void DeleteExport(object sender, RoutedEventArgs e)
        {
           
        }
        private void LookUp(object sender, RoutedEventArgs e)
        {
            lookup lw= new lookup(uasset);
            lw.Show();
        }
        private void exportsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            var t = (Wrapper<Export>)(exportsListView.SelectedItem);
            if (t != null)
            {
                sb.AppendLine($"Object Name: {t.Origin.ObjectName}");
                sb.AppendLine($"Class Index: {t.Origin.ClassIndex}");
                sb.AppendLine($"Serial Size: {t.Origin.SerialSize}");
                sb.AppendLine($"Outer Index: {t.Origin.OuterIndex}");
                infoTextBlock.Text = sb.ToString();
            }
        }

        private void importsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            var t = (Wrapper<Import>)(importsListView.SelectedItem);
            if (t != null)
            {
                sb.AppendLine($"Object Name: {t.Origin.ObjectName}");
                sb.AppendLine($"Class Name: {t.Origin.ClassName}");
                sb.AppendLine($"Package Name: {t.Origin.PackageName}");
                sb.AppendLine($"Outer Index: {t.Origin.OuterIndex}");
                infoiTextBlock.Text = sb.ToString();
            }
            FindFexpBasicP(uasset);
        }
        public FunctionExport FindFexpBasicP(UAsset asset) {
            var a= uasset.SearchForImport(FName.FromString(asset, "Default_Function"));
            MessageBox.Show(uasset.Imports[a].ObjectName.ToString());
            return null;
        }
    }
}