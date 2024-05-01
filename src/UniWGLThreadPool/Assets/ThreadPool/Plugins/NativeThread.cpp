
#include <thread>
#include <unordered_map>
#include <mutex>

namespace {
    std::unordered_map<std::thread::native_handle_type, std::thread> g_threads;
    std::mutex g_mutex;
}

typedef void (*Action)();

/// <summary>
/// create and start native thread
/// </summary>
/// <param name="f">invoked function</param>
/// <returns>thread handle</returns>
extern "C" std::intptr_t __stdcall UniWebGLCreateNativeThread(Action f)
{
    std::thread thread{ f };
    auto id = thread.native_handle();

    std::lock_guard<std::mutex> lock(g_mutex);
    g_threads[id] = std::move(thread);
    return (std::intptr_t)id;
}

/// <summary>
/// join native thread
/// </summary>
/// <param name="id">thread handle</param>
extern "C" void __stdcall UniWebGLJoinNativeThread(std::intptr_t id)
{
    std::lock_guard<std::mutex> lock(g_mutex);
    auto it = g_threads.find((std::thread::native_handle_type)id);
    if (it != g_threads.end()) {
        (*it).second.join();
        g_threads.erase(it);
    }
}
