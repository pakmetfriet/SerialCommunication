Repository overview

This repository contains two cooperating components for serial I/O experiments:

- Arduino sketch: SerialCommunication.ino — an Arduino sketch that embeds a SerialCommand-based command parser (SerialCommand.h/.cpp) and wiring_analog.c helpers. It exposes textual commands over the serial port to read analog inputs, control digital outputs, and PWM.
- Desktop app: SerialCommunication (Windows Forms project) — a small GUI that enumerates COM ports, configures serial parameters, and connects/disconnects to a device and displays status.

Build, test, and lint commands

Arduino (build and upload)
- Open Arduino IDE and load SerialCommunication.ino. Select board (Arduino Uno typical) and Correct COM port. Click Upload.
- arduino-cli (command-line):
  - compile: arduino-cli compile --fqbn arduino:avr:uno "C:\Users\Kilia\source\repos\SerialCommunication\SerialCommunication.ino"
  - upload:  arduino-cli upload -p COM3 --fqbn arduino:avr:uno "C:\Users\Kilia\source\repos\SerialCommunication\SerialCommunication.ino"
  - adjust --fqbn and -p for your board/port. The sketch assumes typical AVR Uno pinout.

Windows desktop app (build/run)
- Visual Studio: Open SerialCommunication.slnx and Build Solution (recommended).
- Command line (Developer Command Prompt): msbuild "SerialCommunication.slnx"
- If the csproj is SDK-style, dotnet build "SerialCommunication\SerialCommunication.csproj" may also work.

Tests / Linting
- No unit tests or linters are present in this repo. Add CI and test projects if desired.

High-level architecture (big picture)

- Serial protocol: plain-text commands parsed by SerialCommand. Commands are tokenized by whitespace and terminated by a newline/terminator. Handlers are registered via sCmd.addCommand(...). The sketch sets sCmd.setDefaultHandler(onUnknownCommand).
- Commands implemented in the sketch:
  - set dN <on|off|high|low|1|0> — set digital pins (d2..d4)
  - set pwmN <0..255> — analogWrite on PWM pins (9..11)
  - toggle dN — toggle digital output (d2..d4)
  - get dN — prints "dN: <0|1>" for d2..d7
  - get aN — prints "aN: <value>" for a0..a5 (uses analogReadDelay)
  - ping / debug / help
- Desktop app: enumerates COM ports, configures BaudRate (default 115200), parity, data bits, stopbits, handshake, DTR/RTS. Connect() opens SerialPort and updates UI; Disconnect() closes it. The app is a simple control surface for sending/receiving serial data.

Key conventions and repo-specific patterns

- SerialCommand usage:
  - Use sCmd.addCommand("name", handler) to register handlers and sCmd.setDefaultHandler(...) for unknown commands.
  - Handlers call sCmd.next() to iterate arguments. sCmd.clearBuffer() resets after unknowns or completion.
  - The SerialCommand buffer size and max commands are defined in SerialCommand.h (SERIALCOMMANDBUFFER, MAXSERIALCOMMANDS).
- Command format conventions:
  - Tokenized by whitespace; commands are ASCII text. Commands in this sketch assume lower-case keywords and short prefixes (dN, aN, pwmN).
  - Responses are human-readable lines: e.g., "d3: 1" or "a2: 512" and short status replies like "set done".
- Pin ranges are hard-coded and validated in the sketch:
  - Digital outputs: 2..4 (set/toggle)
  - PWM outputs: 9..11 (set pwm)
  - Digital inputs: 5..7 (get d)
  - Analog inputs: A0..A5 (get a)
  - Do not assume other pins are safe to use without editing the sketch.
- Serial settings: default BaudRate 115200. The desktop app defaults to 115200 in the UI.
- Debug toggle: SerialCommand has a SERIALCOMMANDDEBUG macro — in the shipped files it is defined then immediately undefined; uncomment definition to enable verbose echo from SerialCommand.

Repository housekeeping / AI assistant notes

- No existing AI assistant config files (CLAUDE.md, .cursorrules, AGENTS.md, etc.) were found.
- This file (copilot-instructions.md) documents the repo-specific build steps and conventions Copilot sessions should know.

If you want changes

If anything needs more detail (e.g., exact msbuild/dotnet target, alternate Arduino boards, or CI steps), say which area to expand and include preferred commands or CI providers.
