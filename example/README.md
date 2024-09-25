# BlackBox Firmware and Software Examples

This example contains firmware and client software that demonstrate the use of the correspondent libraries.

## Controller Features

1. Configurable UART and Wi-Fi interfaces.
2. Configurable UART and network Modbus servers.
3. mDNS server to simplify the device discovery on the LAN.
4. Example of user-defined writable parameters (Modbus holding registers 1000..1002).
5. Example of user-defined read-only parameters (Modbus input registers 1000..1002).
6. Example of a read-only float data array (Modbus input registers 2000..2099).

## ESP-IDF Firmware

1. Download and flash the [hardware information firmware](hardware-esp).
2. Download and flash the [main firmware](firmware-esp).

## LabVIEW Software

1. Download the [software](software-labview).
2. Install the required VIPM packages using `Test Device.vipc`.
3. Configure the device interfaces and servers using the `BlackBox Configurator.vi` from the `PL BlackBox Modbus Client` package examples.
4. Use `Test Device.vi` to control the device.

## .NET Software

1. Download and build the [BlackBox Configurator](https://github.com/plasmapper/blackbox-dotnet/tree/main/Examples/BlackBoxConfigurator) to configure the device interfaces and servers.
2. Download and build the [software](software-dotnet) to control the device.