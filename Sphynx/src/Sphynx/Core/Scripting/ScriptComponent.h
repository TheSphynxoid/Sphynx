#pragma once
#include "Component.h"
#include "Script.h"

namespace Sphynx::Mono {
	class GameObjectWrapper;
}

namespace Sphynx::Core {
	class ScriptComponent final : public Component 
	{
	private:
		std::vector<Scripting::Script*> scripts;
		Mono::GameObjectWrapper* GOWrapper;
	public:
		ScriptComponent();

		// Inherited via Component
		virtual void Start() override;
		virtual void Update() override;
		virtual void FixedUpdate() override;
		virtual void OnComponentAttach(GameObject* parent) override;
		virtual void OnComponentDetach() override;
		void AddComponent(std::string name);
		const char* GetName() override;
	};
}