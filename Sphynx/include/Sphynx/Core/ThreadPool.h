#pragma once
#include <mutex>
#include <thread>
#include <atomic>
#include <queue>
#include <future>
#include "Delegate.h"

namespace Sphynx {
	class Application;
}

namespace Sphynx::Core {
	class ThreadPool
	{
	private:
		inline static int MaxThreads;
		inline static std::vector<std::thread> threads;
		inline static std::deque<std::function<void()>> funcQueue;
		inline static std::condition_variable condition;
		inline static std::mutex ThreadPoolMutex;
		inline static std::atomic<bool> IsRunning{ false };
		inline static std::atomic<bool> FinishedWork{ true };
		inline static bool stopped = false;
		ThreadPool operator=(const ThreadPool& tp) = delete;
		ThreadPool(const ThreadPool&) = delete;
		static void Loop();
		static void JoinAll();
		static void Abort();
	public:
		static void Stop();
		static void Start(int num_threads = std::thread::hardware_concurrency());


		//Delegates ?
		static void Submit(std::function<void()> new_job);
		template<class T>
		static void Submit(Delegate<void, T, void> del) {
			{
				std::unique_lock<std::mutex> ul(ThreadPoolMutex);
				this->funcQueue.push_back( [&del]()->void { del(); });
			}
			condition.notify_one();
		}
		static int GetMaxThreads() { return MaxThreads; };
		static std::vector<std::thread>& GetThreads() { return threads; };
	};
}
