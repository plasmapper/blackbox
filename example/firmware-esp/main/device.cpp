#include "settings.h"
#include "device.h"
#include "modbus_server.h"
#include "pl_mdns.h"
#include "pl_nvs.h"
#include "math.h"
#include "esp_check.h"

//==============================================================================

esp_err_t Device::Initialize() {
  PL::BlackBoxHardwareInfo hardwareInfo = GetHardwareInfo();
  ESP_RETURN_ON_FALSE (hardwareInfo.name == "TestHardware" && hardwareInfo.version.major == 1, ESP_ERR_INVALID_VERSION, TAG, "hardware %s v%d.%d.%d is not supported", \
                       hardwareInfo.name.c_str(), hardwareInfo.version.major, hardwareInfo.version.minor, hardwareInfo.version.patch);

  SetName (defaultDeviceName);

  // Initialize and add hardware interfaces
  auto uart = std::make_shared<PL::Uart> (modbusSerialServerUartPort);
  ESP_RETURN_ON_ERROR (uart->Initialize(), TAG, "uart initialize failed");
  AddHardwareInterface (uart);
  auto wifi = std::make_shared<PL::EspWiFiStation>();
  ESP_RETURN_ON_ERROR (wifi->Initialize(), TAG, "Wi-Fi initialize failed");
  AddHardwareInterface (wifi);

  // Add servers
  auto modbusSerialServer = std::make_shared<DeviceModbusServer> (shared_from_this(), uart, defaultModbusSerialServerProtocol, defaultModbusSerialServerStationAddress);
  modbusSerialServer->SetName (modbusSerialServerName);
  AddServer (modbusSerialServer);
  auto modbusNetworkServer = std::make_shared<DeviceModbusServer> (shared_from_this(), defaultModbusNetworkServerPort);
  modbusNetworkServer->SetName (modbusNetworkServerName);
  AddServer (modbusNetworkServer);
  auto mdnsServer = std::make_shared<PL::MdnsServer>(GetHardwareInfo().uid);
  AddServer (mdnsServer);

  // Set default configuration
  ESP_RETURN_ON_ERROR (uart->SetBaudRate (defaultModbusSerialServerUartBaudrate), TAG, "uart set default baud rate failed");
  ESP_RETURN_ON_ERROR (uart->SetParity (defaultModbusSerialServerUartParity), TAG, "uart set default parity failed");
  ESP_RETURN_ON_ERROR (defaultModbusSerialServerUartEnabled ? uart->Enable() : ESP_OK, TAG, "uart enable failed");
  ESP_RETURN_ON_ERROR (defaultWiFiEnabled ? wifi->Enable() : ESP_OK, TAG, "Wi-Fi enable failed");
  ESP_RETURN_ON_ERROR (defaultModbusSerialServerEnabled ? modbusSerialServer->Enable() : ESP_OK, TAG, "Modbus serial server enable failed");
  ESP_RETURN_ON_ERROR (defaultModbusNetworkServerEnabled ? modbusNetworkServer->Enable() : ESP_OK, TAG, "Modbus network server  enable failed");
  ESP_RETURN_ON_ERROR (defaultMdnsServerEnabled ? mdnsServer->Enable() : ESP_OK, TAG, "mDNS server enable failed");

  {
    PL::LockGuard lg (*graph);
    memset (graph->data, 0, graph->size);
  }

  // Load configuration from the NVS
  LoadConfiguration();
  
  // Add the Modbus network server as an mDNS service
  if (auto baseServer = modbusNetworkServer->GetBaseServer().lock()) {
    if (auto networkServer = std::dynamic_pointer_cast<PL::NetworkServer>(baseServer)) {
      ESP_RETURN_ON_ERROR (mdnsServer->AddService (networkServer, GetName(), "_mbap", "_tcp"), TAG, "add mDNS service failed");
    }      
  }

  return ESP_OK;
}

//==============================================================================

PL::BlackBoxFirmwareInfo Device::GetFirmwareInfo() {
  return firmwareInfo;
}

//==============================================================================

void Device::LoadConfiguration() {
  PL::BlackBox::LoadConfiguration();

  PL::NvsNamespace configurationNvs (deviceNvsNamespace, PL::NvsAccessMode::readOnly);
  uint16_t u16Value;
  
  if (configurationNvs.Read (parameter1NvsKey, u16Value) == ESP_OK)
    SetParameter1 (u16Value);
  if (configurationNvs.Read (parameter2NvsKey, u16Value) == ESP_OK)
    SetParameter2 (u16Value);
  if (configurationNvs.Read (parameter3NvsKey, u16Value) == ESP_OK)
    SetParameter3 (u16Value);
}

//==============================================================================

void Device::SaveConfiguration() {
  PL::BlackBox::SaveConfiguration();

  PL::NvsNamespace configurationNvs (deviceNvsNamespace, PL::NvsAccessMode::readWrite);  
  configurationNvs.Erase();

  configurationNvs.Write (parameter1NvsKey, GetParameter1());
  configurationNvs.Write (parameter2NvsKey, GetParameter2());
  configurationNvs.Write (parameter3NvsKey, GetParameter3());
}

//==============================================================================

void Device::EraseConfiguration() {
  PL::BlackBox::EraseConfiguration();

  PL::NvsNamespace configurationNvs (deviceNvsNamespace, PL::NvsAccessMode::readWrite);  
  configurationNvs.Erase();
}

//==============================================================================

void Device::RefreshGraph() {
  PL::LockGuard lg (*graph);
  float phase = 2 * M_PI * std::rand() / RAND_MAX;
  for (int i = 0; i < graphSize; i++)
    graph->data[i] = sin (i * 2 * M_PI / graphSize + phase);
}

//==============================================================================

uint16_t Device::GetParameter1() {
  PL::LockGuard lg (*this);
  return parameter1;
}

//==============================================================================

void Device::SetParameter1 (uint16_t value) {
  PL::LockGuard lg (*this);
  parameter1 = value;
}

//==============================================================================

uint16_t Device::GetParameter2() {
  PL::LockGuard lg (*this);
  return parameter2;
}

//==============================================================================

void Device::SetParameter2 (uint16_t value) {
  PL::LockGuard lg (*this);
  parameter2 = value;
}

//==============================================================================

uint16_t Device::GetParameter3() {
  PL::LockGuard lg (*this);
  return parameter3;
}

//==============================================================================

void Device::SetParameter3 (uint16_t value) {
  PL::LockGuard lg (*this);
  parameter3 = value;
}

//==============================================================================

uint16_t Device::GetInput1() {
  PL::LockGuard lg (*this);
  return parameter1 + 1;
}

//==============================================================================

uint16_t Device::GetInput2() {
  PL::LockGuard lg (*this);
  return parameter2 + 1;
}

//==============================================================================

uint16_t Device::GetInput3() {
  PL::LockGuard lg (*this);
  return parameter3 + 1;
}

//==============================================================================

std::shared_ptr<PL::TypedBuffer<float>> Device::GetGraph() {
  PL::LockGuard lg (*this);
  return graph;
}