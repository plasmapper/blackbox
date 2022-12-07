#pragma once
#include "device.h"

//==============================================================================

class DeviceModbusServer : public PL::BlackBoxModbusServer {
public:
  DeviceModbusServer (std::shared_ptr<Device> device, std::shared_ptr<PL::Uart> uart, PL::ModbusProtocol protocol, uint8_t stationAddress);
  DeviceModbusServer (std::shared_ptr<Device> Device, uint16_t port);

private:
  std::shared_ptr<Device> device;

  #pragma pack(push, 1)
  struct ParameterData {
    uint16_t parameter1;
    uint16_t parameter2;
    uint16_t parameter3;
  };
  #pragma pack(pop)
  
  class ParameterHR : public PL::ModbusTypedMemoryArea<ParameterData> {
  public:
    ParameterHR (Device& device);
    esp_err_t OnRead() override;
    esp_err_t OnWrite() override;
  
  private:
    Device& device;
  };

  #pragma pack(push, 1)
  struct InputData {
    uint16_t input1;
    uint16_t input2;
    uint16_t input3;
  };
  #pragma pack(pop)

  class InputIR : public PL::ModbusTypedMemoryArea<InputData> {
  public:
    InputIR (Device& device);
    esp_err_t OnRead() override;
  
  private:
    Device& device;
  };

  void AddMemoryAreas();
};