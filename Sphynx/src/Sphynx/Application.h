#pragma once
#ifndef SphynxApplication
#define SphynxApplication
#include "Core.h"
#include "Events/Event.h"
#include "SpTime.h"
#include "Core/Graphics/Imgui.h"
#include "Pointer.h"
#include <type_traits>
#include <utility>
#include <memory>
#include <iostream>
#include "Core/ThreadPool.h"
#define Sphynx_Version "V0.5.0-PreAlpha"
namespace Sphynx {
	namespace Core {
		class IWindow;
	}
	class Application
	{
	private:
		//TODO:Finish the class.
		Core::ThreadPool threadpool = Core::ThreadPool();
		Events::EventSystem eventSystem;
		std::list<Pointer<Events::EventSystem>> EventSystemArray;
		Core::IWindow* MainWindow = nullptr;
		bool AppAlive = true;
#if defined(DEBUG)
		inline void StdLog(OnLog& e) { std::cout << e.msg; };
#endif
	public:
		Application();
		Application(Application&) = delete;
		Application(Application&&) = default;
		virtual ~Application();
		static Application* GetApplication();
		virtual void Update() = 0;
		virtual void Start() = 0;
		void Run();
		bool HasWindow()const noexcept{ return static_cast<bool>(MainWindow); };
		inline void CloseApplication() noexcept { AppAlive = false; };
		//Should the app be aware of the eventsystems or not ?(Currently it is)
		Events::EventSystem RequestNewEventSystem();
		void DeleteEventSystem(Events::EventSystem& e);
		template<typename EventType>
		inline void DispatchToActiveEventSystems(EventType& e, bool Immediate)
		{
			for (auto& es : EventSystemArray) {
				if (Immediate) {
					es->DispatchImmediate(e);
				}
				else es->QueueEvent(e);
			}
		}
		//////////////Getters///////////////////////

		inline Events::EventSystem* GetAppEventSystem()noexcept { return &eventSystem; };
		inline Core::IWindow* GetMainWindow()noexcept { return MainWindow; };
		//////////////Window Handling///////////////

		//Create the Main Window using a premade IWindow Pointer to allow Backend Choice(GL or DX)
		//TODO: LOL, NO more Choice.
		Core::IWindow* CreateMainWindow(Core::IWindow* window);
	};
	//To be defined in a client
	Application* CreateApplication();
}
#define GetApplication() ::Sphynx::Application::GetApplication()
#define GetMainWindow() GetApplication()->GetMainWindow()
#endif