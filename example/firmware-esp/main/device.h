#pragma once
#include "pl_blackbox.h"

//==============================================================================

class Device : public PL::BlackBox, public std::enable_shared_from_this<Device> {
public:
  esp_err_t Initialize();
  PL::BlackBoxFirmwareInfo GetFirmwareInfo() override;

  void LoadConfiguration() override;
  void SaveConfiguration() override;
  void EraseConfiguration() override;

  void RefreshGraph();

  uint16_t GetParameter1();
  void SetParameter1 (uint16_t value);
  uint16_t GetParameter2();
  void SetParameter2 (uint16_t value);
  uint16_t GetParameter3();
  void SetParameter3 (uint16_t value);

  uint16_t GetInput1();
  uint16_t GetInput2();
  uint16_t GetInput3();

  std::shared_ptr<PL::TypedBuffer<float>> GetGraph();

private:
  uint16_t parameter1;
  uint16_t parameter2;
  uint16_t parameter3;

  std::shared_ptr<PL::TypedBuffer<float>> graph = std::make_shared<PL::TypedBuffer<float>>(graphSize);
};