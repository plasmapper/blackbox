#include "parameters.h"
#include "settings.h"

//==============================================================================

Parameters::Parameters(std::string nvsNamespaceName) : nvsNamespaceName(nvsNamespaceName) {}

//==============================================================================

void Parameters::Load() {
  PL::LockGuard lg(*this);
  PL::NvsNamespace nvsNamespace(nvsNamespaceName, PL::NvsAccessMode::readOnly);
  uint16_t u16Value;

  if (nvsNamespace.Read(settings.parameters.configuration.parameter1NvsKey, u16Value) == ESP_OK)
    data->parameter1 = u16Value;
  if (nvsNamespace.Read(settings.parameters.configuration.parameter2NvsKey, u16Value) == ESP_OK)
    data->parameter2 = u16Value;
  if (nvsNamespace.Read(settings.parameters.configuration.parameter3NvsKey, u16Value) == ESP_OK)
    data->parameter3 = u16Value;
}

//==============================================================================

void Parameters::Save() {
  PL::LockGuard lg(*this);
  PL::NvsNamespace nvsNamespace(nvsNamespaceName, PL::NvsAccessMode::readWrite);

  nvsNamespace.Write(settings.parameters.configuration.parameter1NvsKey, data->parameter1);
  nvsNamespace.Write(settings.parameters.configuration.parameter2NvsKey, data->parameter2);
  nvsNamespace.Write(settings.parameters.configuration.parameter3NvsKey, data->parameter3);
}

//==============================================================================

void Parameters::Erase() {
  PL::LockGuard lg(*this);
  PL::NvsNamespace nvsNamespace(nvsNamespaceName, PL::NvsAccessMode::readWrite);
  nvsNamespace.Erase();
}