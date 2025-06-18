using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;

namespace KasaVersiyonGuncelleme
{
    public partial class Form1 : Form
    {
        string username = "ADMINISTRATOR";
        string password = "Server1";

        public Form1()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }



        private async void button1_Click(object sender, EventArgs e)
        {
            // Kullanıcıya onay sorusu
            DialogResult result = MessageBox.Show("Kasa Versiyonları Güncellenecek,Onaylıyor musunuz ?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            logRichtext.Invoke(new Action(() =>
            {
                logRichtext.AppendText($"İşlem Başladı.\n");
            }));

            if (result == DialogResult.Yes)
            {
                // Kullanıcı onay verdi, işlemi başlat
                await Task.Run(() => StartUpdateProcess());
            }
            else
            {
                // Kullanıcı hayır dedi, işlem yapılmaz
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            MessageBox.Show("Kasa Versiyonu Güncelleme İşlemleri Tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnDsil_Click(object sender, EventArgs e)
        {
            string dosyaAdi = txtDsil.Text.Trim();
            List<string> ipList = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");

            if (string.IsNullOrEmpty(dosyaAdi))
            {
                MessageBox.Show("Silinecek Dosya Adı Giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            progressBar1.Minimum = 0;
            progressBar1.Maximum = ipList.Count;
            progressBar1.Value = 0;
            string YapilanIslem = "Dosya Silme";

            foreach (string ip in ipList)
            {
                int pingTime = GetPingTime(ip); // Ping kontrolü
                if (pingTime > 150 || pingTime == -1) // 150 ms üzeriyse atla
                {
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[🚫] Atlanan IP: {ip} - Ping: {pingTime}ms\n");
                    }));

                    WriteLogToExcel(ip, "Başarısız", "Ping yüksek veya ulaşılmadı",pingTime,YapilanIslem);
                    progressBar1.PerformStep();
                    continue; // Atla
                }

                bool isDeleted = await DosyaSil(ip, dosyaAdi);
                WriteLogToExcel(ip, isDeleted ? "Başarılı" : "Başarısız", isDeleted ? "" : "Dosya Silinemedi",pingTime,YapilanIslem);
                progressBar1.PerformStep();
            }

            MessageBox.Show("Silme İşlemi Tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private async Task<bool> DosyaSil(string ipAddress, string dosyaAdi)
        {
            string networkPath = $@"\\{ipAddress}\c$\Fashion\{dosyaAdi}";
            bool isConnected = ConnectToNetworkShare($@"\\{ipAddress}\c$", username, password);

            if (isConnected)
            {
                try
                {
                    if (File.Exists(networkPath))
                    {
                        File.Delete(networkPath);
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[✔] Başarılı {ipAddress} - {dosyaAdi} Silindi!\n");
                        }));
                        return true;
                    }
                    else
                    {
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Dosya Bulunamadığından silinemedi.!\n");
                        }));
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Hata: {ex.Message}\n");
                    }));
                    return false;
                }
            }
            else
            {
                logRichtext.Invoke(new Action(() =>
                {
                    logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Ağ Bağlantısı Kurulamadı!\n");
                }));
                return false;
            }
        }

        private bool ConnectToNetworkShare(string networkPath, string username, string password)
        {
            try
            {
                string command = $"net use {networkPath} /user:{username} {password}";
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/C {command}")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process process = Process.Start(psi);
                process.WaitForExit(10000);

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private int GetPingTime(string ipAddress)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ipAddress, 500);
                return reply.Status == IPStatus.Success ? (int)reply.RoundtripTime : -1;
            }
            catch
            {
                return -1;
            }
        }

        private List<string> ReadIpAddressesFromFile(string fileName)
        {
            List<string> ipAddresses = new List<string>();
            try
            {
                ipAddresses.AddRange(File.ReadAllLines(fileName));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            return ipAddresses;
        }

        private void WriteLogToExcel(string ipAddress, string status, string errorDetail,int pingTime,string YapilanIslem)
        {
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.xlsx");

            using (var package = new ExcelPackage(new FileInfo(logFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // İlk çalışma sayfası
                int row = worksheet.Dimension.Rows + 1;

                // 1. sütun: IP Adresi
                worksheet.Cells[row, 1].Value = ipAddress;
                // 2. sütun: Yeni Jar Dosyası Adı
                //worksheet.Cells[row, 2].Value = newJarFileName;
                // 3. sütun: Durum
                worksheet.Cells[row, 2].Value = status;
                // 4. sütun: Hata Detayı
                worksheet.Cells[row, 3].Value = errorDetail;
                // 5. sütun: Dosya Kopyalama Durumu
                //worksheet.Cells[row, 5].Value = isFileCopied ? "Evet" : "Hayır";
                //// 6. sütun: Bat Güncellenmiş Mi
                //worksheet.Cells[row, 6].Value = isBatFileUpdated ? "Evet" : "Hayır";
                //// 7. sütun: Ping Süresi
                worksheet.Cells[row, 4].Value = pingTime;
                worksheet.Cells[row, 5].Value = YapilanIslem;
                //// 8. sütun: Dosya Aktarım Durumu
                //worksheet.Cells[row, 8].Value = isFileTransfer ? "Evet" : "Hayır";
                //// 9. sütun: Silindi Mi? (Bu sadece dosya silme için yazılacak)
                //worksheet.Cells[row, 9].Value = isFileDeleted ? "Evet" : "";

                package.Save(); // Değişiklikleri kaydet
            }
        }


        // Bu metodun eksik olduğunu ekleyelim
        private async void StartUpdateProcess()
        {
            string sourceFolderPath = @"C:\Kaynak\Fashion\"; // Kaynak klasör yolu
            string[] files = Directory.GetFiles(sourceFolderPath);
            if (files.Length == 0)
            {
                MessageBox.Show("Kaynak klasörde dosya bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sourceFilePath = files[0];
            string newJarFileName = Path.GetFileName(sourceFilePath);
            List<string> ipAddresses = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");


            progressBar1.Invoke(new Action(() =>
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = ipAddresses.Count;
                progressBar1.Value = 0;
                progressBar1.Step = 1;
            }));

            foreach (string ipAddress in ipAddresses)
            {
                int pingTime = GetPingTime(ipAddress);
                string status = "Başarısız";
                string errorDetail = string.Empty;
                bool isFileCopied = false;
                bool isBatFileUpdated = false;
                string YapilanIslem = "Kasa Versiyon Güncelleme";

                // Ping süresi 150ms'den büyükse veya ping alınamadıysa işlem yapma
                if (pingTime > 150 || pingTime == -1)
                {
                    status = "Başarısız";
                    errorDetail = pingTime == -1 ? "Ping atılamadı" : $"Ping yüksek: {pingTime}ms";

                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"Atlanan IP: {ipAddress}, Durum: {status}, Hata: {errorDetail}, Ping: {pingTime}ms\n");
                    }));

                    WriteLogToExcel(ipAddress, status, errorDetail,pingTime,YapilanIslem); // Silindi Mi? kısmı boş
                    progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                    continue;
                }

                string networkPath = $@"\\{ipAddress}\c$";
                string remoteFilePath = $@"\\{ipAddress}\c$\Fashion\Parameters.bat";
                string destinationFolderPath = $@"\\{ipAddress}\c$\Fashion"; // Hedef klasör
                string destinationFilePath = Path.Combine(destinationFolderPath, newJarFileName);

                bool isConnected = ConnectToNetworkShare(networkPath, username, password);

                if (isConnected)
                {
                    try
                    {
                        // Hedef klasörün varlığını kontrol et
                        if (!Directory.Exists(destinationFolderPath))
                        {
                            errorDetail = "Hedef klasör bulunamadı!";
                            status = "Başarısız";
                            logRichtext.Invoke(new Action(() =>
                            {
                                logRichtext.AppendText($"[❌] Başarısız {ipAddress} - {errorDetail}, işlem yapılmadı.\n");
                            }));
                            WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem); // Silindi Mi? kısmı boş
                            progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                            continue; // Klasör yoksa işlemi atla
                        }

                        // Dosyayı hedef klasöre kopyala
                        File.Copy(sourceFilePath, destinationFilePath, true);
                        isFileCopied = true;
                        status = "[✔] Başarılı";
                    }
                    catch (Exception ex)
                    {
                        errorDetail = $"HATA! Dosya kopyalama hatası: {ex.Message}";
                        status = "[❌] Başarısız";
                    }

                    // Eğer Parameters.bat dosyası varsa güncelle
                    bool isModified = ModifyBatFile(remoteFilePath, "set FASHION_JAR", $"set FASHION_JAR={newJarFileName}");
                    if (isModified)
                    {
                        isBatFileUpdated = true;
                    }

                    // Excel'e log yaz
                    WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem); // Silindi Mi? kısmı boş

                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"{status} - {ipAddress} (Dosya Kopyalandı: {isFileCopied}, BAT Güncellendi: {isBatFileUpdated})\n");
                    }));
                }
                else
                {
                    errorDetail = "[❌] Bağlantı kurulamadı!";
                    WriteLogToExcel(ipAddress, "[❌] Başarısız", errorDetail, pingTime,YapilanIslem); // Silindi Mi? kısmı boş
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"Bağlantı kurulamadı: {ipAddress}\n");
                    }));
                }

                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
            }
        }



        // Bat dosyasını güncelleme işlemi
        private bool ModifyBatFile(string filePath, string oldLine, string newLine)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(oldLine))
                    {
                        lines[i] = newLine;
                        File.WriteAllLines(filePath, lines);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
                return false;
            }
        }

        // Excel dosyasını oluşturma
        private void CreateNewExcelFile(string logFilePath)
        {
            using (var package = new ExcelPackage(new FileInfo(logFilePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Log");
                worksheet.Cells[1, 1].Value = "IP Adresi";
                worksheet.Cells[1, 2].Value = "Durum";
                worksheet.Cells[1, 3].Value = "Hata Detayı";
                worksheet.Cells[1, 4].Value = "Ping(ms)";
                worksheet.Cells[1, 5].Value = "YapılanIslem";
                //worksheet.Cells[1, 5].Value = "Kopyalama";
                //worksheet.Cells[1, 6].Value = "BAT Güncellendi";
                //worksheet.Cells[1, 7].Value = "Ping (ms)";
                //worksheet.Cells[1, 8].Value = "Dosya Aktarıldı Mı?";
                //worksheet.Cells[1, 9].Value = "Dosya Adı Değiştirildi Mi?";
                package.Save();
            }
        }
        private void TransferFileToLibFolder()
        {
            string username = "ADMINISTRATOR";
            string password = "Server1";
            string sourceFolderPath = @"C:\Kaynak\Lib\";  // Kaynak klasör yolunuz
            string[] files = Directory.GetFiles(sourceFolderPath);
            string status = "Başarısız";
            string errorDetail = string.Empty;
            bool isFileCopied = false;
            string newJarFileName = string.Empty;

            if (files.Length == 0)
            {
                MessageBox.Show("Kaynak klasörde dosya bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // İlk dosyayı al
            string sourceFilePath = files[0];
            newJarFileName = Path.GetFileName(sourceFilePath);

            List<string> ipAddresses = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");
            string YapilanIslem = "Libe Dosya Gönderme";

            progressBar1.Invoke(new Action(() =>
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = ipAddresses.Count;
                progressBar1.Value = 0;
                progressBar1.Step = 1;
            }));

            foreach (string ipAddress in ipAddresses)
            {
                int pingTime = GetPingTime(ipAddress);
                status = "Başarısız";
                errorDetail = string.Empty;
                isFileCopied = false;

                // Ping süresi 150ms'den büyükse veya ping alınamadıysa işlem yapma
                if (pingTime > 150 || pingTime == -1)
                {
                    status = "Başarısız";
                    errorDetail = pingTime == -1 ? "Ping atılamadı" : $"Ping yüksek: {pingTime}ms";
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[🚫] Atlanan IP: {ipAddress}, Durum: {status}, Hata: {errorDetail}, Ping: {pingTime}ms\n");
                    }));
                    WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem);
                    progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                    continue;
                }

                string networkPath = $@"\\{ipAddress}\c$";
                string libFolderPath = $@"\\{ipAddress}\c$\Fashion\lib";  // lib klasörüne dosya aktarımı
                string destinationFilePath = Path.Combine(libFolderPath, newJarFileName);

                bool isConnected = ConnectToNetworkShare(networkPath, username, password);

                if (isConnected)
                {
                    try
                    {
                        // lib klasörünün varlığını kontrol et
                        if (!Directory.Exists(libFolderPath))
                        {
                            logRichtext.Invoke(new Action(() =>
                            {
                                logRichtext.AppendText($"[❌] {ipAddress} - lib klasörü bulunamadı, işlem yapılmadı.\n");
                            }));
                            WriteLogToExcel(ipAddress, status, "lib klasörü bulunamadı", pingTime,YapilanIslem);
                            progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                            continue; // Klasör yoksa işlemi atla
                        }

                        // lib klasörüne dosya kopyala
                        File.Copy(sourceFilePath, destinationFilePath, true); // Dosyayı kopyala
                        isFileCopied = true;
                        status = "[✔] Başarılı";
                    }
                    catch (Exception ex)
                    {
                        errorDetail = $"HATA! Dosya kopyalama hatası: {ex.Message}";
                        status = "[❌] Başarısız";
                    }
                }
                else
                {
                    errorDetail = "Ağa bağlanılamadı";
                    status = "[❌] Başarısız";
                }

                // Log yazma işlemi
                WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem); // Silindi Mi? boş kalacak

                // GUI üzerinde log güncelleme
                logRichtext.Invoke(new Action(() =>
                {
                    logRichtext.AppendText($"{status} {ipAddress}  (Dosya Kopyalandı: {isFileCopied}) Dosya Adı :{newJarFileName}\n");
                }));

                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
            }
        }



        private async void btnLibAktar_Click(object sender, EventArgs e)
        {
            // Kullanıcıya onay sorusu
            DialogResult result = MessageBox.Show("Lib Klasörüne Jar dosyası aktarmak istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Kullanıcı onay verdi, işlemi başlat
                await Task.Run(() => TransferFileToLibFolder());
            }
            else
            {
                // Kullanıcı hayır dedi, işlem yapılmaz
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            MessageBox.Show("Lib içeriğine dosya aktarımı tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void TransferFileToConfigFolder()
        {
            string username = "ADMINISTRATOR";
            string password = "Server1";
            string sourceFolderPath = @"C:\Kaynak\Config\";  // Kaynak klasör yolunuz
            string[] files = Directory.GetFiles(sourceFolderPath);
            string status = "Başarısız";
            string errorDetail = string.Empty;
            bool isFileCopied = false;
            string newJarFileName = string.Empty;

            if (files.Length == 0)
            {
                MessageBox.Show("Kaynak klasörde dosya bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // İlk dosyayı al
            string sourceFilePath = files[0];
            newJarFileName = Path.GetFileName(sourceFilePath);

            List<string> ipAddresses = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");
            string YapilanIslem = "Confige Transfer Properties Gönderme";

            progressBar1.Invoke(new Action(() =>
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = ipAddresses.Count;
                progressBar1.Value = 0;
                progressBar1.Step = 1;
            }));

            foreach (string ipAddress in ipAddresses)
            {
                int pingTime = GetPingTime(ipAddress);
                status = "Başarısız";
                errorDetail = string.Empty;
                isFileCopied = false;

                // Ping süresi 150ms'den büyükse veya ping alınamadıysa işlem yapma
                if (pingTime > 150 || pingTime == -1)
                {
                    status = "Başarısız";
                    errorDetail = pingTime == -1 ? "Ping atılamadı" : $"Ping yüksek: {pingTime}ms";
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[🚫] Atlanan IP: {ipAddress}, Durum: {status}, Hata: {errorDetail}, Ping: {pingTime}ms\n");
                    }));
                    WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem);
                    progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                    continue;
                }

                string networkPath = $@"\\{ipAddress}\c$";
                string configFolderPath = $@"\\{ipAddress}\c$\Fashion\Config";  // Config klasörüne dosya aktarımı
                string destinationFilePath = Path.Combine(configFolderPath, newJarFileName);

                bool isConnected = ConnectToNetworkShare(networkPath, username, password);

                if (isConnected)
                {
                    try
                    {
                        // Config klasörünün varlığını kontrol et
                        if (!Directory.Exists(configFolderPath))
                        {
                            logRichtext.Invoke(new Action(() =>
                            {
                                logRichtext.AppendText($"[❌] {ipAddress} - Config klasörü bulunamadı, işlem yapılmadı.\n");
                            }));
                            WriteLogToExcel(ipAddress, status, "Config klasörü bulunamadı", pingTime,YapilanIslem);
                            progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                            continue; // Klasör yoksa işlemi atla
                        }

                        // Config klasörüne dosya kopyala
                        File.Copy(sourceFilePath, destinationFilePath, true); // Dosyayı kopyala
                        isFileCopied = true;
                        status = "[✔] Başarılı";
                    }
                    catch (Exception ex)
                    {
                        errorDetail = $"HATA! Dosya kopyalama hatası: {ex.Message}";
                        status = "[❌] Başarısız";
                    }
                }
                else
                {
                    errorDetail = "Ağa bağlanılamadı";
                    status = "[❌] Başarısız";
                }

                // Log yazma işlemi
                WriteLogToExcel(ipAddress, status, errorDetail, pingTime,YapilanIslem); // Silindi Mi? boş kalacak

                // GUI üzerinde log güncelleme
                logRichtext.Invoke(new Action(() =>
                {
                    logRichtext.AppendText($"{status} {ipAddress} (Dosya Kopyalandı: {isFileCopied}) : Dosya Adı : {newJarFileName}\n");
                }));

                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
            }
        }


        private async void btnTpAktar_Click(object sender, EventArgs e)
        {
            // Kullanıcıya onay sorusu
            DialogResult result = MessageBox.Show("Transfer Properties aktarmak istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Kullanıcı onay verdi, işlemi başlat
                await Task.Run(() => TransferFileToConfigFolder());
            }
            else
            {
                // Kullanıcı hayır dedi, işlem yapılmaz
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            MessageBox.Show("Transfer Properties Aktarımı Tamamlandı !", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool AppendClasspathToBat(string ip, string txtGirilenDeger)
        {
            string batFilePath = $@"\\{ip}\c$\Fashion\cp.bat";
            bool isConnected = ConnectToNetworkShare($@"\\{ip}\c$", username, password);

            if (isConnected)
            {
                try
                {
                    // Dosya var mı diye kontrol et
                    if (!File.Exists(batFilePath))
                    {
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[❌] Başarısız. {ip} - cp.bat Bulunamadı!\n");
                        }));
                        return false; // Dosya yoksa işlem yapma
                    }

                    List<string> lines = File.ReadAllLines(batFilePath).ToList();
                    string newLine = $"set CLASSPATH=%CLASSPATH%;%FASHION_HOME%\\lib\\{txtGirilenDeger}";

                    if (!lines.Contains(newLine))
                    {
                        lines.Add(newLine); // Satırı en alta ekliyoruz
                        File.WriteAllLines(batFilePath, lines);

                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[✔] Başarılı {ip} - cp.bat Güncellendi: {newLine}\n");
                        }));
                        return true;
                    }
                    else
                    {
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[ℹ️] İşlem Yapılmadı {ip} - Zaten Mevcut: {newLine}\n");
                        }));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[❌] {ip} - Hata: {ex.Message}\n");
                    }));
                    return false;
                }
            }
            else
            {
                logRichtext.Invoke(new Action(() =>
                {
                    logRichtext.AppendText($"[❌] {ip} - Ağ Bağlantısı Kurulamadı!\n");
                }));
                return false;
            }
        }




        private async void btnCpVeriEkle_Click(object sender, EventArgs e)
        {
            {
                string txtGirilenDeger = txtCpVeriEkle.Text.Trim(); // GUI'den girilen dosya adı
                List<string> ipList = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt"); // Otomatik IP listesi
                string YapilanIslem = "Cp Bata Veri ekleme";
                if (string.IsNullOrEmpty(txtGirilenDeger))
                {
                    MessageBox.Show("Cp.bata Eklenecek Jar ismini giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                progressBar1.Minimum = 0;
                progressBar1.Maximum = ipList.Count;
                progressBar1.Value = 0;

                foreach (string ip in ipList)
                {
                    int ping = GetPingTime(ip);
                    bool isUpdated = false;

                    if (ping > 150 || ping == -1)
                    {
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[❌] Atlandı {ip} - Ping Yüksek: {ping}ms\n");
                        }));

                        WriteLogToExcel(ip, "Başarısız", "Ping yüksek veya ulaşılamadı", ping,YapilanIslem);
                    }
                    else
                    {
                        isUpdated = await Task.Run(() => AppendClasspathToBat(ip, txtGirilenDeger));

                        WriteLogToExcel(ip, isUpdated ? "Başarılı" : "Başarısız", isUpdated ? "" : "cp.bat güncellenemedi", ping,YapilanIslem);
                    }

                    progressBar1.PerformStep();
                }

                MessageBox.Show("Classpath Güncelleme İşlemi Tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private async Task UpdateWebServiceAddressForAllIPs(string newAddress)
        {
            List<string> ipList = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");
            string YapilanIslem = "Eve Teslim Servis Linki Güncelleme";
            if (string.IsNullOrEmpty(newAddress))
            {
                MessageBox.Show("Lütfen geçerli bir adres giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Progress bar'ı başlat
            progressBar1.Minimum = 0;
            progressBar1.Maximum = ipList.Count;
            progressBar1.Value = 0;
            progressBar1.Step = 1;



            foreach (string ip in ipList)
            {
                int pingTime = GetPingTime(ip);
                string status = "Başarısız";
                string errorDetail = string.Empty;
                string logMessage = string.Empty;  // Burada log mesajını hazırlayacağız

                // Ping süresi 150ms'den büyükse veya ping alınamadıysa işlem yapma
                if (pingTime > 150 || pingTime == -1)
                {
                    status = "Başarısız";
                    errorDetail = pingTime == -1 ? "Ping atılamadı" : $"Ping yüksek: {pingTime}ms";
                    logMessage = $"Atlanan IP: {ip}, Durum: {status}, Hata: {errorDetail}, Ping: {pingTime}ms";

                    // logRichText'e yazma
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText(logMessage + "\n");
                    }));

                    // Log dosyasına yazma
                    WriteLogToExcel(ip, status, errorDetail, pingTime,YapilanIslem);

                    progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                    continue;  // Eğer ping yüksekse veya atılamıyorsa atla
                }

                // IP'ye bağlantı kontrolü ve ini dosyasını güncelleme
                //$@"\\{ipAddress}\c$";
                string networkPath = $@"\\{ip}\c$";  // Ağ yolu
                string FashionDirectoryPath = $@"\\{ip}\c$\Fashion";  // Ağ yolu
                string iniFilePath = $@"\\{ip}\c$\Fashion\geniuspos.ini";  // Güncellenecek ini dosyası
                bool isConnected = ConnectToNetworkShare(networkPath, username, password);

                if (isConnected)
                {
                    try
                    {
                        // Hedef klasörün varlığını kontrol et
                        if (!Directory.Exists(FashionDirectoryPath))
                        {
                            errorDetail = "Hedef klasör bulunamadı!";
                            status = "[❌] Başarısız";
                            logMessage = $"[❌] Başarısız {ip} - Hedef klasör bulunamadı, işlem yapılmadı.";

                            // logRichText'e yazma
                            logRichtext.Invoke(new Action(() =>
                            {
                                logRichtext.AppendText(logMessage + "\n");
                            }));

                            // Log dosyasına yazma
                            WriteLogToExcel(ip, status, errorDetail, pingTime,YapilanIslem);

                            progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
                            continue; // Hedef klasör yoksa işlemi atla
                        }

                        // Ini dosyasındaki adresi güncelle
                        UpdateWebServiceAddress(iniFilePath, newAddress);

                        status = "[✔] Başarılı";
                        logMessage = $"[✔] Başarılı {ip} - Web servis adresi '{newAddress}' olarak güncellendi!";

                        // logRichText'e yazma
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText(logMessage + "\n");
                        }));

                        // Log dosyasına yazma
                        WriteLogToExcel(ip, status, "", pingTime,YapilanIslem);

                    }
                    catch (Exception ex)
                    {
                        status = "[❌] Başarısız";
                        errorDetail = $"Hata: {ex.Message}";
                        logMessage = $"[❌] Başarısız {ip} - Hata: {errorDetail}";

                        // logRichText'e yazma
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText(logMessage + "\n");
                        }));

                        // Log dosyasına yazma
                        WriteLogToExcel(ip, status, errorDetail, pingTime,YapilanIslem);
                    }
                }
                else
                {
                    errorDetail = "Ağa bağlanılamadı";
                    status = "[❌] Başarısız";
                    logMessage = $"[❌] Başarısız {ip} - Ağ Bağlantısı Kurulamadı!";

                    // logRichText'e yazma
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText(logMessage + "\n");
                    }));

                    // Log dosyasına yazma
                    WriteLogToExcel(ip, status, errorDetail, pingTime,YapilanIslem);
                }

                // Progress bar'ı bir adım ilerlet
                progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
            }
        }




        private void UpdateWebServiceAddress(string iniFilePath, string newAddress)
        {
            try
            {
                // Dosyanın içeriğini oku
                string[] lines = File.ReadAllLines(iniFilePath);

                // "pWebServiceAddress" satırını bul ve adresi değiştir
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("pWebServiceAddress"))
                    {
                        // Eski adresi değiştir ve yeni adresi ekle
                        lines[i] = $"pWebServiceAddress = {newAddress}";
                        break;  // Değişiklik yapıldı, döngüden çık
                    }
                }

                // Dosyayı tekrar yaz
                File.WriteAllLines(iniFilePath, lines);
            }
            catch (Exception ex)
            {
                // Hata durumunda, log yazma ana fonksiyona bırakılacak
                throw new Exception($"Güncelleme yapılamadı: {ex.Message}");
            }
        }



        private async void btnPosini_Click(object sender, EventArgs e)
        {
            // Kullanıcıya onay sorusu
            DialogResult result = MessageBox.Show("Eve Teslim Servis Linki Güncellemek İstediğinizden Emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string newAddress = txtPosini.Text.Trim();  // Kullanıcının girdiği yeni adres

                // `UpdateWebServiceAddressForAllIPs` metodunu çağır
                await UpdateWebServiceAddressForAllIPs(newAddress);
                MessageBox.Show("Eve Teslim Link Güncelleme İşlemi Tamamlandı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Kullanıcı hayır dedi, işlem yapılmaz
                MessageBox.Show("İşlem iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Label1 için ToolTip ekleyelim
            toolTip1.SetToolTip(label2, "Bu label hakkında bilgi veriliyor.");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label2, "\\xx.xx.xx.xx\\C\\Fashion\\silinecekDosyaadi |  textboxa gireceğiz değer bu alandaki dosyayı silecektir. ");
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label5, "\\xx.xx.xx.xx\\C\\Fashion\\cp.bat |  textboxa gireceğiz değer bu dosyanın en alt satırına veri ekleyecektir.\n yalnızca lib adını girmeniz yeterlidir. ÖRNEK : LicenseDomain.jar\n bu 'set CLASSPATH=%CLASSPATH%;%FASHION_HOME%\\\\lib\\\\LicenseDomain.jar'\"; olacaktır. ");
        }

        private void label7_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label7, "\\xx.xx.xx.xx\\C\\Fashion\\GeniusPos.ini |  textboxa gireceğiz değer bu alandaki dosyada  pWebServiceAddress = 'GirilenDeger'\n olarak güncellenecektir.");
        }

        private void btnTpAktar_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnTpAktar, "\\xx.xx.xx.xx\\C\\Fashion\\Config\\ yoluna C\\Kaynak\\Config\\ Klasörü içerisindeki transfer.properties dosyasını aktarır.");
        }

        private void btnLibAktar_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnLibAktar, "\\xx.xx.xx.xx\\C\\Fashion\\Lib\\ yoluna C\\Kaynak\\Lib\\ Klasörü içerisindeki Jar dosyasını aktarır.");
        }

        private void btnBasla_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnBasla, "\\xx.xx.xx.xx\\C\\Fashion\\ yoluna C\\Kaynak\\Fashion\\ Klasörü içerisindeki Jar dosyasını aktarır.\n İlgili hedefte bulunan PARAMETERS.bat dosyasında\nset FASHION_JAR=(AKTARILANDOSYAADI.jar) olarak günceller. ");
        }

        private void btnDsil_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnDsil, "\\xx.xx.xx.xx\\C\\Fashion\\silinecekDosyaadi |  textboxa gireceğiz değer bu alandaki dosyayı silecektir. ");
        }

        private void btnCpVeriEkle_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnCpVeriEkle, "\\xx.xx.xx.xx\\C\\Fashion\\cp.bat |  textboxa gireceğiz değer bu dosyanın en alt satırına veri ekleyecektir.\n yalnızca lib adını girmeniz yeterlidir. ÖRNEK : LicenseDomain.jar\n bu 'set CLASSPATH=%CLASSPATH%;%FASHION_HOME%\\\\lib\\\\LicenseDomain.jar'\"; olacaktır. ");
        }

        private void btnPosini_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnPosini, "\\xx.xx.xx.xx\\C\\Fashion\\GeniusPos.ini |  textboxa gireceğiz değer bu alandaki dosyada  pWebServiceAddress = 'GirilenDeger'\n olarak güncellenecektir.");
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.xlsx");

            if (File.Exists(logFilePath))
            {
                DialogResult result = MessageBox.Show("Log dosyası mevcut. İçeriği temizlemek istiyor musunuz?",
                                                      "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(logFilePath); // Dosyayı sil
                        CreateNewExcelFile(logFilePath); // Yeni dosyayı oluştur
                        MessageBox.Show("Log dosyası başarıyla temizlendi ve yeniden oluşturuldu.",
                                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Şu anda log temizlenemiyor. Lütfen 'log.xlsx' dosyasını kapatın ve tekrar deneyin.",
                                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}",
                                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                CreateNewExcelFile(logFilePath); // Dosya yoksa oluştur
            }
        }

        private async Task<bool> DosyaYenidenAdlandır(string ipAddress, string eskiAd, string yeniAd)
        {
            string eskiYol = $@"\\{ipAddress}\c$\Fashion\{eskiAd}";
            string yeniYol = $@"\\{ipAddress}\c$\Fashion\{yeniAd}";
            bool isConnected = ConnectToNetworkShare($@"\\{ipAddress}\c$", username, password);

            if (isConnected)
            {
                try
                {
                    if (File.Exists(eskiYol))
                    {
                        File.Move(eskiYol, yeniYol);
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[✔] Başarılı {ipAddress} - {eskiAd} -> {yeniAd} olarak değiştirildi!\n");
                        }));
                        return true;
                    }
                    else
                    {
                        logRichtext.Invoke(new Action(() =>
                        {
                            logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Dosya Bulunamadı!\n");
                        }));
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Hata: {ex.Message}\n");
                    }));
                    return false;
                }
            }
            else
            {
                logRichtext.Invoke(new Action(() =>
                {
                    logRichtext.AppendText($"[❌] Başarısız {ipAddress} - Ağ Bağlantısı Kurulamadı!\n");
                }));
                return false;
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            string eskiAd = txtOldName.Text.Trim();
            string yeniAd = txtNewName.Text.Trim();
            List<string> ipList = ReadIpAddressesFromFile(@"C:\IpAdresleri\ips.txt");
            string YapilanIslem = "Dosya Adı Değiştirme";
            if (string.IsNullOrEmpty(eskiAd) || string.IsNullOrEmpty(yeniAd))
            {
                MessageBox.Show("Eski ve Yeni Dosya Adlarını Giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }





            progressBar1.Minimum = 0;
            progressBar1.Maximum = ipList.Count;
            progressBar1.Value = 0;

            foreach (string ip in ipList)
            {
                int pingTime = GetPingTime(ip); // Ping kontrolü
                if (pingTime > 150 || pingTime == -1) // 150 ms üzeriyse atla
                {
                    logRichtext.Invoke(new Action(() =>
                    {
                        logRichtext.AppendText($"[🚫] Atlanan IP: {ip} - Ping: {pingTime}ms\n");
                    }));

                    WriteLogToExcel(ip, "Başarısız", "Ping yüksek veya ulaşılmadı", pingTime,YapilanIslem);
                    progressBar1.PerformStep();
                    continue;
                }

                bool isRenamed = await DosyaYenidenAdlandır(ip, eskiAd, yeniAd);
                WriteLogToExcel(ip, isRenamed ? "Başarılı" : "Başarısız", isRenamed ? "" : "Dosya Yeniden Adlandırılamadı", pingTime,YapilanIslem);
                progressBar1.PerformStep();
            }

            MessageBox.Show("Yeniden Adlandırma İşlemi Tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label3, "\\xx.xx.xx.xx\\C\\Fashion\\ Dosya yolu içerisindeki bir dosyanın adını ve yeni vereceğiniz adı yazıp değiştir dediğinizde ilgili dosyanın ismi değişmiş olur.");
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button1, "\\xx.xx.xx.xx\\C\\Fashion\\ Dosya yolu içerisindeki bir dosyanın adını ve yeni vereceğiniz adı yazıp değiştir dediğinizde ilgili dosyanın ismi değişmiş olur.");
        }
    }
}

