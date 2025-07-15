using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Linq; // Added for .Select()
using System; // Added for Exception
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace APKS_installer
{
    public partial class APKS_Installer : Form
    {
        private System.Windows.Forms.Timer deviceRefreshTimer;
        public APKS_Installer()
        {
            InitializeComponent();
            InitLanguageComboBox();
            UpdateDeviceList();
            // Таймер для автообновления списка устройств
            deviceRefreshTimer = new System.Windows.Forms.Timer();
            deviceRefreshTimer.Interval = 3000; // 3 секунды
            deviceRefreshTimer.Tick += (s, e) => UpdateDeviceList();
            deviceRefreshTimer.Start();
        }

        private void InitLanguageComboBox()
        {
            comboBoxLanguage.Items.Clear();
            comboBoxLanguage.Items.Add("English");
            comboBoxLanguage.Items.Add("Русский");
            comboBoxLanguage.SelectedIndex = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? 1 : 0;
            comboBoxLanguage.SelectedIndexChanged += ComboBoxLanguage_SelectedIndexChanged;
        }

        private void ComboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lang = comboBoxLanguage.SelectedIndex == 1 ? "ru" : "en";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            UpdateUILocalization();
        }

        private void UpdateUILocalization()
        {
            buttonInstallAll.Text = APKS_installer.Properties.Resources.ButtonInstallAll;
            buttonRefreshDevices.Text = APKS_installer.Properties.Resources.ButtonRefreshDevices;
            this.Text = APKS_installer.Properties.Resources.FormTitle;
            textBoxFilePath.PlaceholderText = APKS_installer.Properties.Resources.DragDropHint;
            // Можно добавить обновление других элементов, если потребуется
        }

        private void Log(string message)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.Invoke(new Action(() => {
                    textBoxLog.AppendText(message + "\r\n");
                }));
            }
            else
            {
                textBoxLog.AppendText(message + "\r\n");
            }
        }

        private void SetProgress(int percent)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = percent));
            }
            else
            {
                progressBar.Value = percent;
            }
        }

        private async void buttonSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "APKs файлы (*.apks)|*.apks|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите файл APKs";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxFilePath.Text = openFileDialog.FileName;
                    string appDir = Application.StartupPath;
                    string unzipDir = Path.Combine(appDir, "appUnzip");
                    await Task.Run(() =>
                    {
                        Log($"Создание/очистка папки: {unzipDir}");
                        if (!Directory.Exists(unzipDir))
                        {
                            Directory.CreateDirectory(unzipDir);
                        }
                        foreach (var file in Directory.GetFiles(unzipDir))
                        {
                            File.Delete(file);
                        }
                        foreach (var dir in Directory.GetDirectories(unzipDir))
                        {
                            Directory.Delete(dir, true);
                        }
                        Log($"Распаковка файла: {openFileDialog.FileName}");
                        // Прогресс разархивирования
                        using (var archive = ZipFile.OpenRead(openFileDialog.FileName))
                        {
                            int total = archive.Entries.Count;
                            int done = 0;
                            foreach (var entry in archive.Entries)
                            {
                                string destPath = Path.Combine(unzipDir, entry.FullName);
                                string destDir = Path.GetDirectoryName(destPath);
                                if (!Directory.Exists(destDir))
                                    Directory.CreateDirectory(destDir);
                                if (!string.IsNullOrEmpty(entry.Name))
                                    entry.ExtractToFile(destPath, true);
                                done++;
                                SetProgress((int)(done * 100.0 / total));
                            }
                        }
                        Log($"Файл успешно разархивирован в: {unzipDir}");
                        SetProgress(100);
                    });
                    MessageBox.Show($"Файл успешно разархивирован в: {unzipDir}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetProgress(0);
                }
            }
        }

        private void RunProcessWithLog(string exePath, string arguments, string description)
        {
            Log($"--- {description} ---");
            Log($"Команда: {exePath} {arguments}");
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Log($"STDOUT:\r\n{output}");
                if (!string.IsNullOrWhiteSpace(error))
                    Log($"STDERR:\r\n{error}");
                Log($"Код завершения: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                Log($"Ошибка запуска процесса: {ex.Message}");
            }
        }

        private void buttonRefreshDevices_Click(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }

        private void UpdateDeviceList()
        {
            string appDir = Application.StartupPath;
            string adbPath = Path.Combine(appDir, "adb.exe");
            if (!File.Exists(adbPath))
            {
                Log(APKS_installer.Properties.Resources.AdbNotFound);
                comboBoxDevices.Items.Clear();
                comboBoxDevices.Items.Add(APKS_installer.Properties.Resources.AdbNotFound);
                comboBoxDevices.SelectedIndex = 0;
                return;
            }
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = adbPath;
                process.StartInfo.Arguments = "devices";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var devices = lines.Skip(1)
                    .Where(l => l.Contains("device") && !l.Contains("unauthorized") && !l.Contains("offline"))
                    .Select(l => l.Split('\t')[0])
                    .ToList();
                comboBoxDevices.Items.Clear();
                if (devices.Count == 0)
                {
                    comboBoxDevices.Items.Add(APKS_installer.Properties.Resources.NoDevices);
                    comboBoxDevices.SelectedIndex = 0;
                }
                else
                {
                    foreach (var dev in devices)
                    {
                        string model = null;
                        string ip = null;
                        try
                        {
                            // Получаем модель устройства
                            var procModel = new System.Diagnostics.Process();
                            procModel.StartInfo.FileName = adbPath;
                            procModel.StartInfo.Arguments = $"-s {dev} shell getprop ro.product.model";
                            procModel.StartInfo.UseShellExecute = false;
                            procModel.StartInfo.RedirectStandardOutput = true;
                            procModel.StartInfo.RedirectStandardError = true;
                            procModel.StartInfo.CreateNoWindow = true;
                            procModel.Start();
                            model = procModel.StandardOutput.ReadToEnd().Trim();
                            procModel.WaitForExit();
                            if (string.IsNullOrWhiteSpace(model)) model = null;
                        }
                        catch { model = null; }
                        try
                        {
                            // Получаем IP-адрес устройства (wlan0)
                            var procIp = new System.Diagnostics.Process();
                            procIp.StartInfo.FileName = adbPath;
                            procIp.StartInfo.Arguments = $"-s {dev} shell ip -f inet addr show wlan0";
                            procIp.StartInfo.UseShellExecute = false;
                            procIp.StartInfo.RedirectStandardOutput = true;
                            procIp.StartInfo.RedirectStandardError = true;
                            procIp.StartInfo.CreateNoWindow = true;
                            procIp.Start();
                            string ipOut = procIp.StandardOutput.ReadToEnd();
                            procIp.WaitForExit();
                            // Парсим IP
                            var ipLine = ipOut.Split('\n').FirstOrDefault(l => l.Trim().StartsWith("inet "));
                            if (ipLine != null)
                            {
                                var parts = ipLine.Trim().Split(' ');
                                if (parts.Length > 1)
                                {
                                    ip = parts[1].Split('/')[0];
                                }
                            }
                        }
                        catch { ip = null; }
                        string display = "";
                        if (!string.IsNullOrEmpty(model))
                            display += model + " ";
                        display += $"({dev}";
                        if (!string.IsNullOrEmpty(ip))
                            display += $", {ip}";
                        display += ")";
                        comboBoxDevices.Items.Add(display);
                    }
                    comboBoxDevices.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log(APKS_installer.Properties.Resources.Error + ": " + ex.Message);
                comboBoxDevices.Items.Clear();
                comboBoxDevices.Items.Add(APKS_installer.Properties.Resources.Error);
                comboBoxDevices.SelectedIndex = 0;
            }
        }

        private async void buttonInstallAll_Click(object sender, EventArgs e)
        {
            string appDir = Application.StartupPath;
            string unzipDir = Path.Combine(appDir, "appUnzip");
            string adbPath = Path.Combine(appDir, "adb.exe");
            SetProgress(0);
            if (!File.Exists(adbPath))
            {
                Log(APKS_installer.Properties.Resources.AdbNotFound);
                MessageBox.Show(APKS_installer.Properties.Resources.AdbNotFound, APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string apksFile = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "APKs (*.apks)|*.apks|*.*|*.*";
                openFileDialog.Title = APKS_installer.Properties.Resources.SelectApksTitle;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    apksFile = openFileDialog.FileName;
                    textBoxFilePath.Text = apksFile;
                }
                else
                {
                    Log(APKS_installer.Properties.Resources.ApksNotSelected);
                    return;
                }
            }
            await Task.Run(() =>
            {
                Log(APKS_installer.Properties.Resources.ClearFolderLog + unzipDir);
                if (!Directory.Exists(unzipDir))
                {
                    Directory.CreateDirectory(unzipDir);
                }
                foreach (var file in Directory.GetFiles(unzipDir))
                {
                    File.Delete(file);
                }
                foreach (var dir in Directory.GetDirectories(unzipDir))
                {
                    Directory.Delete(dir, true);
                }
                Log(APKS_installer.Properties.Resources.UnzipLog + apksFile);
                using (var archive = ZipFile.OpenRead(apksFile))
                {
                    int total = archive.Entries.Count;
                    int done = 0;
                    foreach (var entry in archive.Entries)
                    {
                        string destPath = Path.Combine(unzipDir, entry.FullName);
                        string destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                            Directory.CreateDirectory(destDir);
                        if (!string.IsNullOrEmpty(entry.Name))
                            entry.ExtractToFile(destPath, true);
                        done++;
                        SetProgress((int)(done * 100.0 / total));
                    }
                }
                Log(APKS_installer.Properties.Resources.SuccessUnzip + unzipDir);
                SetProgress(100);
            });
            SetProgress(0);
            // --- установка APK ---
            Log("\n" + APKS_installer.Properties.Resources.InstallStarted);
            SetProgress(10);
            if (!Directory.Exists(unzipDir))
            {
                Log(APKS_installer.Properties.Resources.Error + ": " + APKS_installer.Properties.Resources.SuccessUnzip + "!");
                MessageBox.Show(APKS_installer.Properties.Resources.Error + ": " + APKS_installer.Properties.Resources.SuccessUnzip + "!", APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var files = Directory.GetFiles(unzipDir, "*.apk");
            if (files.Length == 0)
            {
                Log(APKS_installer.Properties.Resources.NoApkFiles);
                MessageBox.Show(APKS_installer.Properties.Resources.NoApkFiles, APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string selectedDevice = null;
            this.Invoke((MethodInvoker)delegate {
                if (comboBoxDevices.SelectedItem != null && !comboBoxDevices.SelectedItem.ToString().Contains(APKS_installer.Properties.Resources.NoDevices, StringComparison.OrdinalIgnoreCase))
                    selectedDevice = ExtractSerial(comboBoxDevices.SelectedItem.ToString());
            });
            if (string.IsNullOrEmpty(selectedDevice) || selectedDevice.Contains(APKS_installer.Properties.Resources.NoDevices, StringComparison.OrdinalIgnoreCase))
            {
                Log(APKS_installer.Properties.Resources.NoDeviceSelected);
                MessageBox.Show(APKS_installer.Properties.Resources.NoDeviceSelected, APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            await Task.Run(() =>
            {
                // 1. Проверяем устройства
                SetProgress(20);
                RunProcessWithLog(adbPath, "devices", APKS_installer.Properties.Resources.CheckDevicesLog);
                // 2. Формируем команду установки
                string filesArg = string.Join(" ", files.Select(f => $"\"{f}\""));
                string arguments = $"-s {selectedDevice} install-multiple {filesArg}";
                Log(string.Format(APKS_installer.Properties.Resources.PrepareInstallLog, files.Length, string.Join(", ", files.Select(Path.GetFileName))));
                SetProgress(40);
                // 3. Запускаем установку
                Log(APKS_installer.Properties.Resources.StartInstallLog);
                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = adbPath;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    SetProgress(90);
                    Log("STDOUT:\r\n" + output);
                    if (!string.IsNullOrWhiteSpace(error))
                        Log("STDERR:\r\n" + error);
                    Log(APKS_installer.Properties.Resources.Success + ": " + process.ExitCode);
                    if (process.ExitCode == 0)
                    {
                        Log(APKS_installer.Properties.Resources.InstallSuccess);
                        this.Invoke((MethodInvoker)delegate {
                            MessageBox.Show(APKS_installer.Properties.Resources.InstallSuccess + "\n" + output, APKS_installer.Properties.Resources.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    }
                    else
                    {
                        Log(APKS_installer.Properties.Resources.InstallError);
                        this.Invoke((MethodInvoker)delegate {
                            MessageBox.Show(APKS_installer.Properties.Resources.InstallError + "\n" + error, APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log(APKS_installer.Properties.Resources.Error + ": " + ex.Message);
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show(APKS_installer.Properties.Resources.Error + ": " + ex.Message, APKS_installer.Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                SetProgress(100);
                Log(APKS_installer.Properties.Resources.InstallFinished + "\n");
                Task.Delay(500).Wait();
                SetProgress(0);
            });
        }

        private string ExtractSerial(string deviceString)
        {
            int start = deviceString.IndexOf('(');
            int end = deviceString.IndexOf(')');
            if (start >= 0 && end > start)
            {
                string inside = deviceString.Substring(start + 1, end - start - 1);
                return inside.Split(',')[0].Trim();
            }
            return deviceString.Trim();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && files[0].ToLower().EndsWith(".apks"))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && files[0].ToLower().EndsWith(".apks"))
                {
                    textBoxFilePath.Text = files[0];
                    string appDir = Application.StartupPath;
                    string unzipDir = Path.Combine(appDir, "appUnzip");
                    await Task.Run(() =>
                    {
                        Log(APKS_installer.Properties.Resources.ClearFolderLog + unzipDir);
                        if (!Directory.Exists(unzipDir))
                        {
                            Directory.CreateDirectory(unzipDir);
                        }
                        foreach (var file in Directory.GetFiles(unzipDir))
                        {
                            File.Delete(file);
                        }
                        foreach (var dir in Directory.GetDirectories(unzipDir))
                        {
                            Directory.Delete(dir, true);
                        }
                        Log(APKS_installer.Properties.Resources.UnzipLog + files[0]);
                        using (var archive = ZipFile.OpenRead(files[0]))
                        {
                            int total = archive.Entries.Count;
                            int done = 0;
                            foreach (var entry in archive.Entries)
                            {
                                string destPath = Path.Combine(unzipDir, entry.FullName);
                                string destDir = Path.GetDirectoryName(destPath);
                                if (!Directory.Exists(destDir))
                                    Directory.CreateDirectory(destDir);
                                if (!string.IsNullOrEmpty(entry.Name))
                                    entry.ExtractToFile(destPath, true);
                                done++;
                                SetProgress((int)(done * 100.0 / total));
                            }
                        }
                        Log(APKS_installer.Properties.Resources.SuccessUnzip + unzipDir);
                        SetProgress(100);
                    });
                    MessageBox.Show(APKS_installer.Properties.Resources.SuccessUnzip + unzipDir, APKS_installer.Properties.Resources.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetProgress(0);
                }
            }
        }
    }
}
