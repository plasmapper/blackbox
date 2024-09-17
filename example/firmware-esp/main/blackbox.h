#pragma once
#include "pl_blackbox.h"

//==============================================================================

class BlackBox : public PL::BlackBox {
public:
  PL::BlackBoxFirmwareInfo GetFirmwareInfo() override;
};