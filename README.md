# APKS Installer

[🇷🇺 Читать на русском](#русский) | [🇬🇧 Read in English](#english)

---

## <a name="русский"></a>🇷🇺 Русский

### Описание

**APKS Installer** — это простое приложение с графическим интерфейсом для установки `.apks` пакетов на Android-устройства через ADB. Я нашёл способ установки приложений формата `.apks` на Android 15+ и решил сделать под него интерфейс для простой установки. На моём Realme GT 5 PRO (Android 15) всё прекрасно устанавливается этим способом. Это решение особенно полезно для тех, у кого не получается установить `.apks` другими способами (например, через SAI или командную строку). Мне самому помог именно этот способ, поэтому я решил сделать для него удобный интерфейс.

### Скачать

- [Скачать для Windows x86 (32-bit)](https://github.com/MaKrotos/APKS-installer/releases/download/Apks_Installer/win-x86.zip)
- [Скачать для Windows x64 (64-bit)](https://github.com/MaKrotos/APKS-installer/releases/download/Apks_Installer/win-x64.zip)

### Возможности

- Установка `.apks` (мульти-APK) пакетов на Android-устройства, включая Android 15 и выше
- Автоматическое определение подключённых устройств
- Локализация интерфейса (русский/английский)
- Логи процесса установки
- Поддержка drag-and-drop и выбора файла через диалог

### Требования

- Windows
- Подключённое Android-устройство по USB
- Включённая отладка по USB на устройстве
- Разрешение на отладку для этого компьютера (при первом подключении подтвердите на устройстве)

### Инструкция по использованию

1. **Включите отладку по USB** на вашем Android-устройстве:
   - Откройте настройки → О телефоне → Несколько раз нажмите на "Номер сборки", чтобы включить режим разработчика.
   - В настройках разработчика включите "Отладка по USB".
2. **Подключите устройство к компьютеру** через USB-кабель.
3. **Разрешите отладку** для этого компьютера на экране устройства (при первом подключении появится запрос).
4. **Запустите APKS Installer**.
5. **Выберите файл `.apks`**:
   - Перетащите файл в окно программы **или**
   - Нажмите "Выбрать файл" и укажите путь к вашему `.apks`.
6. **Выберите устройство** из списка (если их несколько).
7. Нажмите **"Установить"**.
8. Дождитесь завершения процесса — результат появится в логах и всплывающем окне.

### Примечания

- Если устройство не отображается, убедитесь, что драйверы установлены и отладка разрешена.
- Программа использует встроенный `adb.exe`, ничего дополнительно устанавливать не нужно.
- Поддерживаются только `.apks`-файлы (мульти-APK, обычно сгенерированные Google Play или SAI).
- Особо полезно для установки `.apks` на Android 15 и выше.

---

## <a name="english"></a>🇬🇧 English

### Description

**APKS Installer** is a simple GUI application for installing `.apks` packages on Android devices via ADB. I found a way to install `.apks` format applications on Android 15+ and decided to create an interface for easy installation. On my Realme GT 5 PRO (Android 15), everything installs perfectly using this method. This solution is especially useful for those who cannot install `.apks` by other means (e.g., via SAI or command line). This method helped me personally, so I decided to create a user-friendly interface for it.

### Download

- [Download for Windows x86 (32-bit)](https://github.com/MaKrotos/APKS-installer/releases/download/Apks_Installer/win-x86.zip)
- [Download for Windows x64 (64-bit)](https://github.com/MaKrotos/APKS-installer/releases/download/Apks_Installer/win-x64.zip)

### Features

- Install `.apks` (multi-APK) packages to Android devices, including Android 15 and above
- Automatic device detection
- Interface localization (Russian/English)
- Installation process logs
- Drag-and-drop and file dialog support

### Requirements

- Windows
- Android device connected via USB
- USB debugging enabled on the device
- USB debugging authorization for this computer (confirm on device when prompted)

### Usage Instructions

1. **Enable USB debugging** on your Android device:
   - Go to Settings → About phone → Tap "Build number" several times to enable Developer options.
   - In Developer options, enable "USB debugging".
2. **Connect your device to the computer** via USB cable.
3. **Authorize USB debugging** for this computer on your device (confirm the prompt on the device screen).
4. **Run APKS Installer**.
5. **Select your `.apks` file**:
   - Drag and drop the file into the application window **or**
   - Click "Select file" and choose your `.apks` file.
6. **Select your device** from the list (if more than one is connected).
7. Click **"Install"**.
8. Wait for the process to finish — the result will appear in the logs and a popup window.

### Notes

- If your device is not listed, make sure drivers are installed and debugging is authorized.
- The program uses the built-in `adb.exe`, no additional installation is required.
- Only `.apks` files are supported (multi-APK, usually generated by Google Play or SAI).
- Especially useful for installing `.apks` on Android 15 and above.
