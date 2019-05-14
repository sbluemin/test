## Requirements
- docker
- docker-compose
- cmake

## Build
- Make build directory. `build`
- `cd build && cmake ../`
- `make`

## Setting core pattern on host
- Write `kernel.core_pattern=/var/crash/core.%e.%h.%p.%t` to `/etc/sysctl.conf`
  
  or `sysctl -w kernel.core_pattern=/var/crash/core.%e.%h.%p.%t`
  
- Reboot or execute command `sysctl -p` on bash
- Restart docker service. (`sudo service docker restart`)

## Run
- `docker-compose build`
- `docker-compose up`
