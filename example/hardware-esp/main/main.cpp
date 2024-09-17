#include "pl_blackbox.h"
#include "nvs_flash.h"
#include "esp_mac.h"
#include "esp_check.h"

//==============================================================================

static const char* TAG = "flash_hardware_info";

//==============================================================================

// Hardware name, version and UID (WiFi MAC address if empty)
PL::BlackBoxHardwareInfo hardwareInfo = {"TestHardware", {1, 0, 0}, ""};

//==============================================================================

esp_err_t FlashHardwareInfo () {
  PL::NvsNamespace nvsNamespace(PL::BlackBox::defaultHardwareInfoNvsNamespaceName, PL::NvsAccessMode::readWrite);
  
  ESP_RETURN_ON_ERROR(nvsNamespace.Write(PL::BlackBox::hardwareInfoNameNvsKey, hardwareInfo.name), TAG, "write name failed");
  ESP_RETURN_ON_ERROR(nvsNamespace.Write(PL::BlackBox::hardwareInfoMajorVersionNvsKey, hardwareInfo.version.major), TAG, "write version major failed");
  ESP_RETURN_ON_ERROR(nvsNamespace.Write(PL::BlackBox::hardwareInfoMinorVersionNvsKey, hardwareInfo.version.minor), TAG, "write version minor failed");
  ESP_RETURN_ON_ERROR(nvsNamespace.Write(PL::BlackBox::hardwareInfoPatchVersionNvsKey, hardwareInfo.version.patch), TAG, "write version patch failed");
  ESP_RETURN_ON_ERROR(nvsNamespace.Write(PL::BlackBox::hardwareInfoUidNvsKey, hardwareInfo.uid), TAG, "write UID failed");

  return ESP_OK;
}

//==============================================================================

extern "C" void app_main(void) {
  ESP_ERROR_CHECK (esp_netif_init());
  esp_err_t err = nvs_flash_init();
  if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
      ESP_ERROR_CHECK (nvs_flash_erase());
      err = nvs_flash_init();
  }
  ESP_ERROR_CHECK (err);

  if (hardwareInfo.uid.empty()) {
    uint8_t mac[6];
    esp_read_mac (mac, ESP_MAC_WIFI_STA);
    char macString[13];
    sprintf (macString, "%02X%02X%02X%02X%02X%02X", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
    hardwareInfo.uid = macString;
  }

  printf ("Flashing hardware info:\nDevice name: %s\nVersion: %d.%d.%d\nUID: %s\n",
    hardwareInfo.name.c_str(), hardwareInfo.version.major, hardwareInfo.version.minor, hardwareInfo.version.patch, hardwareInfo.uid.c_str());

  if (FlashHardwareInfo () == ESP_OK)
    printf ("Done\n");
}