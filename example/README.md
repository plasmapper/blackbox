# BlackBox Firmware and Software Examples

This example contains firmware and client software that demonstrate the use of the correspondent libraries.

## Controller Features

1. Configurable UART and Wi-Fi interfaces.
2. Configurable UART and network Modbus servers.
3. mDNS server to simplify the device discovery on the LAN.
4. Example of user-defined writable parameters, read-only parameters and read-only float data array.

## ESP-IDF Firmware

1. Download and flash the [hardware information firmware](hardware-esp).
2. Download and flash the [main firmware](firmware-esp).

## LabVIEW Software

1. Download the [software](software-labview).
2. Install the required VIPM packages using `Test Device.vipc`.
3. Configure the device interfaces and servers using the `BlackBox Configurator.vi` from the `PL BlackBox Modbus Client` package examples.
4. Use `Test Device.vi` to control the device.
