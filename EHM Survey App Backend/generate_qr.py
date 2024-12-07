import qrcode
import os
import sys

def generate_qr(store_code):
    # Frontend URL'sini oluşturun
    base_url = "http://localhost:3000/survey"
    url = f"{base_url}/{store_code}"  # QR koduna gömülecek URL

    # QR kodunu oluştur
    qr = qrcode.make(url)

    # QR kodun kaydedileceği klasörü ve dosya yolunu ayarla
    output_directory = os.path.join("wwwroot", "qrcodes")  # 'wwwroot/qrcodes' dizini
    os.makedirs(output_directory, exist_ok=True)  # Klasör yoksa oluştur

    # Dosya adını ve tam yolunu ayarla
    file_name = f"{store_code}_qrcode.png"  # Örnek: STORE123_qrcode.png
    file_path = os.path.join(output_directory, file_name)  # Tam dosya yolunu oluştur

    # QR kodunu kaydet
    qr.save(file_path)
    print(f"QR kod '{file_path}' olarak oluşturuldu.")  # Dosya oluşturulduğunu yazdır

if __name__ == "__main__":
    if len(sys.argv) > 1:
        store_code = sys.argv[1]  # Komut satırından mağaza kodu al
        generate_qr(store_code)
    else:
        print("Lütfen bir mağaza kodu girin.")
