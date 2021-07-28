#pragma once
#include "KeyCode.h"
#include "Events/InputEvents.h"
#include "Vector.h"

namespace Sphynx {
<<<<<<< HEAD
	class Application;
	namespace Core{
	class IWindow;
	}
	//Per-Window.
	class Input {
	public:
		virtual bool IsKeyPressed(Keys key) { return 0; };
		virtual bool IsMouseButtonPressed(MouseButton button) { return 0; };
		virtual Vec2 GetMousePosition() { return Vector2<float>(0, 0); };

		friend class Core::IWindow;
		friend class Application;
=======
	//Per-Window.
	//TODO: Make it static.
	class Input {
	public:
		//not using pure virtuals to allow application to hold a Input Reference not pointer, i don't know why tho
		//as i mostly inherit and override these.

		virtual bool IsKeyPressed(Keys key) { return 0; };
		virtual bool IsMouseButtonPressed(MouseButton button) { return 0; };
		virtual Vec2 GetMousePosition() { return Vector2<float>(0, 0); };
>>>>>>> Dev_ComponentSystem
	};
}
