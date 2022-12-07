#include "settings.h"
#include "device.h"
#include "nvs_flash.h"
#include "driver/gpio.h"
#include "esp_check.h"

//==============================================================================

auto device = std::make_shared<Device>();

//==============================================================================

esp_err_t Initialize() {
  ESP_RETURN_ON_ERROR (esp_event_loop_create_default(), TAG, "event loop create default failed");
  ESP_RETURN_ON_ERROR (esp_netif_init(), TAG, "netif init failed");

  esp_err_t err = nvs_flash_init();
  if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
      ESP_RETURN_ON_ERROR (nvs_flash_erase(), TAG, "nvs flash erase failed");
      err = nvs_flash_init();
  }
  ESP_RETURN_ON_ERROR (err, TAG, "nvs flash init failed");

  ESP_RETURN_ON_ERROR (gpio_reset_pin (clearConfigurationPin), TAG, "pin configuration error");
  ESP_RETURN_ON_ERROR (gpio_set_direction (clearConfigurationPin, GPIO_MODE_INPUT), TAG, "pin configuration error");
  ESP_RETURN_ON_ERROR (gpio_set_pull_mode (clearConfigurationPin, GPIO_PULLUP_ONLY), TAG, "pin configuration error");

  ESP_RETURN_ON_ERROR (device->Initialize(), TAG, "device initialize failed");

  return ESP_OK;
}

//==============================================================================

extern "C" void app_main(void) {
  if (Initialize() != ESP_OK) {
    printf ("Initialization failed\n");
    return;
  } 

  for (TickType_t t = 0; t < clearConfigurationPinDelay; t++) {
    if (gpio_get_level (clearConfigurationPin))
      t = 0;
    vTaskDelay (1);

    device->RefreshGraph();
  }

  device->EraseConfiguration();
  device->Restart();
}

//==============================================================================
