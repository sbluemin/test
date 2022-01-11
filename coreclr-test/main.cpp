#include "net6.0/coreclr_delegates.h"
#include "net6.0/hostfxr.h"
#include "net6.0/nethost.h"

#include <dlfcn.h>
#include <iostream>
#include <limits.h>
#include <string>
#include <unistd.h>

using namespace std;

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

// Globals to hold hostfxr exports
hostfxr_initialize_for_runtime_config_fn init_fptr;
hostfxr_get_runtime_delegate_fn get_delegate_fptr;
hostfxr_close_fn close_fptr;

void *load_library(const char_t *path) {
  void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
  assert(h != nullptr);
  return h;
}
void *get_export(void *h, const char *name) {
  void *f = dlsym(h, name);
  assert(f != nullptr);
  return f;
}

bool load_hostfxr() {
  // Pre-allocate a large buffer for the path to hostfxr
  char_t buffer[MAX_PATH];
  size_t buffer_size = sizeof(buffer) / sizeof(char_t);
  int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
  if (rc != 0)
    return false;

  // Load hostfxr and get desired exports
  void *lib = load_library(buffer);
  init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(
      lib, "hostfxr_initialize_for_runtime_config");
  get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(
      lib, "hostfxr_get_runtime_delegate");
  close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

  return (init_fptr && get_delegate_fptr && close_fptr);
}

// Load and initialize .NET Core and get desired function pointer for scenario
load_assembly_and_get_function_pointer_fn
get_dotnet_load_assembly(const char_t *config_path) {
  // Load .NET Core
  void *load_assembly_and_get_function_pointer = nullptr;
  hostfxr_handle cxt = nullptr;
  int rc = init_fptr(config_path, nullptr, &cxt);
  if (rc != 0 || cxt == nullptr) {
    std::cerr << "Init failed: " << std::hex << std::showbase << rc
              << std::endl;
    close_fptr(cxt);
    return nullptr;
  }

  // Get the load assembly function pointer
  rc = get_delegate_fptr(cxt, hdt_load_assembly_and_get_function_pointer,
                         &load_assembly_and_get_function_pointer);
  if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
    std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc
              << std::endl;

  close_fptr(cxt);
  return (load_assembly_and_get_function_pointer_fn)
      load_assembly_and_get_function_pointer;
}

int main(int argc, char *argv[]) {
  cout << "argv are: ";

  for (auto i = 0; i < argc; ++i) {
    cout << argv[i] << endl;
  }

  char host_path[MAX_PATH];
  auto resolved = realpath(argv[0], host_path);
  assert(resolved != nullptr);

  string root_path = host_path;
  auto pos = root_path.find_last_of(DIR_SEPARATOR);
  assert(pos != string::npos);
  root_path = root_path.substr(0, pos + 1);

  //
  // STEP 1: Load HostFxr and get exported hosting functions
  //
  if (!load_hostfxr()) {
    assert(false && "Failure: load_hostfxr()");
    return EXIT_FAILURE;
  }

  //
  // STEP 2: Initialize and start the .NET Core runtime
  //
  const string config_path =
      root_path + "../DotNetLib/bin/Debug/net6.0/DotNetLib.runtimeconfig.json";
  load_assembly_and_get_function_pointer_fn
      load_assembly_and_get_function_pointer = nullptr;
  load_assembly_and_get_function_pointer =
      get_dotnet_load_assembly(config_path.c_str());
  assert(load_assembly_and_get_function_pointer != nullptr &&
         "Failure: get_dotnet_load_assembly()");

  //
  // STEP 3: Load managed assembly and get function pointer to a managed method
  //
  const string dotnetlib_path =
      root_path + "../DotNetLib/bin/Debug/net6.0/DotNetLib.dll";
  const char *dotnet_type = "DotNetLib.Lib, DotNetLib";
  const char *dotnet_type_method = "Hello";
  // <SnippetLoadAndGet>
  // Function pointer to managed delegate
  typedef void(CORECLR_DELEGATE_CALLTYPE * custom_entry_point_fn)(void *);
  custom_entry_point_fn custom = nullptr;
  int rc = load_assembly_and_get_function_pointer(
      dotnetlib_path.c_str(), dotnet_type, "CustomEntryPoint",
      "DotNetLib.Lib+CustomEntryPointDelegate, DotNetLib", nullptr,
      (void **)&custom);
  // </SnippetLoadAndGet>
  assert(rc == 0 && custom != nullptr &&
         "Failure: load_assembly_and_get_function_pointer()");

  void *handle = dlopen(NULL, RTLD_LAZY | RTLD_LOCAL);
  custom(handle);
}

extern "C" {
void FromManaged() { cout << "Hello, from managed!" << endl; }
void HelloWorld() { cout << "Hello, World!" << endl; }
}