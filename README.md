# GeniusOpenKasaIslemleri — Kasa Manuel İşlemler

Toshiba **GeniusOpen** kasalarında versiyon güncelleme ve dosya işlemlerini, IP listesindeki tüm kasalara **tek ekrandan, toplu olarak** uygulayan bir Windows Forms aracı.

Normalde her kasaya tek tek bağlanıp dosya kopyalamak gerekir. Bu araç sayesinde yüzlerce kasada aynı işlem tek tıkla yapılır ve her kasanın sonucu Excel'e loglanır.

![Ekran görüntüsü](https://github.com/user-attachments/assets/632a1fbb-6376-454d-a63f-781e48b15ee6)

## Özellikler

| İşlem | Açıklama |
|---|---|
| **Kasa Versiyonu Güncelle** | `fashion`, `config` ve `lib` klasörlerine yeni dosyaları kopyalar; `parameters.bat` içindeki kasa versiyon jar adını günceller |
| **Libe Dosya Aktar** | Seçilen dosyaları kasanın `lib` klasörüne aktarır |
| **Cp.Bat Veri Ekle** | `cp.bat` dosyasının sonuna yeni jar için `CLASSPATH` satırı ekler |
| **TransferProperties Aktar** | Transfer ayar dosyasını kasalara dağıtır |
| **Eve Teslim Servis Linki Düzenle** | `geniuspos.ini` içindeki eve teslim servis adresini günceller |
| **Dosya Sil / Dosya Adını Değiştir** | Kasalarda toplu dosya silme ve yeniden adlandırma |

Örnek: Cp.Bat alanına `dogukan.jar` yazılırsa `cp.bat` dosyasının sonuna şu satır eklenir:

```bat
set CLASSPATH=%CLASSPATH%;%FASHION_HOME%\lib\dogukan.jar
```

## Kullanım

1. `C:\IpAdresleri\ips.txt` dosyasına kasa IP adreslerini satır satır yazın.
2. `C:\Kaynak\` altındaki `fashion`, `config` ve `lib` klasörlerine aktarılacak dosyaları koyun.
3. Programı açıp ilgili işlemi seçin.
4. Sonuçlar masaüstündeki `log.xlsx` dosyasına IP adresi ve başarılı/başarısız durumu ile yazılır.

## Gereksinimler

- .NET 6.0 (Windows)
- XP tabanlı kasalara erişim için çalıştırılan bilgisayarda **SMB 1.0** desteği açık olmalı
- TCx cihazlarda güvenlik duvarı paylaşıma izin vermeli
- İşlem sırasında `log.xlsx` Excel'de açık olmamalı

## Teknolojiler

C# · .NET 6 Windows Forms · SMB dosya paylaşımı · EPPlus (Excel loglama)
