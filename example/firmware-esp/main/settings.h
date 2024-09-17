#pragma once
#include "pl_blackbox.h"
#include "driver/gpio.h"

//==============================================================================

const PL::BlackBoxFirmwareInfo firmwareInfo = {"TestFirmware", {1, 0, 0}};
const std::string defaultDeviceName = "TestDevice";

const struct {
  
  // Erase configuration pin
  const struct {
    const gpio_num_t pin = GPIO_NUM_0;
    const TickType_t delay = 5000 / portTICK_PERIOD_MS;
  } eraseConfiguration;

  // Parameters
  const struct {
    const struct {
      const std::string nvsNamespaceName = "parameters";
      const std::string parameter1NvsKey = "param1";
      const std::string parameter2NvsKey = "param2";
      const std::string parameter3NvsKey = "param3";
    } configuration;
  } parameters;
  
  // Graph
  const struct {
    const size_t size = 50;
  } graph;

  // UART
  const struct {
    const bool disableEnabledValidation = false;
    const uart_port_t port = UART_NUM_0;
    const uint32_t defaultBaudrate = 115200;
    const std::vector<uint32_t> validBaudrates = { 9600, 19200, 38400, 57600, 115200 };
    const PL::UartParity defaultParity = PL::UartParity::even;
    const std::vector<PL::UartParity> validParities = { PL::UartParity::none, PL::UartParity::even, PL::UartParity::odd };
    const PL::UartStopBits defaultStopBits = PL::UartStopBits::one;
    const std::vector<PL::UartStopBits> validStopBits = { PL::UartStopBits::one, PL::UartStopBits::two };
    const struct {
      const std::string nvsNamespaceName = "uart";
    } configuration;
  } uart;

  // Wi-Fi
  const struct {
    const bool disableEnabledValidation = true;
    const bool disableSsidValidation = true;
    const bool disablePasswordValidation = true;
    const bool disableIpV4Validation = true;

    const struct {
      const bool defaultEnabled = true;
      const bool disableEnabledValidation = true;
    } dhcpClient;

    const struct {
      const std::string nvsNamespaceName = "wifi";
    } configuration;
  } wifi;

  // Modbus
  const struct {
    const size_t bufferSize = 500;
    const uint16_t parametersAddress = 1000;
    const uint16_t inputsAddress = 1000;
    const uint16_t graphAddress = 2000;

    // UART server
    const struct {
      const bool disableEnabledValidation = false;
      const std::string name = "Modbus UART Server";
      const PL::ModbusProtocol defaultProtocol = PL::ModbusProtocol::rtu;
      const std::vector<PL::ModbusProtocol> validProtocols = { PL::ModbusProtocol::rtu, PL::ModbusProtocol::ascii };
      const uint8_t defaultStationAddress = 1;
      const bool disableStationAddressValidation = true;
      const struct {
        const std::string nvsNamespaceName = "uartMbSrv";
      } configuration;
    } uartServer;

    // Network server
    const struct {
      const bool disableEnabledValidation = true;
      const std::string name = "Modbus TCP Server";
      const uint16_t defaultPort = 502;
      const bool disablePortValidation = true;
      const bool disableMaxNumberOfClientsValidation = true;
      const struct {
        const std::string nvsNamespaceName = "netMbSrv";
      } configuration;
    } networkServer;
  } modbus;

  // mDNS server
  const struct {
    const bool disableEnabledValidation = true;
    const struct {
      const std::string nvsNamespaceName = "mdns";
    } configuration;
  } mdnsServer;
} settings;