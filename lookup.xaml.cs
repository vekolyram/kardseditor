using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.UnrealTypes;

namespace kardseditor
{
    /// <summary>
    /// lookup.xaml 的交互逻辑
    /// </summary>
    public partial class lookup
    {
        UAsset asset;
        public lookup(UAsset asset)
        {
            this.asset = asset;
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FPackageIndex f = new FPackageIndex(int.Parse(t1.Text));
                Export ex = new Export();
                Import ix = new Import();
                StringBuilder sb = new StringBuilder();
                if (f.IsImport())
                {
                    ix = f.ToImport(asset);
                    sb.AppendLine($"Is Import,");
                    sb.AppendLine($"Object Name: {ix.ObjectName},");
                    sb.AppendLine($"Class Name: {ix.ClassName},");
                    sb.AppendLine($"Class Package: {ix.ClassPackage},");
                    sb.AppendLine($"Outer Index: {ix.OuterIndex}");
                }
                else
                {
                    ex = f.ToExport(asset);
                    sb.AppendLine($"Is Export,");
                    sb.AppendLine($"Object Name: {ex.ObjectName},");
                    sb.AppendLine($"Class Index: {ex.ClassIndex},");
                    sb.AppendLine($"Outer Index: {ex.OuterIndex}");
                }
                l1.Text = sb.ToString();
            }
            catch {
                l1.Text = "未找到。";
            }
        }
        private void t1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(null, null);
            }
        }
    }
}
