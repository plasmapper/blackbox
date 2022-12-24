#pragma once
#include "pl_blackbox.h"
#include "driver/gpio.h"

//==============================================================================

const char * const TAG = "test_device";

const PL::BlackBoxFirmwareInfo firmwareInfo = {"TestFirmware", {1, 0, 0}};
const std::string defaultDeviceName = "TestDevice";

const size_t graphSize = 50;

const std::string deviceNvsNamespace = "testDevice";
const std::string parameter1NvsKey = "param1";
const std::string parameter2NvsKey = "param2";
const std::string parameter3NvsKey = "param3";

const uart_port_t modbusSerialServerUartPort = UART_NUM_0;
const uint32_t defaultModbusSerialServerUartBaudrate = 115200;
const PL::UartParity defaultModbusSerialServerUartParity = PL::UartParity::even;
const bool defaultModbusSerialServerUartEnabled = true;

const bool defaultWiFiEnabled = true;
const bool defaultWiFiIpV4DhcpClientEnabled = true;

const bool defaultModbusSerialServerEnabled = true;
const bool defaultModbusNetworkServerEnabled = true;
const bool defaultMdnsServerEnabled = true;

const size_t modbusBufferSize = 500;

const std::string modbusSerialServerName = "Modbus Serial Server";
const PL::ModbusProtocol defaultModbusSerialServerProtocol = PL::ModbusProtocol::rtu;
const uint8_t defaultModbusSerialServerStationAddress = 1;

const std::string modbusNetworkServerName = "Modbus TCP Server";
const uint16_t defaultModbusNetworkServerPort = 502;

const gpio_num_t clearConfigurationPin = GPIO_NUM_0;
const TickType_t clearConfigurationPinDelay = 5000 / portTICK_PERIOD_MS;

const uint16_t parameterModbusAddress = 1000;
const uint16_t inputModbusAddress = 1000;
const uint16_t graphModbusAddress = 2000;