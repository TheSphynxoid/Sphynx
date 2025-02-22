#ifndef Sphynx_GL_Renderer
#define Sphynx_GL_Renderer
#include "Core/Graphics/Pipeline/Renderer.h"
#include <map>
#include "Events/WindowEvents.h"
#include "GLShader.h"
#include "GLMaterial.h"
#include "GLMesh.h"
#include "GLTexture.h"

namespace Sphynx::Core::Graphics::GL {
	class GLRenderer : public IRenderer
	{
		typedef std::vector<RenderObject> RenderObjectList;
		//Object To Be rendered are stored using the ProgramID as an index.
		std::map<unsigned int,std::vector<RenderObject>*> RenderQueue;
		Viewport CurrViewPort;
		//calls glViewport.
		void RendererResizeEvent(Events::OnWindowResize& e);
		//Default.
		inline static RenderObject DefaultRenderObject = RenderObject(nullptr, nullptr);
		virtual void OnSubmit(RenderObject rend) override;
	public:
		//Starts The Rendering Engine.
		virtual void Start(IWindow* app) override;
		//Does Rendering for one frame. Called in the Game Loop.
		virtual void Render() override;
		//Clears The Screen with color specified with SetClearColor(Vec4 color).
		virtual void Clear() override;
		virtual void SetDepthTesting(bool value) override;
		virtual void SetViewport(Viewport view) override;
		virtual const Viewport& GetViewport() override {
			return CurrViewPort;
		};
		//Submits Object for rendering.
	};
}
#else
#endif