#pragma once
#include "pl_blackbox.h"
#include "data_types.h"

//==============================================================================

class Parameters : public PL::TypedBuffer<ParametersData>, public PL::BlackBoxConfiguration {
public:
  Parameters(std::string nvsNamespaceName);

  void Load() override;
  void Save() override;
  void Erase() override;

private:
  std::string nvsNamespaceName;
};