# BlackBox

This is a generic controller firmware library and protocol specification that I made for myself to unify and simplify the device configuration and the client-server interaction.

## Requirements

The firmware library and the protocol should have the following capabilities.

1. Restart the controller.
2. Load, save and erase the controller configuration.
3. Read and write the device name.
4. Read the hardware information (name, version and UID).
5. Read the firmware information (name, version).
6. Read and write the hardware interface information and configuration.
7. Read and write the server information and configuration.
8. Read and write the controller restart flag.

## Protocols

[BlackBox Modbus protocol](modbus.md)

## Implementations

[BlackBox C++ Class Component for ESP-IDF](https://github.com/plasmapper/blackbox-esp-cpp)  
[BlackBox Modbus Client Library for LabVIEW](https://github.com/plasmapper/blackbox-modbus-client-labview)  

## Examples

[BlackBox Firmware and Software Examples](examples)
