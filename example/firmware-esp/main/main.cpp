#include "nvs_flash.h"
#include "driver/gpio.h"
#include "esp_check.h"

#include "settings.h"
#include "data_types.h"
#include "parameters.h"
#include "blackbox.h"
#include "math.h"

//==============================================================================

const char * const TAG = "test_device";

//==============================================================================

auto blackBox = std::make_shared<BlackBox>();

auto parameters = std::make_shared<Parameters>(settings.parameters.configuration.nvsNamespaceName);
auto inputs = std::make_shared<PL::TypedBuffer<InputsData>>();
auto graph = std::make_shared<PL::TypedBuffer<float>>(settings.graph.size);

auto uart = std::make_shared<PL::Uart>(settings.uart.port);
auto wifi = std::make_shared<PL::EspWiFiStation>();

auto uartModbusServer = std::make_shared<PL::BlackBoxModbusServer>(blackBox, uart,
  settings.modbus.uartServer.defaultProtocol, settings.modbus.uartServer.defaultStationAddress, settings.modbus.bufferSize);
auto networkModbusServer = std::make_shared<PL::BlackBoxModbusServer>(blackBox, settings.modbus.networkServer.defaultPort, settings.modbus.bufferSize);
auto mdnsServer = std::make_shared<PL::MdnsServer>(defaultDeviceName);

//==============================================================================

esp_err_t Initialize() {
  ESP_RETURN_ON_ERROR(esp_event_loop_create_default(), TAG, "event loop create default failed");
  ESP_RETURN_ON_ERROR(esp_netif_init(), TAG, "netif init failed");

  esp_err_t err = nvs_flash_init();
  if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
      ESP_RETURN_ON_ERROR(nvs_flash_erase(), TAG, "nvs flash erase failed");
      err = nvs_flash_init();
  }
  ESP_RETURN_ON_ERROR(err, TAG, "nvs flash init failed");

  // Check hardware info
  PL::BlackBoxHardwareInfo hardwareInfo = blackBox->GetHardwareInfo();
  ESP_RETURN_ON_FALSE(hardwareInfo.name == "TestHardware" && hardwareInfo.version.major == 1, ESP_ERR_INVALID_VERSION, TAG, "hardware %s v%d.%d.%d is not supported", \
                       hardwareInfo.name.c_str(), hardwareInfo.version.major, hardwareInfo.version.minor, hardwareInfo.version.patch);

  // Initialize erase configuration pin
  ESP_RETURN_ON_ERROR(gpio_reset_pin(settings.eraseConfiguration.pin), TAG, "pin configuration error");
  ESP_RETURN_ON_ERROR(gpio_set_direction(settings.eraseConfiguration.pin, GPIO_MODE_INPUT), TAG, "pin configuration error");
  ESP_RETURN_ON_ERROR(gpio_set_pull_mode(settings.eraseConfiguration.pin, GPIO_PULLUP_ONLY), TAG, "pin configuration error");

  // Initialize graph
  {
    PL::LockGuard lg(*graph);
    memset(graph->data, 0, graph->size);
  }

  // Set default device name
  blackBox->SetDeviceName(defaultDeviceName);

  // Add parameter configuration
  blackBox->AddConfiguration(parameters);

  // Initialize UART and its configuration
  ESP_RETURN_ON_ERROR(uart->Initialize(), TAG, "uart initialize failed");
  ESP_RETURN_ON_ERROR(uart->SetBaudRate(settings.uart.defaultBaudrate), TAG, "uart set default baud rate failed");
  ESP_RETURN_ON_ERROR(uart->SetParity(settings.uart.defaultParity), TAG, "uart set default parity failed");
  ESP_RETURN_ON_ERROR(uart->SetStopBits(settings.uart.defaultStopBits), TAG, "uart set default stop bits failed");
  auto uartConfiguration = blackBox->AddUartConfiguration(uart, settings.uart.configuration.nvsNamespaceName);
  if (settings.uart.disableEnabledValidation)
    uartConfiguration->enabled.DisableValueValidation();
  uartConfiguration->baudRate.SetValidValues(settings.uart.validBaudrates);
  uartConfiguration->parity.SetValidValues(settings.uart.validParities);
  uartConfiguration->stopBits.SetValidValues(settings.uart.validStopBits);

  // Initialize Wi-Fi and its configuration
  ESP_RETURN_ON_ERROR(wifi->Initialize(), TAG, "Wi-Fi initialize failed");
  ESP_RETURN_ON_ERROR(settings.wifi.dhcpClient.defaultEnabled ? wifi->EnableIpV4DhcpClient() : ESP_OK, TAG, "Wi-Fi IPv4 DHCP client enable failed");
  auto wifiConfiguration = blackBox->AddWiFiConfiguration(wifi, settings.wifi.configuration.nvsNamespaceName);
  if (settings.wifi.disableEnabledValidation)
    wifiConfiguration->enabled.DisableValueValidation();
  if (settings.wifi.disableSsidValidation)
    wifiConfiguration->ssid.DisableValueValidation();
  if (settings.wifi.disablePasswordValidation)
    wifiConfiguration->password.DisableValueValidation();
  if (settings.wifi.disableIpV4Validation) {
    wifiConfiguration->ipV4Address.DisableValueValidation();
    wifiConfiguration->ipV4Netmask.DisableValueValidation();
    wifiConfiguration->ipV4Gateway.DisableValueValidation();
  }
  if (settings.wifi.dhcpClient.disableEnabledValidation)
    wifiConfiguration->ipV4DhcpClientEnabled.DisableValueValidation();

  // Initialize UART Modbus server and its configuration
  uartModbusServer->SetName(settings.modbus.uartServer.name);
  auto uartModbusServerConfiguration = blackBox->AddModbusServerConfiguration(uartModbusServer, settings.modbus.uartServer.configuration.nvsNamespaceName);
  if (settings.modbus.uartServer.disableEnabledValidation)
    uartModbusServerConfiguration->enabled.DisableValueValidation();
  uartModbusServerConfiguration->protocol.SetValidValues(settings.modbus.uartServer.validProtocols);
  if (settings.modbus.uartServer.disableStationAddressValidation)
    uartModbusServerConfiguration->stationAddress.DisableValueValidation();

  // Initialize network Modbus server and its configuration
  networkModbusServer->SetName(settings.modbus.networkServer.name);
  auto networkModbusServerConfiguration = blackBox->AddModbusServerConfiguration(networkModbusServer, settings.modbus.networkServer.configuration.nvsNamespaceName);
  if (settings.modbus.networkServer.disableEnabledValidation)
    networkModbusServerConfiguration->enabled.DisableValueValidation();
  if (settings.modbus.networkServer.disablePortValidation)
    networkModbusServerConfiguration->port.DisableValueValidation();
  if (settings.modbus.networkServer.disableMaxNumberOfClientsValidation)
    networkModbusServerConfiguration->maxNumberOfClients.DisableValueValidation();
  
  // Modbus memory areas
  for (auto& modbusServer : { uartModbusServer, networkModbusServer } ) {
    modbusServer->AddMemoryArea(PL::ModbusMemoryType::holdingRegisters, settings.modbus.parametersAddress, parameters);
    modbusServer->AddMemoryArea(PL::ModbusMemoryType::inputRegisters, settings.modbus.inputsAddress, inputs);
    modbusServer->AddMemoryArea(PL::ModbusMemoryType::inputRegisters, settings.modbus.graphAddress, graph);
  }

  // Initialize mDNS server and its configuration
  mdnsServer->SetHostname(blackBox->GetHardwareInfo().uid);
  auto mdnsServerConfiguration = blackBox->AddMdnsServerConfiguration(mdnsServer, settings.mdnsServer.configuration.nvsNamespaceName);
  if (settings.mdnsServer.disableEnabledValidation)
    mdnsServerConfiguration->enabled.DisableValueValidation();

  // Load and apply configurations
  blackBox->LoadAllConfigurations();
  blackBox->ApplyHardwareInterfaceConfigurations();
  blackBox->ApplyServerConfigurations();

  // Add the Modbus network server as an mDNS service
  if (auto baseServer = networkModbusServer->GetBaseServer().lock()) {
    if (auto networkServer = std::dynamic_pointer_cast<PL::NetworkServer>(baseServer)) {
      ESP_RETURN_ON_ERROR(mdnsServer->AddService(networkServer, blackBox->GetDeviceName(), "_mbap", "_tcp"), TAG, "add mDNS service failed");
    }      
  }

  return ESP_OK;
}

//==============================================================================

extern "C" void app_main(void) {
  if (Initialize() != ESP_OK) {
    printf("Initialization failed\n");
    return;
  } 

  for (TickType_t t = 0; t < settings.eraseConfiguration.delay; t++) {
    if (gpio_get_level(settings.eraseConfiguration.pin))
      t = 0;
    vTaskDelay(1);

    {
      PL::LockGuard lg(*graph);
      float phase = 2 * M_PI * std::rand() / RAND_MAX;
      for (int i = 0; i < settings.graph.size; i++)
        graph->data[i] = sin(i * 2 * M_PI / settings.graph.size + phase);
    }

    {
      PL::LockGuard lg(*parameters, *inputs);
      inputs->data->input1 = parameters->data->parameter1 + 1;
      inputs->data->input2 = parameters->data->parameter2 + 1;
      inputs->data->input3 = parameters->data->parameter3 + 1;
    }
  }

  blackBox->EraseAllConfigurations();
  blackBox->Restart();
}

//==============================================================================
