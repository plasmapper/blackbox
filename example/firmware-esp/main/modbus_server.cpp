#include "settings.h"
#include "modbus_server.h"

//==============================================================================

DeviceModbusServer::DeviceModbusServer (std::shared_ptr<Device> device, std::shared_ptr<PL::Uart> uart, PL::ModbusProtocol protocol, uint8_t stationAddress) :
    PL::BlackBoxModbusServer (device, uart, protocol, stationAddress, modbusBufferSize), device (device) {
  AddMemoryAreas();
}

//==============================================================================

DeviceModbusServer::DeviceModbusServer (std::shared_ptr<Device> device, uint16_t port) :
    PL::BlackBoxModbusServer (device, port, modbusBufferSize), device (device) {
  AddMemoryAreas();
}

//==============================================================================

void DeviceModbusServer::AddMemoryAreas() {
  AddMemoryArea (std::make_shared<ParameterHR>(*device));
  AddMemoryArea (std::make_shared<InputIR>(*device));
  AddMemoryArea (PL::ModbusMemoryType::inputRegisters, graphModbusAddress, device->GetGraph());
}

//==============================================================================

DeviceModbusServer::ParameterHR::ParameterHR (Device& device) :
  PL::ModbusTypedMemoryArea<ParameterData>(PL::ModbusMemoryType::holdingRegisters, parameterModbusAddress), device (device) {}

//==============================================================================

esp_err_t DeviceModbusServer::ParameterHR::OnRead() {
  data->parameter1 = device.GetParameter1();
  data->parameter2 = device.GetParameter2();
  data->parameter3 = device.GetParameter3();
  return ESP_OK;
}

//==============================================================================

esp_err_t DeviceModbusServer::ParameterHR::OnWrite() {
  device.SetParameter1 (data->parameter1);
  device.SetParameter2 (data->parameter2);
  device.SetParameter3 (data->parameter3);
  return ESP_OK;
}

//==============================================================================

DeviceModbusServer::InputIR::InputIR (Device& device) :
  PL::ModbusTypedMemoryArea<InputData>(PL::ModbusMemoryType::inputRegisters, inputModbusAddress), device (device) {}

//==============================================================================

esp_err_t DeviceModbusServer::InputIR::OnRead() {
  data->input1 = device.GetInput1();
  data->input2 = device.GetInput2();
  data->input3 = device.GetInput3();
  return ESP_OK;
}