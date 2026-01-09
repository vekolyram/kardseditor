using Microsoft.Win32;
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

namespace kardseditor
{
    /// <summary>
    /// setting.xaml 的交互逻辑
    /// </summary>
    public partial class setting
    {
        Config needEditConfig = new Config();
        public setting(Config currentConfig)
        {
            InitializeComponent();
            assetPathInput.Text = currentConfig.exportPath;
            mappingPathInput.Text = currentConfig.usmap;
            ueverCombo.SelectedIndex = currentConfig.uever - 36;
            needEditConfig = currentConfig;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            assetPathInput.Text = assetPathInput.Text.Replace(@"\", @"\\");
            mappingPathInput.Text = mappingPathInput.Text.Replace(@"\", @"\\");
            needEditConfig.exportPath = assetPathInput.Text;
            needEditConfig.usmap = mappingPathInput.Text;
            needEditConfig.uever = ueverCombo.SelectedIndex + 36;
            needEditConfig.save();
            this.Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChooseMap(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Unreal映射文件|*.usmap";
            if (dialog.ShowDialog(this) == false) return;
            var fileName = dialog.FileName;
            if (fileName == null) return;
            mappingPathInput.Text = fileName;
        }
        private void ChooseExport(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog(this) == false) return;
            var fileName = dialog.FolderName;
            if (fileName == null) return;
            assetPathInput.Text = fileName;
        }
    }
}
