# BlackBox Modbus Protocol

This is the description of the Modbus protocol that satisfies [this](README.md) specification.

## Memory data format

* Multibyte numbers (0xAABBCCDD) are stored in the consecutive registers: 0xCCDD, 0xAABB.
* ASCII strings are stored in the consecutive registers with each register least significant byte being the first symbol and most significant byte being the second symbol.
* IPv4 addresses (AA.BB.CC.DD) are stored as 2 registers: 0xBBAA and 0xDDCC.
* IPv6 addresses (AABB:CCDD: ...) are stored in the consecutive registers: 0xBBAA, 0xDDCC ...).
* First two holding and input registers of all memory areas are bit registers.
* First two holding registers of all memory areas can be also accessed as coils with addresses starting with the address of the first holding register.
* Sticky bits in the bit input register signal the occurence of some event (like restart) and can be cleared by writing 1 to the correspondent holding register bit.

## Memory map

### General

<table>
<thead><tr><th colspan="2">Holding registers</th><th colspan="2">Input registers</th></tr></thead>
<tbody>
  <tr><td>0</td><td>Control bits:<br>0: Restart<br>1: Save configuration</td><td>0</td><td>Status bits:</td></tr>
  <tr><td>1</td><td>Clear sticky status bits</td><td>1</td><td>Sticky status bits:<br>0: Restarted</td></tr>
  <tr><td>2...17</td><td>Device name (ASCII)</td><td>2, 3</td><td>"PLBB" (ASCII)</td></tr>
  <tr><td>18</td><td>Selected hardware interface index</td><td>4</td><td>Memory map version (1)</td></tr>
  <tr><td>19</td><td>Selected server index</td><td>5...20</td><td>Hardware name (ASCII)</td></tr>
  <tr><td></td><td></td><td>21...23</td><td>Hardware version (major/minor/patch)</td></tr>
  <tr><td></td><td></td><td>24...39</td><td>Hardware UID (ASCII)</td></tr>
  <tr><td></td><td></td><td>40...55</td><td>Firmware name (ASCII)</td></tr>
  <tr><td></td><td></td><td>56...58</td><td>Firmware version (major/minor/patch)</td></tr>
  <tr><td></td><td></td><td>59</td><td>Number of hardware interfaces</td></tr>
  <tr><td></td><td></td><td>60</td><td>Number of servers</td></tr>
</tbody>
</table>

### Hardware interfaces

<table>
<thead>
  <tr><th colspan="6">Holding registers</th><th colspan="6">Input registers</th></tr>
  <tr>
    <th></th><th>All</th><th>UART</th><th>Network interface</th><th>Ethernet</th><th>Wi-Fi station</th>
    <th></th><th>All</th><th>UART</th><th>Network interface</th><th>Ethernet</th><th>Wi-Fi station</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td>100</td><td colspan="2">Control bits:<br>0: Enabled</td><td colspan="3">Control bits:<br>0: Enabled<br>1: IPv4 DHCP client enabled<br>2: IPv6 DHCP client enabled</td>
    <td>100</td><td colspan="2">Status bits:</td><td colspan="3">Status bits:<br>0: Connected</td>
  </tr>
  <tr>
    <td>101</td><td colspan="5">Clear sticky status bits:</td>
    <td>101</td><td colspan="5">Sticky status bits:</td>
  </tr>
  <tr>
    <td>102</td><td></td><td rowspan="2">Baud rate (uint32)</td><td colspan="3" rowspan="2">IPv4 address</td>
    <td>102</td><td colspan="5">Type:<br>0: Unknown<br>1: UART<br>2: Network interface<br>3: Ethernet<br>4: Wi-Fi station</td>
  </tr>
  <tr>
    <td>103</td><td></td>
    <td>103...118</td><td colspan="5">Name (ASCII)</td>
  </tr>
  <tr>
    <td>104</td><td></td><td>Data bits</td><td colspan="3" rowspan="2">IPv4 netmask</td>
    <td>119...126</td><td colspan="2"></td><td colspan="3">IPv6 link-local address</td>
  </tr>
  <tr>
    <td>105</td><td></td><td>Parity:<br>0: None<br>1: Even<br>2: Odd</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>106</td><td></td><td>Stop bits<br>0: 1<br>1: 1.5<br>2: 2</td><td colspan="3" rowspan="2">IPv4 gateway</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>107</td><td></td><td>Flow control<br>0: None<br>1: RTS<br>2: CTS<br>3: RTS and CTS</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>108...115</td><td colspan="2"></td><td colspan="3">IPv6 global address</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>116...131</td><td colspan="4"></td><td>SSID</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>132...163</td><td colspan="4"></td><td>Password</td>
    <td colspan="6"></td>
  </tr>
</tbody>
</table>
  
### Servers

<table>
<thead>
  <tr><th colspan="6">Holding registers</th><th colspan="6">Input registers</th></tr>
  <tr>
    <th></th><th>All</th><th>UART server</th><th>Network server</th><th>Modbus serial server</th><th>Modbus TCP server</th>
    <th></th><th>All</th><th>UART server</th><th>Network server</th><th>Modbus serial server</th><th>Modbus TCP server</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td>200</td><td colspan="5">Control bits:<br>0: Enabled</td>
    <td>200</td><td colspan="5">Status bits:</td>
  </tr>
  <tr>
    <td>201</td><td colspan="5">Clear sticky status bits:</td>
    <td>201</td><td colspan="5">Sticky status bits:</td>
  </tr>
  <tr>
    <td>202</td><td colspan="2"></td><td>Port</td><td colspan="2">Protocol:<br>0: RTU<br>1: ASCII<br>2: TCP</td>
    <td>202</td><td colspan="5">Type:<br>0: Unknown<br>1: UART server<br>2: Network server<br>3: Modbus serial server<br>4: Modbus TCP server</td>
  </tr>
  <tr>
    <td>203</td><td colspan="2"></td><td>Maximum number of clients</td><td colspan="2">Station address</td>
    <td>203...218</td><td colspan="5">Name (ASCII)</td>
  </tr>
  <tr>
    <td>204</td><td colspan="4"></td><td>Port</td>
    <td colspan="6"></td>
  </tr>
  <tr>
    <td>205</td><td colspan="4"></td><td>Maximum number of clients</td>
    <td colspan="6"></td>
  </tr>
</tbody>
</table>

### Application memory

Application memory starts at address 1000.
