#pragma once
#include <chrono>
#include <cstdint>
#include "Core/Module.h"
#define timer std::chrono::steady_clock


namespace Sphynx {
	class Time final {
	private:
		typedef std::chrono::duration<double> time;
		typedef std::chrono::time_point<timer> time_point;
		inline static thread_local time_point last;
		inline static thread_local std::uint64_t last_tick;
		inline static thread_local std::uint64_t diff_tick;
		inline static thread_local time DeltaTime;
	public:
		static double GetDeltaTime();
		static unsigned __int64 GetDeltaTicks();
		static unsigned __int64 GetTicks();
		// Inherited via Module
		static void Start();
		static void Update();
	};
}
